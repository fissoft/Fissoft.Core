using AutoMapper;

namespace Fissoft.Framework
{
    public class MapperUtil
    {
        public static IMapper Mapper => Configuration.CreateMapper();
        public static MapperConfiguration Configuration { get; set; }

        public static TDestination Map<TDestination>(object source)
        {
            return Configuration.CreateMapper().Map<TDestination>(source);
        }

        public static void Config(MapperConfiguration config)
        {
            Configuration = config;
        }

        
    }
}
