[Route("[controller]")]
[ApiController]
public class AreasController : ControllerBase
{
    private readonly AppContext context;

    public AreasController(AppContext _context)
    {
        this.context = _context;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    async public Task<ActionResult<Area>> create([FromBody] CreateAreaBody model)
    {
        var item = new AreaModel
        {
            hasDelivered = false,
            requiredResources = model.requiredResource,
            timeConstraint = model.timeConstraint,
            urgencyLevel = model.urgencyLevel,
        };
        context.areas.Add(item);
        await context.SaveChangesAsync();
        return Ok(item);
    }
}
