using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

[Route("[controller]")]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private readonly AppContext context;
    private readonly IResourceManagement resourceManagement;
    private readonly IDatabase redis;

    public AssignmentsController(AppContext _context, IResourceManagement resourceManagement, IConnectionMultiplexer _connectionMultiplexer)
    {
        this.context = _context;
        this.redis = _connectionMultiplexer.GetDatabase();
        this.resourceManagement = resourceManagement;

    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<ResourceMapper[]> index()
    {
        var assignmented = redis.ListRange("assignment");
        var result = assignmented.Select(s => JsonConvert.DeserializeObject<ResourceMapper>(s)).ToArray();
        return result ?? [];
    }


    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> delete()
    {
        return redis.KeyDelete("assignment");
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
        var resultToPreview = successCase.Select(x => x.mapped).ToList();
        redis.KeyDelete("assignment");
        resultToPreview.ForEach(assign =>
        {
            redis.ListRightPush("assignment", JsonConvert.SerializeObject(assign));
        });
        redis.KeyExpire("assignment", new TimeSpan(0, 30, 0));
        return Ok(resultToPreview);
    }
}
