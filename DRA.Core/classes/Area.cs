public class Area
{
    private string areaId;
    private int urgencyLevel;
    private Dictionary<string, int> requiredResources;
    private int timeConstraint;
    public Area(string areaId, int urgencyLevel, Dictionary<string, int> requiredResources, int timeConstraint)
    {
        this.areaId = areaId;
        this.urgencyLevel = urgencyLevel;
        this.requiredResources = requiredResources;
        this.timeConstraint = timeConstraint;
    }
    public override string ToString()
    {
        var lines = this.requiredResources.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
        var resource = string.Join(",", lines);
        return $"area id  :{this.areaId}[urgency {this.urgencyLevel}] need {resource} can wait {this.timeConstraint}\n";
    }
    public string getId() => this.areaId;
    public int getUrgencyLevel() => this.urgencyLevel;
    public int getTimeConstraint() => this.timeConstraint;
    public Dictionary<string, int> getRequiredResources() => this.requiredResources;

}