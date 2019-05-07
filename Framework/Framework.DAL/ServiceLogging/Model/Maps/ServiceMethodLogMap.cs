using DapperExtensions.Mapper;

namespace Framework.Data.ServiceLogging.Model.Maps
{
    public class ServiceMethodLogMap : ClassMapper<ServiceMethodLog>
    {
        public ServiceMethodLogMap()
        {
            AutoMap();
        }
    }
}