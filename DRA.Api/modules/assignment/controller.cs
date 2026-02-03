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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ResourceManagementResult[]> assignment()
    {
        var areas = new List<Area>();
        var trucks = new List<Truck>();
        var management = new ResourceManagement();
        var result = management.computedResource(areas, trucks);
        var successCase = result.Where(x => x.successedCase != null).Select(x => x.successedCase!).ToArray();
        var truckUsed = successCase.Select(x => x.truckDelivered).ToArray();
        var areaDelivered = successCase.Select(x => x.areaDelivered).ToArray();
        var resourceMapped = successCase.Select(x => x.mapped).ToArray();
        return result;
    }
}
