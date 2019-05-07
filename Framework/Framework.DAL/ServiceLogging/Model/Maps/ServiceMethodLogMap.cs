namespace Framework.Data.ServiceLogging.Model.Maps
{
    using DapperExtensions.Mapper;

    public class ServiceMethodLogMap : ClassMapper<ServiceMethodLog>
    {
        public ServiceMethodLogMap()
        {
            this.AutoMap();
        }
    }
}