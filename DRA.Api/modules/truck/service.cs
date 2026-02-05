
public class TrucksService
{
    private readonly AppContext context;

    public TrucksService(AppContext _context)
    {
        this.context = _context;
    }


    public async Task update(IEnumerable<TruckModel> truckEntities)
    {
        context.trucks.UpdateRange(truckEntities);
        await context.SaveChangesAsync();
    }
    public async Task deleteAllTrucks()
    {
        var trucks = await context.trucks.ToListAsync();
        if (trucks != null)
        {
            context.RemoveRange(trucks);
            await context.SaveChangesAsync();
        }
    }
    public async Task<TruckModel[]> getAllTrucks()
    {
        var item = await context.trucks.Where(x => x.canUse).ToArrayAsync();
        return item;
    }

    public async Task<TruckModel> create(CreateTruckBody model)
    {
        var item = new TruckModel
        {
            canUse = true,
            resources = model.resources,
            timeToTravel = model.timeToTravel,
        };
        context.trucks.Add(item);
        await context.SaveChangesAsync();
        return item;
    }
}
