using System.Threading.Tasks;

[Route("[controller]")]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private readonly AppContext context;
    private readonly IResourceManagement resourceManagement;

    public AssignmentsController(AppContext _context, IResourceManagement resourceManagement)
    {
        this.context = _context;
        this.resourceManagement = resourceManagement;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> index()
    {
        return "return redis cache";
    }


    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> delete()
    {
        return "remove cache redis db";
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ResourceManagementResult[]>> assignment()
    {
        var areas = await context.areas.ToListAsync();
        var trucks = await context.trucks.ToListAsync();
        var result = resourceManagement.computedResource(
            areas.Select(area => new Area
            (
                "A" + area.id,
                area.urgencyLevel,
                area.requiredResources,
                area.timeConstraint
            )).ToList(),

            trucks.Select(truck => new Truck
            (
                "T" + truck.id,
                truck.resources,
                truck.timeToTravel
            )).ToList());
        var successCase = result.Where(x => x.successedCase != null).Select(x => x.successedCase!).ToArray();
        var truckUsed = successCase.Select(x => new TruckModel
        {
            canUse = false,
            id = Int32.Parse(x.truckDelivered.getId().Replace("T", "")),
            resources = x.truckDelivered.getAvaliableResources(),
            timeToTravel = x.truckDelivered.getTravelTimeToArea(),
        }).ToList();
        var areaDelivered = successCase.Select(x => new AreaModel
        {
            hasDelivered = true,
            id = Int32.Parse(x.areaDelivered.getId().Replace("A", "")),
            requiredResources = x.areaDelivered.getRequiredResources(),
            timeConstraint = x.areaDelivered.getTimeConstraint(),
            urgencyLevel = x.areaDelivered.getUrgencyLevel(),
        }).ToArray();
        var resourceMapped = successCase.Select(x => new AssignmentModel
        {
            areaId = Int32.Parse(x.mapped.areaId.Replace("A", "")),
            resources = x.mapped.resourcesDelivered,
            truckId = Int32.Parse(x.mapped.truckId.Replace("T", "")),
        }).ToArray();
        context.trucks.UpdateRange(truckUsed);
        context.areas.UpdateRange(areaDelivered);
        context.assignments.AddRange(resourceMapped);
        return Ok(successCase.Select(x => x.mapped));
    }
}
