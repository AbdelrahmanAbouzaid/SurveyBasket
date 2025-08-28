using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Contracts.User;

namespace SurveyBasket.Api.Sevices
{
    public class UserService(UserManager<AppUser> userManager) : IUserService
    {
        public async Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId)
        {
            var user = await userManager.Users
                .Where(u => u.Id == userId)
                .ProjectToType<UserProfileResponse>()
                .SingleAsync();

            return Result.Success(user);
        }
        public async Task UpdateUserProfileAsync(string userId, UpdateUserRequest request)
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
    }
}
