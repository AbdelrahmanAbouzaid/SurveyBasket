using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Role;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.ReadRoles)]
        public async Task<IActionResult> GetAllRoles([FromQuery] bool includeDisable = false, CancellationToken cancellationToken = default)
        {
            var roles = await roleService.GetAllRolesAsync(includeDisable, cancellationToken);
            return Ok(roles);
        }

        [HttpGet("{roleId}")]
        [HasPermission(Permissions.ReadRoles)]
        public async Task<IActionResult> GetRoleById([FromRoute] string roleId, CancellationToken cancellationToken = default)
        {
            var result = await roleService.GetRoleByIdAsync(roleId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost]
        [HasPermission(Permissions.CreateRoles)]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequest request, CancellationToken cancellationToken = default)
        {
            var result = await roleService.CreateRoleAsync(request, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(GetRoleById), new { result.Value!.Id }, result.Value) : result.ToProblem();
        }

        [HttpPut("{roleId}")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> UpdateRole([FromRoute] string roleId, [FromBody] RoleRequest request, CancellationToken cancellationToken = default)
        {
            var result = await roleService.UpdateRoleAsync(roleId, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{roleId}/toggle-status")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string roleId)
        {
            var result = await roleService.ToggleStatusAsync(roleId);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
