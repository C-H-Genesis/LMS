
-- Create the Courses table
CREATE TABLE Courses (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,       -- Primary key
    CourseCode NVARCHAR(100) NOT NULL,              -- Course code
    CourseName NVARCHAR(255) NOT NULL,              -- Course name
    TeacherName NVARCHAR(100),                      -- Foreign key to Teachers table (unique constraint)

    CONSTRAINT FK_Courses_TeacherName FOREIGN KEY (TeacherName) REFERENCES Teacher(TeacherName) ON DELETE SET NULL
);

-- Create the Enrollments table
CREATE TABLE Enrollments (
    Id INT NOT NULL PRIMARY KEY IDENTITY,          -- Primary key
    CourseId UNIQUEIDENTIFIER NOT NULL,            -- Foreign key to Courses table
    UserId UNIQUEIDENTIFIER NOT NULL,                 -- Foreign key to Users table
    Status BIT NOT NULL,                           -- Registration status (true = first time, false = second time)
    EnrollmentDate DATETIME NOT NULL,             -- Enrollment date

    CONSTRAINT FK_Enrollments_Courses FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Enrollments_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
