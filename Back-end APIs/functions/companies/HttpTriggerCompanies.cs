using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;

namespace Company.Function
{
    public class Company
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
    }
    public class HttpTriggerCompanies
    {
        private readonly ILogger _logger;

        public HttpTriggerCompanies(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTriggerCompanies>();
        }

        [Function("GetCompanies")]
        //[HttpTrigger(AuthorizationLevel.Function, "get", Route = "companies")]
        public async Task<HttpResponseData> GetCompanies([HttpTrigger(AuthorizationLevel.Function, "get", Route = "companies")] HttpRequestData req)
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
                                Location = reader.GetString(2)
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
        //[HttpTrigger(AuthorizationLevel.Function, "get", Route = "companies/{id}")]
        public async Task<HttpResponseData> GetCompanyById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "companies/{id}")] HttpRequestData req, int id)
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
                                Location = reader.GetString(2)
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
        //[HttpTrigger(AuthorizationLevel.Function, "post", Route = "companies")]
        public async Task<HttpResponseData> CreateCompany([HttpTrigger(AuthorizationLevel.Function, "post", Route = "companies")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request to create a new company.");

            string requestBody = await req.ReadAsStringAsync();
            Company company = JsonSerializer.Deserialize<Company>(requestBody);

            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Companies (CompanyID, CompanyName, Location) VALUES (@CompanyID, @CompanyName, @Location)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CompanyID", company.CompanyID);
                    command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                    command.Parameters.AddWithValue("@Location", company.Location);

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
        //[HttpTrigger(AuthorizationLevel.Function, "put", Route = "companies/{id}")]
        public async Task<HttpResponseData> UpdateCompany([HttpTrigger(AuthorizationLevel.Function, "put", Route = "companies/{id}")] HttpRequestData req, int id)
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
        //[HttpTrigger(AuthorizationLevel.Function, "delete", Route = "companies/{id}")]
        public async Task<HttpResponseData> DeleteCompany([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "companies/{id}")] HttpRequestData req, int id)
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
    }
}