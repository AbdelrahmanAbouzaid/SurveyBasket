using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Contracts.Role;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Sevices
{
    public class RoleService(RoleManager<AppRole> roleManager, ApplicationDbContext context) : IRoleService
    {
        private readonly RoleManager<AppRole> roleManager = roleManager;


        public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool includeDisable = false, CancellationToken cancellationToken = default)
        => await roleManager.Roles
            .Where(r => !r.IsDefault && (!r.IsDeleted || includeDisable))
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);
        public async Task<Result<RoleDetailsResponse>> GetRoleByIdAsync(string roleId, CancellationToken cancellationToken = default)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role is null)
                return Result.Failure<RoleDetailsResponse>(RoleError.RoleNotFound);

            var permissions = await roleManager.GetClaimsAsync(role);
            var response = new RoleDetailsResponse(roleId, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));
            return Result.Success(response);

        }
        public async Task<Result<RoleDetailsResponse>> CreateRoleAsync(RoleRequest request, CancellationToken cancellationToken)
        {
            var roleIsExists = await roleManager.Roles.AnyAsync(r => r.Name == request.Name);
            if (roleIsExists)
                return Result.Failure<RoleDetailsResponse>(RoleError.DuplicatedRole);
            var allowedPermissions = Permissions.GetAllPermissions();
            if (request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailsResponse>(RoleError.InvalidPermissions);

            var newRole = new AppRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await roleManager.CreateAsync(newRole);
            if (!result.Succeeded)
                return result.Errors.FirstErrorResult<RoleDetailsResponse>();

            var claims = request.Permissions
                .Select(p => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = p,
                    RoleId = newRole.Id
                }).ToList();

            await context.RoleClaims.AddRangeAsync(claims, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var response = new RoleDetailsResponse(newRole.Id, newRole.Name, newRole.IsDeleted, request.Permissions);
            return Result.Success(response);
        }
        public async Task<Result> UpdateRoleAsync(string roleId, RoleRequest request, CancellationToken cancellationToken)
        {
            var roleIsExists = await roleManager.Roles.AnyAsync(r => r.Name == request.Name && r.Id != roleId);
            if (roleIsExists)
                return Result.Failure(RoleError.DuplicatedRole);

            var role = await roleManager.FindByIdAsync(roleId);
            if (role is null)
                return Result.Failure(RoleError.RoleNotFound);

            var allowedPermissions = Permissions.GetAllPermissions();

            if (request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure(RoleError.InvalidPermissions);

            role.Name = request.Name;

            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return result.Errors.FirstErrorResult();

            var currentPermissions = await context.RoleClaims
                .Where(rc => rc.RoleId == roleId)
                .Select(rc => rc.ClaimValue)
                .ToListAsync(cancellationToken);

            var permissionsToAdd = request.Permissions.Except(currentPermissions)
                .Select(p => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = p,
                    RoleId = roleId
                });
            var permissionsToRemove = currentPermissions.Except(request.Permissions);

            await context.RoleClaims
                .Where(rc => rc.RoleId == roleId && permissionsToRemove.Contains(rc.ClaimValue))
                .ExecuteDeleteAsync(cancellationToken);


            await context.RoleClaims.AddRangeAsync(permissionsToAdd, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        public async Task<Result> ToggleStatusAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role is null)
                return Result.Failure(RoleError.RoleNotFound);
            role.IsDeleted = !role.IsDeleted;
            await roleManager.UpdateAsync(role);
           
            return Result.Success();
        }
    }
}
