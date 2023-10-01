namespace CodingTest.DepthCharts.Models;

public class GetDepthChartResponse
{
    public class Position
    {
        public string PositionId { get; set; }
        public IEnumerable<int> Players { get; set; }
    }

    public IEnumerable<Position> Positions { get; set; }
}
