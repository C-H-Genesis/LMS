import { Component, OnInit } from '@angular/core';
import { StudentService } from '../../services/student.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  profile: any = null;

  constructor(private studentService: StudentService) {}

  ngOnInit(): void {
    this.fetchProfile();
  }

  fetchProfile(): void {
    this.studentService.getProfile().subscribe(
      (data) => {
        this.profile = data; // Assign fetched data to the profile object
      },
      (error) => {
        console.error('Error fetching profile:', error);
        alert('Failed to fetch profile information.');
      }
    );
  }
  

  updateProfile(): void {
    if (this.profile) {
      this.studentService.updateProfile(this.profile).subscribe(
        (updatedProfile) => {
          console.log('Profile updated:', updatedProfile);
          this.profile = updatedProfile; // Update the local profile object
          alert('Profile updated');
        },
        (error) => {
          console.error('Error updating profile:', error);
          alert('Failed to update profile information.');
        }
      );
    }
  }
}
