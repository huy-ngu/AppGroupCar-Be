using AppGroupCar.Entities;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Entities.Identity;
using AppGroupCar.Entities.Market;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AppGroupCar.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,
    ApplicationRole,
    Guid,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    ApplicationRoleClaim,
    ApplicationUserToken>
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
        public DbSet<Old_CompanyDriver> CompanyDrivers { get; set; }
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
        public DbSet<Seat> Seats { get; set; }
        public DbSet<SeatStatusHistory> SeatStatusHistories { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Phải giữ lại dòng base này để Identity thiết lập các cấu hình mặc định trước
            base.OnModelCreating(builder);

            // Đổi tên các bảng
            builder.Entity<ApplicationUser>(b => b.ToTable("Users"));
            builder.Entity<ApplicationRole>(b => b.ToTable("Roles"));
            builder.Entity<ApplicationUserRole>(b => b.ToTable("UserRoles"));
            builder.Entity<ApplicationUserClaim>(b => b.ToTable("UserClaims"));
            builder.Entity<ApplicationUserLogin>(b => b.ToTable("UserLogins"));
            builder.Entity<ApplicationRoleClaim>(b => b.ToTable("RoleClaims"));
            builder.Entity<ApplicationUserToken>(b => b.ToTable("UserTokens"));

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
            builder.Entity<Old_CompanyDriver>(b => b.ToTable("Old_CompanyDrivers"));
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

            builder.Entity<Seat>(entity =>
            {
                entity.Property(x => x.Id)
                    .ValueGeneratedNever();
                entity.HasKey(x => x.SeatId);

                entity.Property(x => x.SeatNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(x => x.Status)
                    .HasConversion<int>();

                entity.Property(x => x.LastUpdatedAt)
                    .IsRequired();

                entity.HasOne(x => x.Showtime)
                    .WithMany(x => x.Seats)
                    .HasForeignKey(x => x.ShowtimeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.BookingSeats)
                    .WithOne(x => x.Seat)
                    .HasForeignKey(x => x.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.SeatStatusHistories)
                    .WithOne(x => x.Seat)
                    .HasForeignKey(x => x.SeatId);

                // Index cực quan trọng

                entity.HasIndex(x => x.ShowtimeId);

                entity.HasIndex(x => new { x.ShowtimeId, x.SeatNumber })
                    .IsUnique()
                    .HasFilter("[DeleteAt] IS NULL");
            });

            builder.Entity<SeatStatusHistory>(entity =>
            {
                entity.Property(x => x.Id)
                    .ValueGeneratedNever();
                entity.HasKey(x => x.HistoryId);

                entity.Property(x => x.OldStatus)
                    .HasConversion<int>();

                entity.Property(x => x.NewStatus)
                    .HasConversion<int>();

                entity.Property(x => x.Reason)
                    .HasMaxLength(255);

                entity.Property(x => x.ChangedBy)
                    .IsRequired();

                entity.Property(x => x.ChangedAt)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.ChangedBy)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Seat)
                    .WithMany(x => x.SeatStatusHistories)
                    .HasForeignKey(x => x.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);
                // Index log

                entity.HasIndex(x => x.SeatId);

                entity.HasIndex(x => x.ChangedAt);

                b.HasQueryFilter(b => b.DeleteAt == null);

            });

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

                b.HasQueryFilter(b => b.DeleteAt == null);

            });

            // CompanyDriver
            //builder.Entity<CompanyDriver>(b =>
            //{
            //    b.HasOne(x => x.Company)
            //        .WithMany()
            //        .HasForeignKey(x => x.CompanyId)
            //        .OnDelete(DeleteBehavior.Cascade);

            //    b.HasOne(x => x.User)
            //        .WithMany()
            //        .HasForeignKey(x => x.UserId)
            //        .OnDelete(DeleteBehavior.Restrict);
            //});

            // CompanyVehicle
            builder.Entity<CompanyVehicle>(b =>
            {
                b.Ignore(x => x.Id);
                b.HasKey(x => x.CompanyVehicleId);

                b.HasIndex(x => x.PlateNumber)
                    .IsUnique()
                    .HasFilter("[DeleteAt] IS NULL");

                b.HasOne(x => x.Company)
                    .WithMany()
                    .HasForeignKey(x => x.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.VehicleSeatLayout)
                    .WithMany()
                    .HasForeignKey(x => x.SeatLayoutId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasQueryFilter(b => b.DeleteAt == null);

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

                b.HasQueryFilter(b => b.DeleteAt == null);

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
                    .HasPrincipalKey(x => x.CompanyVehicleId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Route)
                    .WithMany(r => r.Showtimes)
                    .HasForeignKey(x => x.RouteId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasQueryFilter(b => b.DeleteAt == null);

            });

            // ShowtimeLog
            builder.Entity<ShowtimeLog>(b =>
            {
                b.HasOne(x => x.Showtime)
                    .WithMany(s => s.Logs)
                    .HasForeignKey(x => x.ShowtimeId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasQueryFilter(b => b.DeleteAt == null);

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

                b.HasQueryFilter(b => b.DeleteAt == null);

            });

            // BusBookingSeat
            builder.Entity<BusBookingSeat>(b =>
            {
                b.HasIndex(x => x.SeatId)
                    .IsUnique()
                    .HasFilter("[DeleteAt] IS NULL");

                b.HasOne(x => x.Booking)
                    .WithMany(bk => bk.Seats)
                    .HasForeignKey(x => x.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(x => x.Seat)
        .WithMany(x => x.BookingSeats)
        .HasForeignKey(x => x.SeatId)
        .OnDelete(DeleteBehavior.Restrict);
            });
            // Driver: key = UserId,1-1 với ApplicationUser
            builder.Entity<Driver>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.Id)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasQueryFilter(b => b.DeleteAt == null);

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

                b.HasQueryFilter(b => b.DeleteAt == null);

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

                b.HasQueryFilter(b => b.DeleteAt == null);

            });

            // SharedRideOfferDetail1-1 Offer
            builder.Entity<SharedRideOfferDetail>(b =>
            {
                b.HasOne(x => x.Offer)
                    .WithOne(o => o.SharedRideDetail)
                    .HasForeignKey<SharedRideOfferDetail>(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasQueryFilter(b => b.DeleteAt == null);

            });

            // CargoOfferDetail1-1 Offer
            builder.Entity<CargoOfferDetail>(b =>
            {
                b.HasOne(x => x.Offer)
                    .WithOne(o => o.CargoDetail)
                    .HasForeignKey<CargoOfferDetail>(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasQueryFilter(b => b.DeleteAt == null);

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

                b.HasQueryFilter(b => b.DeleteAt == null);

            });

            // BookingItem
            builder.Entity<BookingItem>(b =>
            {
                b.HasOne(x => x.Booking)
                    .WithMany(bk => bk.Items)
                    .HasForeignKey(x => x.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(bi => bi.PickupLocation)
                    .WithMany() // Để trống nếu Location không chứa danh sách BookingItems
                    .HasForeignKey(bi => bi.PickupLocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(bi => bi.DropoffLocation)
                   .WithMany() // Để trống nếu Location không chứa danh sách BookingItems
                   .HasForeignKey(bi => bi.DropoffLocationId)
                   .OnDelete(DeleteBehavior.Restrict);

                b.HasQueryFilter(b => b.DeleteAt == null);

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

                b.HasQueryFilter(b => b.DeleteAt == null);

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

                b.HasQueryFilter(b => b.DeleteAt == null);

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

                b.HasQueryFilter(b => b.DeleteAt == null);

            });

            // Cấu hình cho bảng Shipment
            builder.Entity<Shipment>(b =>
            {
                // 1. Luồng Đấu giá: Quan hệ 1-1 với ShipmentOffer
                b.HasOne(x => x.ShipmentOffer)
                    .WithOne(o => o.Shipment) // Lưu ý: Class ShipmentOffer của em phải có property 'public virtual Shipment? Shipment { get; set; }'
                    .HasForeignKey<Shipment>(x => x.ShipmentOfferId)
                    .IsRequired(false) // BẮT BUỘC: Thêm cái này để EF Core biết trường này được phép NULL
                    .OnDelete(DeleteBehavior.Cascade); // Thầy nhắc nhẹ: Xóa Offer mà Cascade bay luôn Vận Đơn thực tế là đền ốm tiền đấy nhé, nhưng DB em đang thiết kế thế thì thầy tôn trọng.

                // 2. Luồng Tiện chuyến: Quan hệ N-1 với Offer (1 Chuyến xe ghép có thể chở NHIỀU đơn hàng)
                b.HasOne(x => x.Offer)
                    .WithMany() // Nếu class Offer em có List<Shipment> thì viết là .WithMany(o => o.Shipments)
                    .HasForeignKey(x => x.OfferId)
                    .IsRequired(false) // BẮT BUỘC: Phải cho phép NULL
                    .OnDelete(DeleteBehavior.Restrict); // Chỗ này nên để Restrict, xe đang chở hàng cấm không cho xóa chuyến.

                b.HasOne(x => x.PickupLocation)
                    .WithMany()
                    .HasForeignKey(x => x.PickupLocationId)
                    .OnDelete(DeleteBehavior.Restrict); // Rút thẻ đỏ: Cấm xóa Location nếu có Shipment xài

                // 2. Cấu hình cấm xóa lan truyền cho Điểm trả
                b.HasOne(x => x.DropoffLocation)
                    .WithMany()
                    .HasForeignKey(x => x.DropoffLocationId)
                    .OnDelete(DeleteBehavior.Restrict);
                // 3. Query Filter xóa mềm (Giữ nguyên của em)
                b.HasQueryFilter(x => x.DeleteAt == null);

                // 4. "MÀI PHẢI KIỂM TRA KỸ" - Ráp vào đây!
                b.ToTable(t => t.HasCheckConstraint(
                    name: "CK_Shipment_Must_Have_Only_One_Offer_Source",
                    sql: "(OfferId IS NOT NULL AND ShipmentOfferId IS NULL) OR (OfferId IS NULL AND ShipmentOfferId IS NOT NULL)"
                 ));
            });

        }
    }
}
