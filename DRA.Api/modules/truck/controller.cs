[Route("[controller]")]
[ApiController]
public class TrucksController : ControllerBase
{
    private readonly AppContext context;

    public TrucksController(AppContext _context)
    {
        this.context = _context;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<TruckModel[]> index()
    {
        var item = context.trucks.Where(x => true).ToArray();
        return item;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TruckModel>> create([FromBody] CreateTruckBody model)
    {
        var item = new TruckModel
        {
            canUse = true,
            resources = model.resources,
            timeToTravel = model.timeToTravel,
        };
        context.trucks.Add(item);
        await context.SaveChangesAsync();
        return Ok(item);
    }
}