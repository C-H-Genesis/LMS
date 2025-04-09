import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { StudentDashboardComponent } from './components/student-dashboard/student-dashboard.component';
import { TeacherDashboardComponent } from './components/teacher-dashboard/teacher-dashboard.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { FinanceDashboardComponent } from './components/finance-dashboard/finance-dashboard.component';
import { roleGuard } from "./auth/role-guard.guard";
import { ManageCoursesComponent } from './components/manage-courses/manage-courses.component';
import { ManageUsersComponent } from './components/manage-users/manage-users.component';
import { ReportsComponent } from './components/reports/reports.component';
import { TeacherMyCoursesComponent } from './components/teacher-my-courses/teacher-my-courses.component';
import { TeacherAssignmentsComponent } from './components/teacher-assignments/teacher-assignments.component';
import { StudentFeesComponent } from './components/student-fees/student-fees.component';
import { CourseRegistrationComponent } from './components/course-registration/course-registration.component';
import { GradesComponent } from './components/grades/grades.component';
import { ProfileComponent } from './components/profile/profile.component';
import { StudentAssignmentComponent } from './student-assignment/student-assignment.component';
import { LearningDashboardComponent } from './components/learning-dashboard/learning-dashboard.component';


const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Admin Routes
  { path: 'admin-dashboard', component: AdminDashboardComponent, canActivate: [roleGuard], data: { role: 'Admin' } },
  { path: 'manage-users', component: ManageUsersComponent, canActivate: [roleGuard], data: { role: 'Admin' } },
  { path: 'manage-courses', component: ManageCoursesComponent, canActivate: [roleGuard], data: { role: 'Admin' } },
  { path: 'reports', component: ReportsComponent, canActivate: [roleGuard], data: { role: 'Admin' } },

  // Teacher Routes
  { path: 'teacher-dashboard', component: TeacherDashboardComponent, canActivate: [roleGuard], data: { role: 'Teacher' } },
  { path: 'my-courses', component: TeacherMyCoursesComponent, canActivate: [roleGuard], data: { role: 'Teacher' } },
  { path: 'assignments', component: TeacherAssignmentsComponent, canActivate: [roleGuard], data: { role: 'Teacher' } },

  
  // Student Routes
  { path: 'student-dashboard', component: StudentDashboardComponent, canActivate: [roleGuard], data: { role: 'Student' } },
  { path: 'fees', component: StudentFeesComponent, canActivate: [roleGuard], data: { role: 'Student' } },
  { path: 'course-registration', component: CourseRegistrationComponent, canActivate: [roleGuard], data: { role: 'Student' } },
  { path: 'student-Assignments', component: StudentAssignmentComponent, canActivate: [roleGuard], data: { role: 'Student' } },
  { path: 'grades', component: GradesComponent, canActivate: [roleGuard], data: { role: 'Student' } },
  { path: 'profile', component: ProfileComponent, canActivate: [roleGuard], data: { role: 'Student' } },
  { path: 'learning-dashboard', component: LearningDashboardComponent, canActivate: [roleGuard], data: { role: 'Student' } },



  { path: 'finance-dashboard', component: FinanceDashboardComponent, canActivate: [roleGuard], data: { role: 'Finance' } },
  { path: '**', redirectTo: 'login' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
