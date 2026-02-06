using AppGroupCar.Entities;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Entities.Market;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<VehicleSeatLayout> VehicleSeatLayouts { get; set; }

        // Bus domain
        public DbSet<BusBooking> BusBookings { get; set; }
        public DbSet<BusBookingSeat> BusBookingSeats { get; set; }
        public DbSet<BusRoute> BusRoutes { get; set; }
        public DbSet<BusRoutePoint> BusRoutePoints { get; set; }
        public DbSet<BusShowtime> BusShowtimes { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyDriver> CompanyDrivers { get; set; }
        public DbSet<CompanyUser> CompanyUsers { get; set; }
        public DbSet<CompanyVehicle> CompanyVehicles { get; set; }
        public DbSet<ShowtimeLog> ShowtimeLogs { get; set; }

        // Market domain
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingItem> BookingItems { get; set; }
        public DbSet<CargoOfferDetail> CargoOfferDetails { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OfferRoutePoint> OfferRoutePoints { get; set; }
        public DbSet<RideRequest> RideRequests { get; set; }
        public DbSet<SharedRideOfferDetail> SharedRideOfferDetails { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentOffer> ShipmentOffers { get; set; }
        public DbSet<ShipmentRequest> ShipmentRequests { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Phải giữ lại dòng base này để Identity thiết lập các cấu hình mặc định trước
            base.OnModelCreating(builder);

            // Đổi tên các bảng
            builder.Entity<ApplicationUser>(b => b.ToTable("Users"));
            builder.Entity<IdentityRole>(b => b.ToTable("Roles"));
            builder.Entity<IdentityUserRole<string>>(b => b.ToTable("UserRoles"));
            builder.Entity<IdentityUserClaim<string>>(b => b.ToTable("UserClaims"));
            builder.Entity<IdentityUserLogin<string>>(b => b.ToTable("UserLogins"));
            builder.Entity<IdentityRoleClaim<string>>(b => b.ToTable("RoleClaims"));
            builder.Entity<IdentityUserToken<string>>(b => b.ToTable("UserTokens"));

            // Đổi tên bảng domain
            builder.Entity<RefreshToken>(b => b.ToTable("RefreshTokens"));
            builder.Entity<Location>(b => b.ToTable("Locations"));

            builder.Entity<VehicleSeatLayout>(b => b.ToTable("VehicleSeatLayouts"));
            builder.Entity<BusBooking>(b => b.ToTable("BusBookings"));
            builder.Entity<BusBookingSeat>(b => b.ToTable("BusBookingSeats"));
            builder.Entity<BusRoute>(b => b.ToTable("BusRoutes"));
            builder.Entity<BusRoutePoint>(b => b.ToTable("BusRoutePoints"));
            builder.Entity<BusShowtime>(b => b.ToTable("BusShowtimes"));
            builder.Entity<Company>(b => b.ToTable("Companies"));
            builder.Entity<CompanyDriver>(b => b.ToTable("CompanyDrivers"));
            builder.Entity<CompanyUser>(b => b.ToTable("CompanyUsers"));
            builder.Entity<CompanyVehicle>(b => b.ToTable("CompanyVehicles"));
            builder.Entity<ShowtimeLog>(b => b.ToTable("ShowtimeLogs"));
            builder.Entity<Booking>(b => b.ToTable("Bookings"));
            builder.Entity<BookingItem>(b => b.ToTable("BookingItems"));
            builder.Entity<CargoOfferDetail>(b => b.ToTable("CargoOfferDetails"));
            builder.Entity<Driver>(b => b.ToTable("Drivers"));
            builder.Entity<Offer>(b => b.ToTable("Offers"));
            builder.Entity<OfferRoutePoint>(b => b.ToTable("OfferRoutePoints"));
            builder.Entity<RideRequest>(b => b.ToTable("RideRequests"));
            builder.Entity<SharedRideOfferDetail>(b => b.ToTable("SharedRideOfferDetails"));
            builder.Entity<Shipment>(b => b.ToTable("Shipments"));
            builder.Entity<ShipmentOffer>(b => b.ToTable("ShipmentOffers"));
            builder.Entity<ShipmentRequest>(b => b.ToTable("ShipmentRequests"));
            builder.Entity<Vehicle>(b => b.ToTable("Vehicles"));


            // CompanyUser
            builder.Entity<CompanyUser>(b =>
            {
                b.HasOne(x => x.Company)
                    .WithMany(c => c.CompanyUsers)
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CompanyDriver
            builder.Entity<CompanyDriver>(b =>
            {
                b.HasOne(x => x.Company)
                    .WithMany()
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CompanyVehicle
            builder.Entity<CompanyVehicle>(b =>
            {
                b.HasOne(x => x.Company)
                    .WithMany()
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.VehicleSeatLayout)
                    .WithMany()
                    .HasForeignKey(x => x.SeatLayoutId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BusRoutePoint
            builder.Entity<BusRoutePoint>(b =>
            {
                b.HasOne(x => x.Route)
                    .WithMany(r => r.RoutePoints)
                    .HasForeignKey(x => x.RouteId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Location)
                    .WithMany()
                    .HasForeignKey(x => x.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BusShowtime
            builder.Entity<BusShowtime>(b =>
            {
                b.HasOne(x => x.Company)
                    .WithMany()
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.CompanyVehicle)
                    .WithMany()
                    .HasForeignKey(x => x.CompanyVehicleId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Route)
                    .WithMany(r => r.Showtimes)
                    .HasForeignKey(x => x.RouteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ShowtimeLog
            builder.Entity<ShowtimeLog>(b =>
            {
                b.HasOne(x => x.Showtime)
                    .WithMany(s => s.Logs)
                    .HasForeignKey(x => x.ShowtimeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // BusBooking
            builder.Entity<BusBooking>(b =>
            {
                b.HasOne(x => x.Showtime)
                    .WithMany() // Showtimes chưa có nav Bookings
                    .HasForeignKey(x => x.ShowtimeId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BusBookingSeat
            builder.Entity<BusBookingSeat>(b =>
            {
                b.HasOne(x => x.Booking)
                    .WithMany(bk => bk.Seats)
                    .HasForeignKey(x => x.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Driver: key = UserId, 1-1 với ApplicationUser
            builder.Entity<Driver>(b =>
            {
                b.HasKey(x => x.UserId);
                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Offer
            builder.Entity<Offer>(b =>
            {
                b.HasOne(x => x.Driver)
                    .WithMany() // không có navigation từ Driver
                    .HasForeignKey(x => x.DriverId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Vehicle)
                    .WithMany() // nếu Vehicle có nav Offers thì đổi thành WithMany(v => v.Offers)
                    .HasForeignKey(x => x.VehicleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OfferRoutePoint: composite key, route order
            builder.Entity<OfferRoutePoint>(b =>
            {
                b.HasKey(x => new { x.OfferId, x.Sequence });

                b.HasOne(x => x.Offer)
                    .WithMany(o => o.RoutePoints)
                    .HasForeignKey(x => x.OfferId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Nếu cần ràng buộc Location, dùng HasOne<Location> không cần navigation
                b.HasOne<Location>()
                    .WithMany()
                    .HasForeignKey(x => x.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SharedRideOfferDetail 1-1 Offer
            builder.Entity<SharedRideOfferDetail>(b =>
            {
                b.HasOne(x => x.Offer)
                    .WithOne(o => o.SharedRideDetail)
                    .HasForeignKey<SharedRideOfferDetail>(x => x.OfferId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CargoOfferDetail 1-1 Offer
            builder.Entity<CargoOfferDetail>(b =>
            {
                b.HasOne(x => x.Offer)
                    .WithOne(o => o.CargoDetail)
                    .HasForeignKey<CargoOfferDetail>(x => x.OfferId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Booking
            builder.Entity<Booking>(b =>
            {
                b.HasOne(x => x.Offer)
                    .WithMany() // nếu Offer có nav Bookings thì đổi WithMany(o => o.Bookings)
                    .HasForeignKey(x => x.OfferId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Customer)
                    .WithMany()
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BookingItem
            builder.Entity<BookingItem>(b =>
            {
                b.HasOne(x => x.Booking)
                    .WithMany(bk => bk.Items)
                    .HasForeignKey(x => x.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RideRequest
            builder.Entity<RideRequest>(b =>
            {
                b.HasOne(x => x.Customer)
                    .WithMany()
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.FromLocation)
                    .WithMany()
                    .HasForeignKey(x => x.FromLocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.ToLocation)
                    .WithMany()
                    .HasForeignKey(x => x.ToLocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ShipmentRequest
            builder.Entity<ShipmentRequest>(b =>
            {
                b.HasOne(x => x.Customer)
                    .WithMany()
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.PickupLocation)
                    .WithMany()
                    .HasForeignKey(x => x.PickupLocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.DropoffLocation)
                    .WithMany()
                    .HasForeignKey(x => x.DropoffLocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ShipmentOffer
            builder.Entity<ShipmentOffer>(b =>
            {
                b.HasOne(x => x.Request)
                    .WithMany(r => r.Offers)
                    .HasForeignKey(x => x.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Driver)
                    .WithMany()
                    .HasForeignKey(x => x.DriverId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Vehicle)
                    .WithMany()
                    .HasForeignKey(x => x.VehicleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Shipment 1-1 ShipmentOffer
            builder.Entity<Shipment>(b =>
            {
                b.HasOne(x => x.ShipmentOffer)
                    .WithOne(o => o.Shipment)
                    .HasForeignKey<Shipment>(x => x.ShipmentOfferId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
