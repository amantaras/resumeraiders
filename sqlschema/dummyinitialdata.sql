
-- Insert sample data into Companies tables
MERGE INTO Companies AS target
USING (VALUES 
    (1, 'Acme Inc.', 'New York'),
    (2, 'Globex Corporation', 'Springfield'),
    (3, 'Initech', 'Austin')
) AS source (CompanyID, CompanyName, Location)
ON target.CompanyID = source.CompanyID
WHEN NOT MATCHED THEN
    INSERT (CompanyID, CompanyName, Location)
    VALUES (source.CompanyID, source.CompanyName, source.Location);

-- Insert sample data into Categories table
MERGE INTO Categories AS target
USING (VALUES
    (1, 'Software Development'),
    (2, 'Marketing'),
    (3, 'Sales')
) AS source (CategoryID, CategoryName)
ON target.CategoryID = source.CategoryID
WHEN NOT MATCHED THEN
    INSERT (CategoryID, CategoryName)
    VALUES (source.CategoryID, source.CategoryName);

-- Insert sample data into Jobs table
MERGE INTO Jobs AS target
USING (VALUES
    (1, 'Software Engineer', 'Develop software applications', 'New York', 100000, 1, 1),
    (2, 'Marketing Manager', 'Manage marketing campaigns', 'Springfield', 80000, 2, 2),
    (3, 'Sales Representative', 'Sell products to customers', 'Austin', 60000, 3, 3)
) AS source (JobID, JobTitle, Description, Location, Salary, CompanyID, CategoryID)
ON target.JobID = source.JobID
WHEN NOT MATCHED THEN
    INSERT (JobID, JobTitle, Description, Location, Salary, CompanyID, CategoryID)
    VALUES (source.JobID, source.JobTitle, source.Description, source.Location, source.Salary, source.CompanyID, source.CategoryID);

-- Insert sample data into Users table
MERGE INTO Users AS target
USING (VALUES
    (1, 'John', 'Doe', 'johndoe@example.com'),
    (2, 'Jane', 'Doe', 'janedoe@example.com'),
    (3, 'Bob', 'Smith', 'bobsmith@example.com')
) AS source (UserID, FirstName, LastName, Email)
ON target.UserID = source.UserID
WHEN NOT MATCHED THEN
    INSERT (UserID, FirstName, LastName, Email)
    VALUES (source.UserID, source.FirstName, source.LastName, source.Email);

-- Insert sample data into Applications table
MERGE INTO Applications AS target
USING (VALUES
    (1, 1, 1, '2022-01-01'),
    (2, 2, 1, '2022-01-02'),
    (3, 3, 2, '2022-01-03')
) AS source (ApplicationID, UserID, JobID, ApplicationDate)
ON target.ApplicationID = source.ApplicationID
WHEN NOT MATCHED THEN
    INSERT (ApplicationID, UserID, JobID, ApplicationDate)
    VALUES (source.ApplicationID, source.UserID, source.JobID, source.ApplicationDate);

-- Insert sample data into Skills table
MERGE INTO Skills AS target
USING (VALUES
    (1, 'C#', 1),
    (2, 'Java', 1),
    (3, 'Marketing Strategy', 2)
) AS source (SkillID, SkillName, JobID)
ON target.SkillID = source.SkillID
WHEN NOT MATCHED THEN
    INSERT (SkillID, SkillName, JobID)
    VALUES (source.SkillID, source.SkillName, source.JobID);