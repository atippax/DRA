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
    public ActionResult<TruckItem[]> index()
    {
        var item = context.truckItems.Where(x => true).ToArray();
        return item;
    }
}
