using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Contracts.User;
using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Sevices
{
    public class UserService(UserManager<AppUser> userManager,
        IRoleService roleService,
        ApplicationDbContext context) : IUserService
    {
        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        => await (from u in context.Users
                  join ur in context.UserRoles
                  on u.Id equals ur.UserId
                  join r in context.Roles
                  on ur.RoleId equals r.Id into roles
                  where !roles.Any(x => x.Name == DefaultRole.Member)
                  select new
                      {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.IsDisabled,
                        Roles = roles.Select(x => x.Name!).ToList()
                      }
                  ).GroupBy(u => new {u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled})
            .Select(u => new UserResponse
            (
                u.Key.Id,
                u.Key.FirstName,
                u.Key.LastName,
                u.Key.Email,
                u.Key.IsDisabled,
                u.SelectMany(x => x.Roles).Distinct()
            ))
            .ToListAsync(cancellationToken);
        public async Task<Result<UserResponse>> GetAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure<UserResponse>(UserError.UserNotFound);
            var roles = await userManager.GetRolesAsync(user);
            var response = (user, roles).Adapt<UserResponse>();
            return Result.Success(response);
        }
        public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var EmailIsExist = await userManager.Users.AnyAsync(u => u.Email == request.Email);
            if (EmailIsExist)
                return Result.Failure<UserResponse>(UserError.DuplicateEmail);

            var allowedRoles = await roleService.GetAllRolesAsync(cancellationToken: cancellationToken);
            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure<UserResponse>(UserError.InvalidRoles);

            var user = request.Adapt<AppUser>();

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return result.Errors.FirstErrorResult<UserResponse>();

            await userManager.AddToRolesAsync(user, request.Roles);
            var response = (user, request.Roles).Adapt<UserResponse>();
            return Result.Success(response);
        }
        public async Task<Result> UpdateAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var EmailIsExist = await userManager.Users.AnyAsync(u => u.Email == request.Email && u.Id != userId);
            if (EmailIsExist)
                return Result.Failure(UserError.DuplicateEmail);
            var allowedRoles = await roleService.GetAllRolesAsync(cancellationToken: cancellationToken);
            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure(UserError.InvalidRoles);
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(UserError.UserNotFound);

            user = request.Adapt(user);
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return result.Errors.FirstErrorResult();
            await context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
            await userManager.AddToRolesAsync(user, request.Roles);
            return Result.Success();



        }
        public async Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId)
        {
            var user = await userManager.Users
                .Where(u => u.Id == userId)
                .ProjectToType<UserProfileResponse>()
                .SingleAsync();

            return Result.Success(user);
        }
        public async Task UpdateUserProfileAsync(string userId, UpdateProfileRequest request)
        {
            await userManager.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setters => 
                    setters
                        .SetProperty(u => u.FirstName, request.FirstName ?? string.Empty)
                        .SetProperty(u => u.LastName, request.LastName ?? string.Empty)
                );
        }
        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await userManager.FindByIdAsync(userId);


            var result = await userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
                return Result.Success();
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ToggleStatusAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure(UserError.UserNotFound);
            user.IsDisabled = !user.IsDisabled;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded ? Result.Success() : result.Errors.FirstErrorResult();
        }
        public async Task<Result> UnLockAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var result = await userManager.SetLockoutEndDateAsync(user!, DateTimeOffset.UtcNow);
            return result.Succeeded ? Result.Success() : result.Errors.FirstErrorResult();
        }
    }
}
