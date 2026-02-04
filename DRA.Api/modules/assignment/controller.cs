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
        var areas = await context.areas.Where(x => !x.hasDelivered).ToListAsync();
        var trucks = await context.trucks.Where(x => x.canUse).ToListAsync();
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
        var truckUsedIds = successCase.Select(x => Int32.Parse(x.truckDelivered.getId().Replace("T", ""))).ToList();
        var areaDeliveredIds = successCase.Select(x => Int32.Parse(x.areaDelivered.getId().Replace("A", ""))).ToArray();
        var resourceMapped = successCase.Select(x => new AssignmentModel
        {
            areaId = Int32.Parse(x.mapped.areaId.Replace("A", "")),
            resources = x.mapped.resourcesDelivered,
            truckId = Int32.Parse(x.mapped.truckId.Replace("T", "")),
        }).ToArray();

        var truckEntities = await context.trucks.Where(x => truckUsedIds.Any(f => f == x.id)).ToListAsync();
        truckEntities.ForEach(x => x.canUse = false);
        context.trucks.UpdateRange(truckEntities);


        var areaEntities = await context.areas.Where(x => areaDeliveredIds.Any(f => f == x.id)).ToListAsync();
        areaEntities.ForEach(x => x.hasDelivered = true);
        context.areas.UpdateRange(areaEntities);

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
