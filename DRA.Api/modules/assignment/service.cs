using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using StackExchange.Redis;

public class AssignmentService
{
    private readonly AppContext context;
    private readonly IResourceManagement resourceManagement;
    private readonly IDatabase redis;
    private readonly string redisAssignmentKey = "assignment";
    private readonly TrucksService trucksService;
    private readonly AreasService areasService;
    public AssignmentService(AppContext _context, IResourceManagement resourceManagement, IConnectionMultiplexer _connectionMultiplexer, TrucksService trucksService, AreasService areasService)
    {
        this.context = _context;
        this.trucksService = trucksService;
        this.redis = _connectionMultiplexer.GetDatabase();
        this.resourceManagement = resourceManagement;
        this.areasService =areasService;

    }
    public async Task deleteAssigmnents()
    {
        var assignments = await context.assignments.ToListAsync();
        if (assignments != null)
        {
            context.RemoveRange(assignments);
            await context.SaveChangesAsync();
        }
        await redis.KeyDeleteAsync(redisAssignmentKey);
    }
    public async Task<bool> deleteAssignmentInRedis()
    {
        return await redis.KeyDeleteAsync(redisAssignmentKey);
    }
    public async Task<IEnumerable<ResourceMapper>> getAssignmentInRedis()
    {
        var assignmented = await redis.ListRangeAsync(redisAssignmentKey);
        if (assignmented == null) return [];
        var result = assignmented.Select(s => JsonConvert.DeserializeObject<ResourceMapper>(s!)!).ToArray();
        return result;
    }

    private async Task updateTrucks(ResultAfterMap[] successCase)
    {
        var truckUsedIds = successCase.Select(x => Int32.Parse(x.truckDelivered.getId().Replace("T", ""))).ToList();
        var truckEntities = await context.trucks.Where(x => truckUsedIds.Any(f => f == x.id)).ToListAsync();
        truckEntities.ForEach(x => x.canUse = false);
        await trucksService.update(truckEntities);
    }

    private async Task updateAreas(ResultAfterMap[] successCase)
    {
        var areaDeliveredIds = successCase.Select(x => Int32.Parse(x.areaDelivered.getId().Replace("A", ""))).ToArray();
        var areaEntities = await context.areas.Where(x => areaDeliveredIds.Any(f => f == x.id)).ToListAsync();
        areaEntities.ForEach(x => x.hasDelivered = true);
        await  areasService.update(areaEntities);
    }
    public async Task createAssignment(AssignmentModel[] assignment)
    {
        context.assignments.AddRange(assignment);
        await context.SaveChangesAsync();
    }
    private async Task saveResultMappedToAssignment(ResultAfterMap[] successCase)
    {
        var resourceMapped = successCase.Select(x => new AssignmentModel
        {
            areaId = Int32.Parse(x.mapped.areaId.Replace("A", "")),
            resourcesDelivered = x.mapped.resourcesDelivered,
            truckId = Int32.Parse(x.mapped.truckId.Replace("T", "")),
        }).ToArray();
        await createAssignment(resourceMapped);
    }

    private async Task updateRedis(ResultAfterMap[] successCase)
    {
        var resultToPreview = successCase.Select(x => x.mapped).ToList();
        var existRedis = await redis.ListRangeAsync(redisAssignmentKey);
        redis.KeyDelete(redisAssignmentKey);
        existRedis.ToList().ForEach(x =>
        {
            redis.ListRightPush(redisAssignmentKey, x);
        });
        resultToPreview.ForEach(assign =>
        {
            redis.ListRightPush(redisAssignmentKey, JsonConvert.SerializeObject(assign));
        });
        redis.KeyExpire(redisAssignmentKey, new TimeSpan(0, 30, 0));
    }

    public async Task<IEnumerable<ResourceManagementResult>> assignmentToArea()
    {
        var areas = await context.areas.Where(x => !x.hasDelivered).ToListAsync();
        var trucks = await trucksService.getAllTrucks();
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
        if (successCase.Count() == 0) return result;
        await updateTrucks(successCase);
        await updateAreas(successCase);
        await saveResultMappedToAssignment(successCase);
        await updateRedis(successCase);
        return result;
    }
}
