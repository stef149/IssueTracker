using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IssueTrackerService _service;
        public UsersController(IssueTrackerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            return Ok(await _service.GetUsersAsync());
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
        {
            try
            {
                var user = await _service.AddUserAsync(request.Name, request.Email, request.Role!.Value);
                return StatusCode(StatusCodes.Status201Created, user);
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}/issues")]
        public async Task<ActionResult<List<IssueResponse>>> GetIssues(int id)
        {
            try
            {
                return Ok(await _service.GetIssuesForUserAsync(id));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
