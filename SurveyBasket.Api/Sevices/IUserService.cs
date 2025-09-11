using SurveyBasket.Api.Contracts.User;

namespace SurveyBasket.Api.Sevices
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<Result<UserResponse>> GetAsync(string userId);
        Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId);
        Task UpdateUserProfileAsync(string userId, UpdateProfileRequest request);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<Result> ToggleStatusAsync(string userId);
        Task<Result> UnLockAsync(string userId);
    }
}
