namespace SurveyBasket.Api.Sevices
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T: class;
        Task SetAsync(string key, object value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }
}
