using SurveyBasket.Api.Contracts.Role;

namespace SurveyBasket.Api.Sevices
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool includeDisable = false, CancellationToken cancellationToken = default);
        Task<Result<RoleDetailsResponse>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);
        Task<Result<RoleDetailsResponse>> CreateRoleAsync(RoleRequest request, CancellationToken cancellationToken);
        Task<Result> UpdateRoleAsync(string roleId, RoleRequest request, CancellationToken cancellationToken);
        Task<Result> ToggleStatusAsync(string roleId);
    }
}
