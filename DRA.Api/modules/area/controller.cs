[Route("api/[controller]")]
[ApiController]
public class AreasController : ControllerBase
{
    private readonly AreasService areasService;

    public AreasController(AreasService areasService)
    {
        this.areasService = areasService;
    }
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> delete()
    {
        await areasService.deleteAllAreas();
        return Ok(true);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AreaModel[]>> index()
    {
        var item = await areasService.getAllAreas();
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    async public Task<ActionResult<AreaModel>> create([FromBody] CreateAreaBody model)
    {
        return Ok(await areasService.create(model));
    }
}
