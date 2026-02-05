using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

[Route("api/[controller]")]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private readonly AssignmentService assignmentService;

    public AssignmentsController(AssignmentService assignmentService)
    {
        this.assignmentService = assignmentService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ResourceMapper[]>> index()
    {
        return Ok(await assignmentService.getAssignmentInRedis());
    }

    [HttpDelete("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> reset()
    {
        await assignmentService.deleteAssigmnents();
        return Ok(true);
    }


    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> delete()
    {
        return Ok(assignmentService.deleteAssignmentInRedis());
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ResourceManagementResult[]>> assignment()
    {
        return Ok(await assignmentService.assignmentToArea());
    }
}
