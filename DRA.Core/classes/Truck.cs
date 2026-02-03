public class Truck
{
    private string truckId;
    private Dictionary<string,int> avaliableResources;
    private Dictionary<string,int> travelTimeToArea;
    public Truck(string truckId,Dictionary<string,int> avaliableResources,Dictionary<string,int> travelTimeToArea)
    {
        this.truckId = truckId;
        this.avaliableResources= avaliableResources;
        this.travelTimeToArea = travelTimeToArea;
    }
    public string getId()=>this.truckId;
    public Dictionary<string,int>  getAvaliableResources()=>this.avaliableResources;
    public Dictionary<string,int>  getTravelTimeToArea()=>this.travelTimeToArea;

}