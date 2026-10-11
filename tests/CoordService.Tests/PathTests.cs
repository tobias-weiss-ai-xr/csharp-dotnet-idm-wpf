using CoordService;
using Xunit;

public class PathTests
{
    [Fact] public void Dist_Correct() => Assert.Equal(5.0, PathOptimizer.Dist(new(0,0), new(3,4)), 1);
    
    [Fact]
    public void GreedyNn_Returns_Valid_Path()
    {
        var pts = new[] { new CoordPoint(0,0), new CoordPoint(1,3), new CoordPoint(4,2), new CoordPoint(3,5) };
        var path = PathOptimizer.GreedyNn(pts);
        Assert.Equal(4, path.Length); Assert.Equal(path.Distinct().Count(), path.Length);
    }
    
    [Fact] public void GreedyNn_Starts_At_Zero()
    {
        var pts = new[] { new CoordPoint(0,0), new CoordPoint(1,1), new CoordPoint(2,2) };
        var path = PathOptimizer.GreedyNn(pts); Assert.Equal(0, path[0]);
    }
    
    [Fact]
    public void AntColony_Returns_Valid_Path()
    {
        var pts = new[] { new CoordPoint(0,0), new CoordPoint(1,3), new CoordPoint(4,2), new CoordPoint(3,5) };
        var (order, dist) = AntColonyOptimizer.Optimize(pts, iterations: 10);
        Assert.Equal(4, order.Length); Assert.Equal(order.Distinct().Count(), order.Length);
        Assert.InRange(dist, 5.0, 20.0);
    }
    
    [Fact] public void AntColony_Two_Points_Distance()
    {
        var pts = new[] { new CoordPoint(0,0), new CoordPoint(3,4) };
        var (_, dist) = AntColonyOptimizer.Optimize(pts);
        Assert.Equal(5.0, dist, 1);
    }
}
