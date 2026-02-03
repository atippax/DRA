public class Truck
{
    private string truckId;
    private Dictionary<string, int> avaliableResources;
    private Dictionary<string, int> travelTimeToArea;
    public Truck(string truckId, Dictionary<string, int> avaliableResources, Dictionary<string, int> travelTimeToArea)
    {
        this.truckId = truckId;
        this.avaliableResources = avaliableResources;
        this.travelTimeToArea = travelTimeToArea;
    }
    public override string ToString()
    {
        var lines1 = this.avaliableResources.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
        var lines2 = this.travelTimeToArea.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());

        var resource = string.Join(",", lines1);
        var travels = string.Join(",", lines2);
        return $"truck id  :{this.truckId} have {resource} can go {travels}\n";
    }
    public string getId() => this.truckId;
    public Dictionary<string, int> getAvaliableResources() => this.avaliableResources;
    public Dictionary<string, int> getTravelTimeToArea() => this.travelTimeToArea;

}