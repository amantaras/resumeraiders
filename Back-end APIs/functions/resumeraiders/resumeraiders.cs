using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class Application
    {
        public int ApplicationID { get; set; }
        public int UserID { get; set; }
        public int JobID { get; set; }
        public DateTime ApplicationDate { get; set; }
    }

     public class Company
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public string About { get; set; }
    }

    public class Job
    {
        public int JobID { get; set; }
        public string JobTitle { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? Salary { get; set; }
        public int? CompanyID { get; set; }
        public int? CategoryID { get; set; }
        public string? ContractType { get; set; }
        public string?  Benefits { get; set; }
        public string?  ApplicationProcess { get; set; }
        public string? ReportsTo { get; set; }
        public string? Tags { get; set; }

    }


    public class HttpTriggerResumeRaiders
    {
        private readonly ILogger _logger;

        public HttpTriggerResumeRaiders(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTriggerResumeRaiders>();
        }

        [Function("GetApplications")]
        public async Task<HttpResponseData> GetApplications(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "applications")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to get all applications.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Applications";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "application/json");

                        var applications = new List<Application>();
                        while (reader.Read())
                        {
                            var application = new Application
                            {
                                ApplicationID = reader.GetInt32(0),
                                UserID = reader.GetInt32(1),
                                JobID = reader.GetInt32(2),
                                ApplicationDate = reader.GetDateTime(3)
                                
                            };
                            applications.Add(application);
                        }

                        string json = JsonSerializer.Serialize(applications);
                        response.WriteString(json);

                        return response;
                    }
                }
            }
        }


        [Function("GetApplicationByUserIDandApplicationID")]
        public async Task<HttpResponseData> GetApplicationByUserIDandApplicationID(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "applications/{userId}/{applicationId}")] HttpRequestData req, int userId, int applicationId)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to get application with id {applicationId} for user with id {userId}.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Applications WHERE UserID = @UserID AND ApplicationID = @ApplicationID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@ApplicationID", applicationId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var application = new Application
                            {
                                ApplicationID = reader.GetInt32(0),
                                UserID = reader.GetInt32(1),
                                JobID = reader.GetInt32(2),
                                ApplicationDate = reader.GetDateTime(3)
                            };

                            var response = req.CreateResponse(HttpStatusCode.OK);
                            response.Headers.Add("Content-Type", "application/json");

                            string json = JsonSerializer.Serialize(application);
                            response.WriteString(json);

                            return response;
                        }
                        else
                        {
                            return req.CreateResponse(HttpStatusCode.NotFound);
                        }
                    }
                }
            }
        }

        [Function("GetApplicationById")]
        public async Task<HttpResponseData> GetApplicationById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "applications/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to get application with id {id}.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Applications WHERE ApplicationID = @ApplicationID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var application = new Application
                            {
                                ApplicationID = reader.GetInt32(0),
                                UserID = reader.GetInt32(1),
                                JobID = reader.GetInt32(2),
                                ApplicationDate = reader.GetDateTime(3)
                            };

                            var response = req.CreateResponse(HttpStatusCode.OK);
                            response.Headers.Add("Content-Type", "application/json");

                            string json = JsonSerializer.Serialize(application);
                            response.WriteString(json);

                            return response;
                        }
                        else
                        {
                            return req.CreateResponse(HttpStatusCode.NotFound);
                        }
                    }
                }
            }
        }

        [Function("CreateApplication")]
        public async Task<HttpResponseData> CreateApplication(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "applications")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to create a new application.");

            string requestBody = await req.ReadAsStringAsync();
            Application application = JsonSerializer.Deserialize<Application>(requestBody);

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Applications (UserID, JobID, ApplicationDate) VALUES (@UserID, @JobID, @ApplicationDate)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", application.UserID);
                    command.Parameters.AddWithValue("@JobID", application.JobID);
                    command.Parameters.AddWithValue("@ApplicationDate", application.ApplicationDate);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                        response.WriteString($"Application for job {application.JobID} added successfully");

                        return response;
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
            }
        }

        [Function("UpdateApplication")]
        public async Task<HttpResponseData> UpdateApplication(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "applications/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to update application with id {id}.");

            string requestBody = await req.ReadAsStringAsync();
            Application application = JsonSerializer.Deserialize<Application>(requestBody);

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Applications SET UserID = @UserID, JobID = @JobID, ApplicationDate = @ApplicationDate WHERE ApplicationID = @ApplicationID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", id);
                    command.Parameters.AddWithValue("@UserID", application.UserID);
                    command.Parameters.AddWithValue("@JobID", application.JobID);
                    command.Parameters.AddWithValue("@ApplicationDate", application.ApplicationDate);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                        response.WriteString($"Application with id {id} updated successfully");

                        return response;
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
            }
        }

        [Function("DeleteApplication")]
        public async Task<HttpResponseData> DeleteApplication(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "applications/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to delete application with id {id}.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Applications WHERE ApplicationID = @ApplicationID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                        response.WriteString($"Application with id {id} deleted successfully");

                        return response;
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
            }
        }
         [Function("GetCompanies")]
        //[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies")]
        public async Task<HttpResponseData> GetCompanies([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to get all companies.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Companies";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "application/json");

                        var companies = new List<Company>();
                        while (reader.Read())
                        {
                            var company = new Company
                            {
                                CompanyID = reader.GetInt32(0),
                                CompanyName = reader.GetString(1),
                                Location = reader.GetString(2),
                                About = reader.GetString(3)
                            };
                            companies.Add(company);
                        }

                        string json = JsonSerializer.Serialize(companies);
                        response.WriteString(json);

                        return response;
                    }
                }
            }
        }

        [Function("GetCompanyById")]
        //[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{id}")]
        public async Task<HttpResponseData> GetCompanyById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to get company with id {id}.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Companies WHERE CompanyID = @CompanyID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyID", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var company = new Company
                            {
                                CompanyID = reader.GetInt32(0),
                                CompanyName = reader.GetString(1),
                                Location = reader.GetString(2),
                                About = reader.GetString(3)
                            };

                            var response = req.CreateResponse(HttpStatusCode.OK);
                            response.Headers.Add("Content-Type", "application/json");

                            string json = JsonSerializer.Serialize(company);
                            response.WriteString(json);

                            return response;
                        }
                        else
                        {
                            return req.CreateResponse(HttpStatusCode.NotFound);
                        }
                    }
                }
            }
        }

        [Function("CreateCompany")] 
        //[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies")]
        public async Task<HttpResponseData> CreateCompany([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to create a new company.");

            string requestBody = await req.ReadAsStringAsync();
            Company company = JsonSerializer.Deserialize<Company>(requestBody);

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Companies (CompanyName, Location) VALUES (@CompanyName, @Location)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //command.Parameters.AddWithValue("@CompanyID", company.CompanyID);
                    command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                    command.Parameters.AddWithValue("@Location", company.Location);
                     command.Parameters.AddWithValue("@About", company.About);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                        response.WriteString($"Company {company.CompanyName} added successfully");

                        return response;
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
            }
        }

        [Function("UpdateCompany")]
        //[HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{id}")]
        public async Task<HttpResponseData> UpdateCompany([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to update company with id {id}.");

            string requestBody = await req.ReadAsStringAsync();
            Company company = JsonSerializer.Deserialize<Company>(requestBody);

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Companies SET CompanyName = @CompanyName, Location = @Location WHERE CompanyID = @CompanyID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyID", id);
                    command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                    command.Parameters.AddWithValue("@Location", company.Location);
                    command.Parameters.AddWithValue("@About", company.About);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                        response.WriteString($"Company with id {id} updated successfully");

                        return response;
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
            }
        }

        [Function("DeleteCompany")]
        //[HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companies/{id}")]
        public async Task<HttpResponseData> DeleteCompany([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "companies/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to delete company with id {id}.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Companies WHERE CompanyID = @CompanyID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyID", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                        response.WriteString($"Company with id {id} deleted successfully");

                        return response;
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
            }
        }

[Function("GetJobs")]
        public async Task<HttpResponseData> GetJobs(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "jobs")] HttpRequestData req,
            FunctionContext context)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to get all jobs.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Jobs";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "application/json");

                        var jobs = new List<Job>();
                        while (reader.Read())
                        {
                            var job = new Job
                            {
                                JobID = reader.GetInt32(0),
                                JobTitle = reader.GetString(1),
                                Description = reader.GetString(2),
                                Location = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Salary = reader.GetDecimal(4),
                                CompanyID = reader.GetInt32(5),
                                CategoryID = reader.GetInt32(6),
                                ContractType = reader.GetString(7),
                                Benefits = reader.IsDBNull(8) ? null : reader.GetString(8),
                                ApplicationProcess = reader.IsDBNull(9) ? null : reader.GetString(9),
                                ReportsTo = reader.IsDBNull(10) ? null : reader.GetString(10),
                                Tags = reader.IsDBNull(11) ? null : reader.GetString(11)
                            };
                            jobs.Add(job);
                        }

                        string json = JsonSerializer.Serialize(jobs);
                        response.WriteString(json);

                        return response;
                    }
                }
            }
        }

        [Function("GetJobById")]
        public async Task<HttpResponseData> GetJobById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "jobs/{id}")] HttpRequestData req,
            int id,
            FunctionContext context)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to get job with id {id}.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM Jobs WHERE JobID = @JobID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@JobID", id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var job = new Job
                            {
                                JobID = reader.GetInt32(0),
                                JobTitle = reader.GetString(1),
                                Description = reader.GetString(2),
                                Location = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Salary = reader.GetDecimal(4),
                                CompanyID = reader.GetInt32(5),
                                CategoryID = reader.GetInt32(6),
                                ContractType = reader.GetString(7),
                                Benefits = reader.IsDBNull(8) ? null : reader.GetString(8),
                                ApplicationProcess = reader.IsDBNull(9) ? null : reader.GetString(9),
                                ReportsTo = reader.IsDBNull(10) ? null : reader.GetString(10),
                                Tags = reader.IsDBNull(11) ? null : reader.GetString(11)
                            };

                            var response = req.CreateResponse(HttpStatusCode.OK);
                            response.Headers.Add("Content-Type", "application/json");

                            string json = JsonSerializer.Serialize(job);
                            response.WriteString(json);

                            return response;
                        }
                        else
                        {
                            return req.CreateResponse(HttpStatusCode.NotFound);
                        }
                    }
                }
            }
        }

        [Function("CreateJob")]
        public async Task<HttpResponseData> CreateJob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "jobs")] HttpRequestData req,
            FunctionContext context)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to create a new job.");

            string requestBody = await req.ReadAsStringAsync();
            Job job = JsonSerializer.Deserialize<Job>(requestBody);

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Jobs (JobTitle, Description, Location, Salary, CompanyID, CategoryID,ContractType,Benefits,ApplicationProcess,ReportsTo,Tags) VALUES (@JobTitle, @Description, @Location, @Salary, @CompanyID, @CategoryID,@ContractType,@Benefits,@ApplicationProcess,@ReportsTo,@Tags)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@JobTitle", job.JobTitle);
                    command.Parameters.AddWithValue("@Description", job.Description);
                    command.Parameters.AddWithValue("@Location", job.Location);
                    command.Parameters.AddWithValue("@Salary", job.Salary);
                    command.Parameters.AddWithValue("@CompanyID", job.CompanyID);
                    command.Parameters.AddWithValue("@CategoryID", job.CategoryID);
                    command.Parameters.AddWithValue("@CategoryID", job.CategoryID);
                    command.Parameters.AddWithValue("@ContractType", job.ContractType);
                    command.Parameters.AddWithValue("@Benefits", job.Benefits);
                    command.Parameters.AddWithValue("@ApplicationProcess", job.ApplicationProcess);
                    command.Parameters.AddWithValue("@ReportsTo", job.ReportsTo);
                    command.Parameters.AddWithValue("@Tags", job.Tags);


                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                        response.WriteString($"Job {job.JobTitle} added successfully");

                        return response;
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
            }
        }

        [Function("UpdateJob")]
        public async Task<HttpResponseData> UpdateJob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "jobs/{id}")] HttpRequestData req,
            int id,
            FunctionContext context)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to update job with id {id}.");

            string requestBody = await req.ReadAsStringAsync();
            Job job = JsonSerializer.Deserialize<Job>(requestBody);

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Jobs SET JobTitle = @JobTitle, Tags=@Tags, ReportsTo=@ReportsTo,ApplicationProcess=@ApplicationProcess , Benefits=@Benefits,ContractType=@ContractType, Description = @Description, Location = @Location, Salary = @Salary, CompanyID = @CompanyID, CategoryID = @CategoryID WHERE JobID = @JobID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@JobID", id);
                    command.Parameters.AddWithValue("@JobTitle", job.JobTitle);
                    command.Parameters.AddWithValue("@Description", job.Description);
                    command.Parameters.AddWithValue("@Location", job.Location);
                    command.Parameters.AddWithValue("@Salary", job.Salary);
                    command.Parameters.AddWithValue("@CompanyID", job.CompanyID);
                    command.Parameters.AddWithValue("@CategoryID", job.CategoryID);
                    command.Parameters.AddWithValue("@ContractType", job.ContractType);
                    command.Parameters.AddWithValue("@Benefits", job.Benefits);
                    command.Parameters.AddWithValue("@ApplicationProcess", job.ApplicationProcess);
                    command.Parameters.AddWithValue("@ReportsTo", job.ReportsTo);
                    command.Parameters.AddWithValue("@Tags", job.Tags);


                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                        response.WriteString($"Job with id {id} updated successfully");

                        return response;
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
            }
        }

        [Function("DeleteJob")]
        public async Task<HttpResponseData> DeleteJob(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "jobs/{id}")] HttpRequestData req,
            int id,
            FunctionContext context)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request to delete job with id {id}.");

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Jobs WHERE JobID = @JobID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@JobID", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        var response = req.CreateResponse(HttpStatusCode.OK);
                        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                        response.WriteString($"Job with id {id} deleted successfully");

                        return response;
                    }
                    else
                    {
                        return req.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
            }
        }

    }
}