
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.Serialization;

public class ResourceMapper
{
    public string areaId;
    public string truckId;
    public Dictionary<string, int> resourcesDelivered;
    public ResourceMapper(string areaId, string truckId, Dictionary<string, int> resourcesDelivered)
    {
        this.areaId = areaId;
        this.truckId = truckId;
        this.resourcesDelivered = resourcesDelivered;
    }
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




public class ResourceManagement
{
    private List<Area> areas = new List<Area>();
    private List<Truck> trucks = new List<Truck>();
    public ResourceManagement() { }

    public List<Area> allAreas() => this.areas;
    public List<Truck> allTracks() => this.trucks;

    public void AddArea(int urgencyLevel, Dictionary<string, int> requiredResources, int timeConstraint)
    {
        areas.Add(new Area("A" + (areas.Count() + 1), urgencyLevel, requiredResources, timeConstraint));
    }
    public void AddTrack(Dictionary<string, int> avaliableResources, Dictionary<string, int> travelTimeToArea)
    {
        trucks.Add(new Truck("T" + (trucks.Count() + 1), avaliableResources, travelTimeToArea));
    }
    private ResourceMapper[] computed(List<Area> areas, List<Truck> trucks)
    {
        if (areas.Count() == 0 || trucks.Count() == 0) return [];
        var firstArea = areas.FirstOrDefault();
        if (firstArea == null) return [];
        var areaKey = firstArea.getId();
        var truckCanDelivery = trucks.Where(truck =>
        {
            var isCanDilivery = firstArea.getRequiredResources().All((item) =>
            {
                truck.getTravelTimeToArea().TryGetValue(areaKey, out var timeDelivery);
                truck.getAvaliableResources().TryGetValue(item.Key, out var trackItem);
                return trackItem >= item.Value && firstArea.getTimeConstraint() >= timeDelivery;
            });
            return isCanDilivery;
        });
        var truckUsage = truckCanDelivery.FirstOrDefault();
        if (truckUsage == null) return [];
        var resourceMapped = new ResourceMapper(areaKey, truckUsage.getId(), firstArea.getRequiredResources());
        return [resourceMapped, .. computed(areas.Skip(1).ToList(), trucks.Where(x => x != truckUsage).ToList())];
    }
    public ResourceMapper[] computedResource()
    {
        var sorter = new SortArea();
        this.areas.Sort(sorter);
        return this.computed(this.areas, this.trucks);
    }
}