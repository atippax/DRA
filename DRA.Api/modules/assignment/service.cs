using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using StackExchange.Redis;

public class AssignmentService
{
    private readonly AppContext context;
    private readonly IResourceManagement resourceManagement;
    private readonly IDatabase redis;
    private readonly string redisAssignmentKey = "assignment";
    public AssignmentService(AppContext _context, IResourceManagement resourceManagement, IConnectionMultiplexer _connectionMultiplexer)
    {
        this.context = _context;
        this.redis = _connectionMultiplexer.GetDatabase();
        this.resourceManagement = resourceManagement;

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
    public async Task<IEnumerable<ResourceManagementResult>> assignmentToArea()
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
        if (successCase.Count() == 0)
        {
            return result;
        }
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
        await context.SaveChangesAsync();


        var areaEntities = await context.areas.Where(x => areaDeliveredIds.Any(f => f == x.id)).ToListAsync();
        areaEntities.ForEach(x => x.hasDelivered = true);
        context.areas.UpdateRange(areaEntities);
        await context.SaveChangesAsync();

        context.assignments.AddRange(resourceMapped);
        var resultToPreview = successCase.Select(x => x.mapped).ToList();
        redis.KeyDelete(redisAssignmentKey);
        resultToPreview.ForEach(assign =>
        {
            redis.ListRightPush(redisAssignmentKey, JsonConvert.SerializeObject(assign));
        });
        redis.KeyExpire(redisAssignmentKey, new TimeSpan(0, 30, 0));
        return result;
    }
}