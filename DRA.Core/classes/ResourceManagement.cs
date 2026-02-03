public class ResourceMapper
{
    public string areaId;
    public string truckId;
    public Dictionary<string, int> resourcesDelivered;
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
    public ResourceMapper mapped;
    public List<Truck> remainTrucks;
    public Area areaDelivered;
    public Truck truckDelivered;
}
public class ResourceManagementResult
{
    public ResultAfterMap? successedCase;
    public ErrorCase? errorCase;

}
public class ErrorCase
{
    public string areaId;
    public string reason;
}
public class ResourceManagement
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
        var _areas = new List<Area>(areas);
        var sorter = new SortArea();
        _areas.Sort(sorter);
        var resourcesMapped = this.computed(_areas, trucks);
        if (resourcesMapped.Length == 0 && _areas.Count() != 0)
            throw new Exception("Insufficient resources for any area.");
        return resourcesMapped;
    }
}