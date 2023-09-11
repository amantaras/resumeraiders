-- Delete sample data from Applications table
DELETE FROM Applications;

-- Delete sample data from Skills table
DELETE FROM Skills;

-- Delete sample data from Users table
DELETE FROM Users;

-- Delete sample data from Jobs table
DELETE FROM Jobs;

-- Delete sample data from Categories table
DELETE FROM Categories;

-- Delete sample data from Companies table
DELETE FROM Companies;

--- Insert sample data into Companies table
INSERT INTO Companies (CompanyName, Location)
VALUES ('Acme Inc.', 'New York'),
       ('Globex Corporation', 'Springfield'),
       ('Initech', 'Austin');

-- Insert sample data into Categories table
INSERT INTO Categories (CategoryName)
VALUES ('Software Development'),
       ('Marketing'),
       ('Sales');

-- Insert sample data into Jobs table
INSERT INTO Jobs (JobTitle, Description, Location, Salary, CompanyID, CategoryID)
SELECT 'Software Engineer', 'Develop software applications', 'New York', 100000, CompanyID, CategoryID
FROM Companies, Categories
WHERE Companies.CompanyName = 'Acme Inc.' AND Categories.CategoryName = 'Software Development'
UNION ALL
SELECT 'Marketing Manager', 'Develop marketing strategies', 'Springfield', 80000, CompanyID, CategoryID
FROM Companies, Categories
WHERE Companies.CompanyName = 'Globex Corporation' AND Categories.CategoryName = 'Marketing'
UNION ALL
SELECT 'Sales Representative', 'Sell products to customers', 'Austin', 60000, CompanyID, CategoryID
FROM Companies, Categories
WHERE Companies.CompanyName = 'Initech' AND Categories.CategoryName = 'Sales';

-- Insert sample data into Users table
INSERT INTO Users (FirstName, LastName, Email)
VALUES ('John', 'Doe', 'johndoe@example.com'),
       ('Jane', 'Doe', 'janedoe@example.com'),
       ('Bob', 'Smith', 'bobsmith@example.com');

-- Insert sample data into Applications table
INSERT INTO Applications (UserID, JobID, ApplicationDate)
SELECT UserID, JobID, '2021-01-01'
FROM Users, Jobs
WHERE Users.FirstName = 'John' AND Jobs.JobTitle = 'Software Engineer'
UNION ALL
SELECT UserID, JobID, '2021-02-01'
FROM Users, Jobs
WHERE Users.FirstName = 'Jane' AND Jobs.JobTitle = 'Marketing Manager'
UNION ALL
SELECT UserID, JobID, '2021-03-01'
FROM Users, Jobs
WHERE Users.FirstName = 'Bob' AND Jobs.JobTitle = 'Sales Representative';

-- Insert sample data into Skills table
INSERT INTO Skills (SkillName, JobID)
SELECT 'C#', JobID
FROM Jobs
WHERE JobTitle = 'Software Engineer';

INSERT INTO Skills (SkillName, JobID)
SELECT 'Marketing Strategy', JobID
FROM Jobs
WHERE JobTitle = 'Marketing Manager';

INSERT INTO Skills (SkillName, JobID)
SELECT 'Sales', JobID
FROM Jobs
WHERE JobTitle = 'Sales Representative';