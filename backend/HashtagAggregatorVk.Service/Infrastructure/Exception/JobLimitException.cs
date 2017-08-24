namespace HashtagAggregatorVk.Service.Infrastructure.Exception
{
    public class JobLimitException : System.Exception
    {
        public JobLimitException()
        {
        }

        public JobLimitException(string message)
            : base(message)
        {
        }

        public JobLimitException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}