using AppGroupCar.Entities;

namespace AppGroupCar.Services.RouteRules
{
    public interface IRouteValidationService
    {
        Task ValidateRouteForCreationAsync(RouteValidationRequest request, CancellationToken cancellationToken = default);

        Task EnsureLocationAllowedForRouteAsync(Guid locationId, CancellationToken cancellationToken = default);

        Task<Location> GetRouteLocationOrThrowAsync(Guid locationId, CancellationToken cancellationToken = default);
    }
}
