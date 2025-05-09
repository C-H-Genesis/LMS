import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { catchError, of } from 'rxjs';
import { ok } from 'assert';
import { Router } from '@angular/router';
import { Console, error, log } from 'console';

@Component({
    selector: 'app-manage-users',
    templateUrl: './manage-users.component.html',
    styleUrl: './manage-users.component.css',
    standalone: false
})
export class ManageUsersComponent implements OnInit {
  users: any[] = [];
  isCreatingUser: boolean = false; // Controls form visibility
  user = {
    userId : '', 
    fullName: '',
    username: '',
    Email:'',
    role: '',
    usertype: '',
  };
  selectedrole : string = '';
  roles: string[] = ['Admin', 'Student', 'Teacher', 'Finance'];
  selectedUser: any = null;
  showModal = false;

  constructor(private adminService: AdminService, private router: Router) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.adminService.getAllUsers().subscribe((data: any) => {
      this.users = data;
    });
  }
  openModal() {  
    this.showModal = true;
  }

  loadUsersById(userId: string) {
    this.adminService.getAllUsersById(userId).subscribe((data: any) => {
      this.selectedUser = data; // Store the selected user's details
    }, (error) => {
      console.log("Error fetching user details:", error);
    });
  }

  loadUsersByRole() { 
    if (!this.selectedrole) {
      // If no role is selected, fetch all users
      this.adminService.getAllUsers().subscribe((data: any) => {
        this.users = data || [];
      });
    } else {
      // Fetch users based on selected role
      this.adminService.getAllUsersByRole(this.selectedrole).subscribe((data: any) => {
        this.users = data || [];
      });
    }
  }

  onSubmit() {
    this.adminService.createUser(this.user).subscribe(() => {
      alert('User created successfully!');
      this.router.navigate(['/manage-users']);
      this.isCreatingUser = false; // Hide the form
      this.loadUsers(); // Reload the list
          
    }, 
    (error) => {
      alert("Failed to create User");
      console.log(error);
      
    });
  }

  onDeleteUser(userId: string) {

    if (confirm('Are you sure you want to delete this user?')) {
      this.adminService.deleteUser(userId)
        .pipe(
          catchError((error) => {
            console.error('Error deleting user:', error);
            alert('Failed to delete the user.');
            return of(null);
          })
        )
        .subscribe((response) => {
          if (response) {
            alert('User deleted successfully!');
            this.users = this.users.filter((user) => user.userId !== userId); // Update the list locally
            this.router.navigate(['/manage-users']);
          }
        });
    }
  }


  updateUser() {
    if (!this.selectedUser.userId) {
      alert("User ID is missing!");
      return;
    }
  
    this.adminService.updateUserInfo(this.selectedUser.userId)
      .subscribe(() => {
        alert('User updated successfully!');
        this.showModal = false;
        this.loadUsers(); // Refresh user list
      }, error => {
        alert(error.error);
      });
  }
  
  closeModal() {
    this.showModal = false;
    this.selectedUser = null;
  }
  
}
