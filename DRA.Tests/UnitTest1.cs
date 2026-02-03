namespace DRA.Tests;

public class UnitTest1
{
    [Fact]
    public void TestCreateTruck()
    {
        var resources = new Dictionary<string, int>
            {
                {"water",50}
            };
        var areas = new Dictionary<string, int>
            {
                {"A1",4}
            };
        var dra_mn = new ResourceManagement();
        dra_mn.AddTrack(resources, areas);
        Assert.Equal("T1", dra_mn.allTracks().FirstOrDefault().getId());
        Assert.Equivalent(resources, dra_mn.allTracks().FirstOrDefault().getAvaliableResources());
        Assert.Equivalent(areas, dra_mn.allTracks().FirstOrDefault().getTravelTimeToArea());
    }

    [Fact]
    public void TestCreateArea()
    {
        var resources = new Dictionary<string, int>
            {
                {"water",50}
            };

        var dra_mn = new ResourceManagement();
        dra_mn.AddArea(5, resources, 4);
        Assert.Equal("A1", dra_mn.allAreas().FirstOrDefault().getId());
        Assert.Equivalent(resources, dra_mn.allAreas().FirstOrDefault().getRequiredResources());
        Assert.Equivalent(4, dra_mn.allAreas().FirstOrDefault().getTimeConstraint());
        Assert.Equivalent(5, dra_mn.allAreas().FirstOrDefault().getUrgencyLevel());
    }



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
        dra_mn.AddTrack(resourcesForT1, timeToAreaT1);
        dra_mn.AddTrack(resourcesForT2, timeToAreaT2);
        dra_mn.AddArea(5, resourcesForA1, 6);
        dra_mn.AddArea(4, resourcesForA2, 4);
        var expectedResult = new ResourceMapper[]
        {
            new ResourceMapper("A1","T1",resourcesForA1),
            new ResourceMapper("A2","T2",resourcesForT2),
        };
        Console.WriteLine(string.Join("\n", dra_mn.computedResource().Select(x => x.ToString())).ToList());
        Assert.Equivalent(expectedResult, dra_mn.computedResource());
    }

}