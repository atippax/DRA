[Route("[controller]")]
[ApiController]
public class AreaController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<ResourceManagementResult[]> create()
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
