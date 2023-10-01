using AutoMapper;
using CodingTest.DepthCharts.Mappers;

namespace CodingTest.DepthCharts.Tests.Mappers;

public class MapperTests
{
    [Fact]
    public void Mapper_Configuration_Is_Valid()
    {
        // arrange
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MapperProfile());
        });

        // act and assert
        mappingConfig.AssertConfigurationIsValid();
    }
}

