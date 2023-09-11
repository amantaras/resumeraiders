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

    public class HttpTriggerApplications
    {
        private readonly ILogger _logger;

        public HttpTriggerApplications(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTriggerApplications>();
        }

        [Function("GetApplications")]
        public async Task<HttpResponseData> GetApplications(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "applications")] HttpRequestData req)
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "applications/{userId}/{applicationId}")] HttpRequestData req, int userId, int applicationId)
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "applications/{id}")] HttpRequestData req, int id)
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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "applications")] HttpRequestData req)
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
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "applications/{id}")] HttpRequestData req, int id)
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
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "applications/{id}")] HttpRequestData req, int id)
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
    }
}