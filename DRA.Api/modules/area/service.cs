public class AreasService
{
    private readonly AppContext context;

    public AreasService(AppContext _context)
    {
        this.context = _context;
    }

    public async Task deleteAllAreas()
    {
        var areas = await context.areas.ToListAsync();
        if (areas != null)
        {
            context.RemoveRange(areas);
            await context.SaveChangesAsync();
        }
    }
    public async Task<AreaModel[]> getAllAreas()
    {
        var item = await context.areas.Where(x => !x.hasDelivered).ToArrayAsync();
        return item;
    }
    async public Task<AreaModel> create(CreateAreaBody model)
    {
        if (model.urgencyLevel < 1 || model.urgencyLevel > 5)
            throw new Exception("urgency level not in range");
        var item = new AreaModel
        {
            hasDelivered = false,
            requiredResources = model.requiredResource,
            timeConstraint = model.timeConstraint,
            urgencyLevel = model.urgencyLevel,
        };
        context.areas.Add(item);
        await context.SaveChangesAsync();
        return item;
    }
}
