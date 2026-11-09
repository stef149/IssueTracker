using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IssueTrackerService _service;

        public ProjectsController(IssueTrackerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectResponse>>> GetAll()
        {
            return Ok(await _service.GetProjectsAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectResponse>> GetById(int id)
        {
            try
            {
                return Ok(await _service.GetProjectByIdAsync(id));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectRequest request)
        {
            try
            {
                var project = await _service.CreateProjectAsync(request.Name, request.Description);
                return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}/issues")]
        public async Task<ActionResult<List<IssueResponse>>> GetIssues(int id, [FromQuery] IssueFilterQuery filter)
        {
            try
            {
                return Ok(await _service.GetIssuesForProjectAsync(id, filter.Status, filter.Priority, filter.AssignedUserId));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/issues")]
        public async Task<ActionResult<IssueResponse>> CreateIssue(int id, [FromBody] CreateIssueRequest request)
        {
            try
            {
                var issue = await _service.CreateIssueAsync(id, request.CreatedByUserId, request.Title,
                                                            request.Description, request.Priority!.Value, request.DueDate!.Value);

                return CreatedAtAction(nameof(IssuesController.GetById), "Issues", new { id = issue.Id }, issue);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/members")]
        public async Task<IActionResult> AddMember(int id, [FromBody] AddProjectMemberRequest request)
        {
            try
            {
                await _service.AddMemberToProjectAsync(id, request.UserId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
