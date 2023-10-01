using AutoMapper;

namespace CodingTest.DepthCharts.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // models to messages
        CreateMap<Models.AddPlayerRequest, Messages.AddPlayerCommand>();
        CreateMap<Models.AddPlayerRequest.PlayerObj, Messages.AddPlayerCommand.PlayerObj>();
        CreateMap<Models.GetTrailingPlayersRequest, Messages.GetTrailingPlayersQuery>();

        // messages to models
        CreateMap<Messages.GetDepthChartResponse, Models.GetDepthChartResponse>();
        CreateMap<Messages.GetDepthChartResponse.Position, Models.GetDepthChartResponse.Position>();
        CreateMap<Messages.GetTrailingPlayersResponse, Models.GetTrailingPlayersResponse>();

        // messages to entities
        CreateMap<Messages.AddPlayerCommand.PlayerObj, Entities.Player>();
    }
}