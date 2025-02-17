import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule  } from '@angular/forms';
import { HttpClientModule, provideHttpClient, withFetch } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { StudentDashboardComponent } from './components/student-dashboard/student-dashboard.component';
import { TeacherDashboardComponent } from './components/teacher-dashboard/teacher-dashboard.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { FinanceDashboardComponent } from './components/finance-dashboard/finance-dashboard.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { StudentFeesComponent } from './components/student-fees/student-fees.component';
import { CourseRegistrationComponent } from './components/course-registration/course-registration.component';
import { GradesComponent } from './components/grades/grades.component';
import { ProfileComponent } from './components/profile/profile.component';
import { TeacherMyCoursesComponent } from './components/teacher-my-courses/teacher-my-courses.component';
import { TeacherAssignmentsComponent } from './components/teacher-assignments/teacher-assignments.component';
import { ManageUsersComponent } from './components/manage-users/manage-users.component';
import { ManageCoursesComponent } from './components/manage-courses/manage-courses.component';
import { ReportsComponent } from './components/reports/reports.component';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    StudentDashboardComponent,
    TeacherDashboardComponent,
    AdminDashboardComponent,
    FinanceDashboardComponent,
    NavbarComponent,
    StudentFeesComponent,
    CourseRegistrationComponent,
    GradesComponent,
    ProfileComponent,
    TeacherMyCoursesComponent,
    TeacherAssignmentsComponent,
    ManageUsersComponent,
    ManageCoursesComponent,
    ReportsComponent,
    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    FormsModule ,
  ],
  providers: [
    provideHttpClient(withFetch())
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
