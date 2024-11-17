namespace RscMic01
{
    public static class Utils
    {
        public static string GetConnectionString() => Environment.GetEnvironmentVariable("DbConnection", EnvironmentVariableTarget.Process);
    }
}
