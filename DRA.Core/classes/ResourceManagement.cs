public class ResourceMapper
{
    public required string areaId { get; set; }
    public required string truckId { get; set; }
    public required Dictionary<string, int> resourcesDelivered { get; set; }
    public override string ToString()
    {
        var lines = this.resourcesDelivered.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
        var resource = string.Join(",", lines);
        return $"areaId : {this.areaId} truckId : ${this.truckId} resouce ${resource}";
    }
}
public class SortArea : IComparer<Area>
{
    public int Compare(Area x, Area y)
    {
        if (x.getUrgencyLevel() == y.getUrgencyLevel())
            return x.getTimeConstraint() - y.getTimeConstraint();
        return y.getUrgencyLevel() - x.getUrgencyLevel();
    }
}
public class ResultAfterMap
{
    public required ResourceMapper mapped { get; set; }
    public required List<Truck> remainTrucks { get; set; }
    public required Area areaDelivered { get; set; }
    public required Truck truckDelivered { get; set; }
}
public class ResourceManagementResult
{
    public ResultAfterMap? successedCase { get; set; }
    public ErrorCase? errorCase { get; set; }

}
public class ErrorCase
{
    public required string areaId { get; set; }
    public required string reason { get; set; }
}
public interface IResourceManagement
{
    public ResourceManagementResult[] computedResource(List<Area> areas, List<Truck> trucks);
}
public class ResourceManagement : IResourceManagement
{
    private ResultAfterMap resourceDelivery(Area area, List<Truck> trucks)
    {
        if (trucks.Count() == 0) throw new Exception("no trucks avaliable");
        var areaKey = area.getId();
        var truckResourceAvaliable = trucks.Where(truck => area.getRequiredResources().All((item) =>
            {
                truck.getAvaliableResources().TryGetValue(item.Key, out var trackItem);
                return trackItem >= item.Value;
            })
        );
        if (truckResourceAvaliable.Count() == 0) throw new Exception("No trucks available any resources");
        var truckCanDelivery = truckResourceAvaliable.Where(truck => area.getRequiredResources().All((item) =>
            {
                truck.getTravelTimeToArea().TryGetValue(areaKey, out var timeDelivery);
                return area.getTimeConstraint() >= timeDelivery;
            })
        );
        var truckUsage = truckCanDelivery.FirstOrDefault();
        if (truckUsage == null) throw new Exception("No trucks available to meet the time constraint");
        var resourceMapped = new ResourceMapper { areaId = areaKey, truckId = truckUsage.getId(), resourcesDelivered = area.getRequiredResources() };
        return new ResultAfterMap
        {
            mapped = resourceMapped,
            remainTrucks = trucks.Where(x => x != truckUsage).ToList(),
            areaDelivered = area,
            truckDelivered = truckUsage
        };
    }
    private ResourceManagementResult[] computed(List<Area> areas, List<Truck> trucks)
    {
        var areaNeedToDelivery = areas.FirstOrDefault();
        if (areaNeedToDelivery == null) return [];
        var remainAreas = areas.Skip(1).ToList();
        try
        {
            var resourceMapped = resourceDelivery(areaNeedToDelivery, trucks);
            if (resourceMapped == null) return [];
            return [
                new ResourceManagementResult { successedCase = resourceMapped, errorCase = null },
                 .. computed(remainAreas, resourceMapped.remainTrucks)
                 ];
        }
        catch (Exception ex)
        {
            var errorCase = new ErrorCase
            {
                areaId = areaNeedToDelivery.getId(),
                reason = ex.Message
            };
            return [
                new ResourceManagementResult { successedCase = null, errorCase=errorCase},
                 .. computed(remainAreas,trucks)
                 ];
        }
    }
    public ResourceManagementResult[] computedResource(List<Area> areas, List<Truck> trucks)
    {
        if(areas.Count()==0) throw new Exception("no any area to delivery!");
        if(trucks.Count()==0) throw new Exception("no any truck are ready to work!");
        var _areas = new List<Area>(areas);
        var sorter = new SortArea();
        _areas.Sort(sorter);
        var resourcesMapped = computed(_areas, trucks);
        if (resourcesMapped.Length == 0 && _areas.Count() != 0)
            throw new Exception("Insufficient resources for any area.");
        return resourcesMapped;
    }
}
