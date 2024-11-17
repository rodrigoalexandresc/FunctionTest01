using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace RscMic01
{
    public static class HttpUtils
    {
        public async static Task<T?> BodyToObject<T>(HttpRequest req) where T : class
        {
            using (var reader = new StreamReader(req.Body))
            {
                var body = await reader.ReadToEndAsync();
                return JsonSerializer.Deserialize<T>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
            }

        }
    }
}
