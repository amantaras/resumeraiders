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
    public class Job
    {
        public int JobID { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public int CompanyID { get; set; }
        public int CategoryID { get; set; }
    }

    public class HttpTriggerJobs
    {
        private readonly ILogger _logger;

        public HttpTriggerJobs(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTriggerJobs>();
        }

        [Function("GetJobs")]
        public async Task<HttpResponseData> GetJobs(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobs")] HttpRequestData req,
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
                                Location = reader.GetString(3),
                                Salary = reader.GetDecimal(4),
                                CompanyID = reader.GetInt32(5),
                                CategoryID = reader.GetInt32(6)
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobs/{id}")] HttpRequestData req,
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
                                Location = reader.GetString(3),
                                Salary = reader.GetDecimal(4),
                                CompanyID = reader.GetInt32(5),
                                CategoryID = reader.GetInt32(6)
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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobs")] HttpRequestData req,
            FunctionContext context)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to create a new job.");

            string requestBody = await req.ReadAsStringAsync();
            Job job = JsonSerializer.Deserialize<Job>(requestBody);

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Jobs (JobTitle, Description, Location, Salary, CompanyID, CategoryID) VALUES (@JobTitle, @Description, @Location, @Salary, @CompanyID, @CategoryID)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@JobTitle", job.JobTitle);
                    command.Parameters.AddWithValue("@Description", job.Description);
                    command.Parameters.AddWithValue("@Location", job.Location);
                    command.Parameters.AddWithValue("@Salary", job.Salary);
                    command.Parameters.AddWithValue("@CompanyID", job.CompanyID);
                    command.Parameters.AddWithValue("@CategoryID", job.CategoryID);

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
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "jobs/{id}")] HttpRequestData req,
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

                string query = "UPDATE Jobs SET JobTitle = @JobTitle, Description = @Description, Location = @Location, Salary = @Salary, CompanyID = @CompanyID, CategoryID = @CategoryID WHERE JobID = @JobID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@JobID", id);
                    command.Parameters.AddWithValue("@JobTitle", job.JobTitle);
                    command.Parameters.AddWithValue("@Description", job.Description);
                    command.Parameters.AddWithValue("@Location", job.Location);
                    command.Parameters.AddWithValue("@Salary", job.Salary);
                    command.Parameters.AddWithValue("@CompanyID", job.CompanyID);
                    command.Parameters.AddWithValue("@CategoryID", job.CategoryID);

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
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "jobs/{id}")] HttpRequestData req,
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