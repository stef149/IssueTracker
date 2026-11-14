using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Api.Controllers
{
    [ApiController]
    [Route("api/issues")]
    public class IssuesController : ControllerBase
    {
        private readonly IssueTrackerService _service;

        public IssuesController(IssueTrackerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<IssueResponse>>> GetAll([FromQuery] IssueQuery filter)
        {
            return Ok(await _service.GetIssuesAsync(filter));
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<List<IssueResponse>>> GetOverdue()
        {
            return Ok(await _service.GetOverdueIssuesAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IssueResponse>> GetById(int id)
        {
            try
            {
                return Ok(await _service.GetIssueByIdAsync(id));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/assign")]
        public async Task<ActionResult<IssueResponse>> Assign(int id, [FromBody] AssignIssueRequest request)
        {
            try
            {
                return Ok(await _service.AssignIssueAsync(id, request.UserId));
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

        [HttpPut("{id:int}/status")]
        public async Task<ActionResult<IssueResponse>> ChangeStatus(int id, [FromBody] ChangeIssueStatusRequest request)
        {
            try
            {
                return Ok(await _service.ChangeIssueStatusAsync(id, request.Status!.Value, request.ChangedByUserId));
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

        [HttpPost("{id:int}/comments")]
        public async Task<ActionResult<CommentResponse>> AddComment(int id, [FromBody] AddCommentRequest request)
        {
            try
            {
                var comment = await _service.AddCommentAsync(id, request.UserId, request.Text);
                return CreatedAtAction(nameof(GetById), new { id }, comment);
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
