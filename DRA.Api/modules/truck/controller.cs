[Route("[controller]")]
[ApiController]
public class TruckController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> index()
    {
        return "hi";
    }
}
