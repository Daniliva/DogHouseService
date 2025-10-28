namespace DogHouse.Middleware
{
    public class RateLimitingOptions
    {
        public int RequestsPerSecond { get; set; } = 10;
    }
}