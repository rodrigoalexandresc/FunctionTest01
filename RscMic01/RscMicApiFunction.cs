using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace RscMic01
{
    public class RscMicApiFunction
    {
        private readonly ILogger<RscMicApiFunction> _logger;

        public RscMicApiFunction(ILogger<RscMicApiFunction> logger)
        {
            _logger = logger;
        }

        [Function("Rscmc")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            if (req.Method == "GET")
                return await DailyGet(req);

            if (req.Method == "POST")
                return await DailyPost(req);

            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions! RSC Mic");
        }

        private async Task<IActionResult> DailyPost(HttpRequest req)
        {
            var cmd = await HttpUtils.BodyToObject<DailyRegCreateCommand>(req);
            if (cmd == null) return new BadRequestObjectResult("Invalid data");
         
            var entity = new DailyReg
            {
                Date = cmd.Date ?? DateTime.Now,
                Volume = cmd.Volume,
                Leakage = cmd.Leakage,
            };

            using (var conn = new SqlConnection(Utils.GetConnectionString()))
            {
                await conn.OpenAsync();
                await conn.ExecuteAsync("INSERT INTO Daily (Date, Volume, Leakage) VALUES (@Date, @Volume, @Leakage)", entity);
            }

            return new OkObjectResult("Entity created sucessefully");
        }

        private async Task<IActionResult> DailyGet(HttpRequest req)
        {
            int.TryParse(req.Query["page"].ToString(), out int page);
            int.TryParse(req.Query["pageSize"].ToString(), out int pageSize);

            using (var conn = new SqlConnection(Utils.GetConnectionString()))
            {
                await conn.OpenAsync();
                var result = await conn.QueryAsync<DailyReg>($"SELECT * FROM Daily d ORDER BY Date OFFSET {page} ROWS FETCH NEXT {pageSize} ROWS ONLY ");

                return new OkObjectResult(result);
            }
        }
    }
}
