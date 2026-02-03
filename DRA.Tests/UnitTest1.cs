namespace DRA.Tests;

public class UnitTest1
{
    [Fact]
    public void TestComputed()
    {
        var resourcesForA1 = new Dictionary<string, int>
            {
                {"food",200},
                {"water",300},
            };
        var resourcesForA2 = new Dictionary<string, int>
            {
                {"medicine",50},
            };
        var resourcesForT1 = new Dictionary<string, int>
            {
                {"food",250},
                {"water",300},
            };
        var timeToAreaT1 = new Dictionary<string, int>
        {
          {"A1",5},
          {"A2",3}
        };
        var resourcesForT2 = new Dictionary<string, int>
            {
                {"medicine",60}
            };
        var timeToAreaT2 = new Dictionary<string, int>
             {
          {"A1",4},
          {"A2",2}
        };
        var dra_mn = new ResourceManagement();
        var expectedResult = new ResourceMapper[]
        {
            new ResourceMapper{areaId="A1",truckId="T1",resourcesDelivered=resourcesForA1},
            new ResourceMapper{areaId="A2",truckId="T2",resourcesDelivered=resourcesForA2},
        };
        var areas = new List<Area>
        {
            new Area("A1",5, resourcesForA1, 6),
            new Area("A2",4, resourcesForA2, 4),
        };
        var trucks = new List<Truck>
        {
            new Truck("T1",resourcesForT1,timeToAreaT1),
            new Truck("T2",resourcesForT2,timeToAreaT2),
        };
        var result = dra_mn.computedResource(areas, trucks);
        Assert.Equivalent(expectedResult, result.Where(x => x.successedCase != null).Select(x => x.successedCase!.mapped).ToArray());
    }

}