using AppGroupCar.Entities;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Entities.Identity;
using AppGroupCar.Entities.Market;
using AppGroupCar.Entities.Wallet;
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
        public DbSet<Review> Reviews { get; set; }

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
        public DbSet<Seat> Seats { get; set; }
        public DbSet<SeatStatusHistory> SeatStatusHistories { get; set; }

        // Wallet domain
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Withdraw> Withdrawals { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Payment> Payments { get; set; }



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
            builder.Entity<Review>(b => b.ToTable("Reviews"));

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
            builder.Entity<Wallet>(b => b.ToTable("Wallets"));
            builder.Entity<Deposit>(b => b.ToTable("Deposits"));
            builder.Entity<Withdraw>(b => b.ToTable("Withdrawals"));
            builder.Entity<WalletTransaction>(b => b.ToTable("WalletTransactions"));
            builder.Entity<Payment>(b => b.ToTable("Payments"));

            // Wallet
            builder.Entity<Wallet>(b =>
            {
                b.Property(x => x.Balance)
                    .HasColumnType("decimal(18,2)");

                b.Property(x => x.Status)
                    .HasConversion<int>();

                b.HasIndex(x => x.UserId)
                    .IsUnique();

                b.HasCheckConstraint("CK_Wallet_Balance_NonNegative", "[Balance] >= 0");

                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(x => x.Deposits)
                    .WithOne(x => x.Wallet)
                    .HasForeignKey(x => x.WalletId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(x => x.Withdrawals)
                    .WithOne(x => x.Wallet)
                    .HasForeignKey(x => x.WalletId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(x => x.Transactions)
                    .WithOne(x => x.Wallet)
                    .HasForeignKey(x => x.WalletId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // review
            builder.Entity<Review>(b =>
            {
                b.HasKey(x => x.Id);
                // Cấu hình Enum thành Int dưới Database
                b.Property(x => x.ServiceType)
                    .HasConversion<int>();

                // Giới hạn độ dài bình luận, cho phép null
                b.Property(x => x.Comment)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                // Ràng buộc Check: Rating chỉ được từ 1 đến 5
                b.HasCheckConstraint("CK_Review_Rating_Range", "[Rating] >= 1 AND [Rating] <= 5");

                // Index hỗ trợ truy vấn nhanh danh sách đánh giá của 1 chuyến đi
                b.HasIndex(x => new { x.ReferenceId, x.ServiceType });

                // Unique Index: Ràng buộc cốt lõi, 1 user chỉ được đánh giá 1 chuyến đi 1 lần
                b.HasIndex(x => new { x.ReferenceId, x.ServiceType, x.ReviewerId })
                    .IsUnique()
                    .HasDatabaseName("IX_Review_OnePerUserPerTrip");

                // Khóa ngoại: Người đánh giá (Bắt buộc Restrict để tránh lỗi Multiple Cascade Paths)
                b.HasOne(x => x.Reviewer)
                    .WithMany()
                    .HasForeignKey(x => x.ReviewerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Khóa ngoại: Người bị đánh giá (Bắt buộc Restrict)
                b.HasOne(x => x.Reviewee)
                    .WithMany()
                    .HasForeignKey(x => x.RevieweeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Deposit
            builder.Entity<Deposit>(b =>
            {
                b.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)");

                b.Property(x => x.Status)
                    .HasConversion<int>();

                b.Property(x => x.Provider)
                    .HasConversion<int>();

                b.HasIndex(x => x.Code)
                    .IsUnique();

                b.HasIndex(x => x.ProviderTransactionId)
                    .IsUnique()
                    .HasFilter("[ProviderTransactionId] IS NOT NULL");

                b.HasCheckConstraint("CK_Deposit_Amount_Positive", "[Amount] > 0");
            });

            // Withdrawal
            builder.Entity<Withdraw>(b =>
            {
                b.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)");

                b.Property(x => x.Status)
                    .HasConversion<int>();

                b.HasCheckConstraint("CK_Withdraw_Amount_Positive", "[Amount] > 0");
            });

            // Wallet transaction
            builder.Entity<WalletTransaction>(b =>
            {
                b.Property(x => x.Type)
                    .HasConversion<int>();

                b.Property(x => x.Status)
                    .HasConversion<int>();

                b.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)");

                b.Property(x => x.BalanceBefore)
                    .HasColumnType("decimal(18,2)");

                b.Property(x => x.BalanceAfter)
                    .HasColumnType("decimal(18,2)");

                b.HasIndex(x => new { x.ReferenceType, x.ReferenceId });

                b.HasCheckConstraint("CK_WalletTransaction_Amount_Positive", "[Amount] > 0");
                b.HasCheckConstraint("CK_WalletTransaction_BalanceAfter_NonNegative", "[BalanceAfter] >= 0");
            });

            // Payment
            builder.Entity<Payment>(b =>
            {
                b.Property(x => x.PaymentMethod)
                    .HasConversion<int>();

                b.Property(x => x.Status)
                    .HasConversion<int>();

                b.Property(x => x.Amount)
                    .HasColumnType("decimal(18,2)");

                b.HasIndex(x => x.Code)
                    .IsUnique();

                b.HasIndex(x => x.BookingId);

                b.HasCheckConstraint("CK_Payment_Amount_Positive", "[Amount] > 0");
            });

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

                entity.HasQueryFilter(b => b.DeleteAt == null);

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

            //CompanyDriver
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
                b.Property(x => x.StopType)
                    .HasConversion<int>();

                b.HasOne(x => x.Route)
                    .WithMany(r => r.RoutePoints)
                    .HasForeignKey(x => x.RouteId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Location)
                    .WithMany(x => x.BusRoutePoints)
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

                b.HasOne(x => x.CompanyDriver)
                    .WithMany()
                    .HasForeignKey(x => x.CompanyDriverId)
                    .OnDelete(DeleteBehavior.Restrict);

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

                b.Property(x => x.StopType)
                    .HasConversion<int>();

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

                b.HasOne(x => x.Offer)
                   .WithMany()
                   .HasForeignKey(x => x.OfferId)
                   .IsRequired(false)
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
                    .OnDelete(DeleteBehavior.Cascade); // Thầy nhắc nhẹ: Xóa Offer mà Cascade bay luôn Vận Đơn thực tế là đền ốm tiền đấy nhé, nhưng DB em đang thiết kế thế thì thầy tôn trọng.

                b.HasQueryFilter(x => x.DeleteAt == null);

            });

        }
    }
}
