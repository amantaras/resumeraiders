CREATE TABLE Companies (
    CompanyID INT PRIMARY KEY,
    CompanyName VARCHAR(50) NOT NULL,
    Location VARCHAR(50) NOT NULL
);

CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL
);

CREATE TABLE Jobs (
    JobID INT PRIMARY KEY,
    JobTitle VARCHAR(50) NOT NULL,
    Description VARCHAR(MAX) NOT NULL,
    Location VARCHAR(50) NOT NULL,
    Salary DECIMAL(10, 2) NOT NULL,
    CompanyID INT NOT NULL,
    CategoryID INT NOT NULL,
    CONSTRAINT FK_Jobs_Companies FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
    CONSTRAINT FK_Jobs_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);

CREATE TABLE Users (
    UserID INT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(50) NOT NULL
);

CREATE TABLE Applications (
    ApplicationID INT PRIMARY KEY,
    UserID INT NOT NULL,
    JobID INT NOT NULL,
    ApplicationDate DATETIME NOT NULL,
    CONSTRAINT FK_Applications_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Applications_Jobs FOREIGN KEY (JobID) REFERENCES Jobs(JobID)
);

CREATE TABLE Skills (
    SkillID INT PRIMARY KEY,
    SkillName VARCHAR(50) NOT NULL,
    JobID INT NOT NULL,
    CONSTRAINT FK_Skills_Jobs FOREIGN KEY (JobID) REFERENCES Jobs(JobID)
);
