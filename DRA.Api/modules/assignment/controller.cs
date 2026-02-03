[Route("[controller]")]
[ApiController]
public class AssignmentController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> index()
    {
        return "hi";
    }

    [HttpGet("test")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ResourceManagementResult[]> assignment()
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
        var management = new ResourceManagement();
        var result = management.computedResource(areas, trucks);
        var successCase = result.Where(x => x.successedCase != null).Select(x => x.successedCase!).ToArray();
        var truckUsed = successCase.Select(x => x.truckDelivered).ToArray();
        var areaDelivered = successCase.Select(x => x.areaDelivered).ToArray();
        var resourceMapped = successCase.Select(x => x.mapped).ToArray();
        return Ok(result);
    }
}
