using SurveyBasket.Api.Contracts.User;

namespace SurveyBasket.Api.Sevices
{
    public interface IUserService
    {
        Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId);
        Task UpdateUserProfileAsync(string userId, UpdateUserRequest request);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    }
}
