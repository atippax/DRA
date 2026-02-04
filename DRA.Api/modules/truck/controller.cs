using System.Threading.Tasks;

[Route("[controller]")]
[ApiController]
public class TrucksController : ControllerBase
{
    private readonly TrucksService trucksService;

    public TrucksController(TrucksService trucksService)
    {
        this.trucksService = trucksService;
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> delete()
    {
        await trucksService.deleteAllTrucks();
        return Ok();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TruckModel[]>> index()
    {
        var item = await trucksService.getAllTrucks();
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TruckModel>> create([FromBody] CreateTruckBody model)
    {
        return Ok(await trucksService.create(model));
    }
}