

using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Prng;
using TruckApi.Models;

[Route("[controller]")]
[ApiController]
public class TruckController : ControllerBase
{
    private readonly AppContext context;

    public TruckController(AppContext _context)
    {
        this.context = _context;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<TruckModel[]> index()
    {
        var item = context.truckItems.Where(x => true).ToArray();
        return item;
    }

    [HttpGet("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TruckModel>> create()
    {
        var item = new TruckModel
        {
            canUse = true,
            resources = new Dictionary<string, int>
            {
                {
                    "medicine",40
                }
            },
            timeToTravel = new Dictionary<string, int>
            {
                {
                    "A3",40
                }
            },

        };
        context.truckItems.Add(item);
        await context.SaveChangesAsync();
        await context.Entry(item).ReloadAsync();
        return Ok(item);
    }
}