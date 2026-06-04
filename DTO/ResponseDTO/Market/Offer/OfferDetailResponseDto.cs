using System;
using System.Collections.Generic;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market.Offer
{
    public class OfferDetailResponseDto
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public DateTime DepartureTime { get; set; }
        public decimal BasePrice { get; set; }
        public OfferStatus Status { get; set; }
        public ServiceType ServiceType { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public OfferRouteResponseDto Route { get; set; } = new();
        public OfferVehicleDto Vehicle { get; set; } = new();
        public OfferDriverDto Driver { get; set; } = new();
    }

    public class OfferRouteResponseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
        public List<OfferRoutePointResponseDto> RoutePoints { get; set; } = new();
    }

    public class OfferRoutePointResponseDto
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public RouteStopType StopType { get; set; }
        public bool PickupAllowed => StopType is RouteStopType.Start or RouteStopType.Pickup;
        public bool DropoffAllowed => StopType is RouteStopType.Dropoff or RouteStopType.End;
    }

    public class OfferVehicleDto
    {
        public Guid VehicleId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int SeatCapacity { get; set; }
        public int VehicleType { get; set; }
        public string? UrlImage { get; set; }
    }

    public class OfferDriverDto
    {
        public Guid DriverId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseClass { get; set; } = string.Empty;
        public decimal DriverRatingAverage { get; set; }
        public int DriverRatingCount { get; set; }
    }
}
