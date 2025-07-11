import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
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
import { StudentAssignmentComponent } from './student-assignment/student-assignment.component';
import { LearningDashboardComponent } from './components/learning-dashboard/learning-dashboard.component';
import { CollapseModule } from '@coreui/angular';
import { NavbarModule } from '@coreui/angular';
import { NavModule } from '@coreui/angular';
import { ButtonModule } from '@coreui/angular';
import { SharedModule } from '@coreui/angular';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component'; // For <c-container>




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
    StudentAssignmentComponent,
    LearningDashboardComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    FormsModule ,
    BrowserModule,
    CollapseModule,
    NavbarModule,
    NavModule,
    ButtonModule,
    SharedModule
   
    
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    provideHttpClient(withFetch())
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }



