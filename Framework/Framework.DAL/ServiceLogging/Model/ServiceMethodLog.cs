namespace Framework.Data.ServiceLogging.Model
{
    public class ServiceMethodLog
    {
        public int Id { get; set; }

        public string Service { get; set; }

        public string Method { get; set; }

        public string RequestMessage { get; set; }

        public string ResponseMessage { get; set; }

        public long ProcessingTime { get; set; }
    }
}