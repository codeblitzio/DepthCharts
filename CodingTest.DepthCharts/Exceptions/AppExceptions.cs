namespace CodingTest.DepthCharts.Exceptions
{
    public abstract class AppException : Exception
    {
        public AppException(string message) : base(message) { }
    }

    public class RepositoryException : AppException
    {
        public RepositoryException(string message) : base(message) { }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message) { }
    }
}
