using AutoMapper;

namespace Lumiere.Tests.Unit.Helpers;

public class MapperFactory
{
    public static IMapper Create()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(Lumiere.Application.Services.ArticleService).Assembly);
        });
        return config.CreateMapper();
    }

}
