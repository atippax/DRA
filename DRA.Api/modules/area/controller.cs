[Route("[controller]")]
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
        return Ok();
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    async public Task<ActionResult<AreaModel>> create([FromBody] CreateAreaBody model)
    {
        return Ok(await areasService.create(model));
    }
}
