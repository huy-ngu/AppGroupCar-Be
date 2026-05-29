using AppGroupCar.Data;
using AppGroupCar.DTO.RequestDTO.Statistic;
using AppGroupCar.DTO.ResponseDTO.Statistic;
using AppGroupCar.Entities.Bus;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Statistics.AdminCompanyStatistics
{
    public class AdminCompanyStatisticService : IAdminCompanyStatisticService
    {
        private readonly ApplicationDbContext _context;

        public AdminCompanyStatisticService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyRevenueResponseDTO> GetCompanyRevenueAsync(CompanyRevenueInDateRangeRequestDTO request)
        {
            var revenue = await _context.BusBookings
                .Where(b => b.Showtime.CompanyId == request.CompanyId &&
                            (b.Status == BusBookingStatus.Cash || b.Status == BusBookingStatus.Paid) &&
                            b.DeleteAt == null &&
                            b.CreatedAt >= request.StartDate &&
                            b.CreatedAt <= request.EndDate)
                .GroupBy(b => 1)
                .Select(g => new
                {
                    TotalRevenue = g.Sum(b => b.TotalPrice),
                    BookingCount = g.Count()
                })
                .FirstOrDefaultAsync();

            var totalRevenue = revenue?.TotalRevenue ?? 0;
            var bookingCount = revenue?.BookingCount ?? 0;

            return new CompanyRevenueResponseDTO
            {
                CompanyId = request.CompanyId,
                TotalRevenue = totalRevenue,
                BookingCount = bookingCount,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };
        }

        public async Task<CompletedShowTimeCountResponseDTO> GetCompletedShowtimeCountAsync(CompanyRevenueInDateRangeRequestDTO request)
        {
            var completedCount = await _context.BusShowtimes
                .Where(s => s.CompanyId == request.CompanyId &&
                            s.Status == BusShowtimeStatus.complete &&
                            s.DeleteAt == null &&
                            s.CreatedAt >= request.StartDate &&
                            s.CreatedAt <= request.EndDate)
                .CountAsync();

            return new CompletedShowTimeCountResponseDTO
            {
                CompanyId = request.CompanyId,
                CompletedShowtimeCount = completedCount,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };
        }

        public async Task<CompanyMonthlyRevenueResponseDTO> GetCompanyMonthlyRevenueAsync(CompanyMonthlyRevenueRequestDTO request)
        {
            // Tính toán 12 tháng trở lại từ tháng hiện tại
            var now = DateTime.UtcNow;
            var startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-11);
            var endDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddSeconds(-1);

            // Lấy dữ liệu booking từ database
            var monthlyData = await _context.BusBookings
                .Where(b => b.Showtime.CompanyId == request.CompanyId &&
                            (b.Status == BusBookingStatus.Cash || b.Status == BusBookingStatus.Paid) &&
                            b.DeleteAt == null &&
                            b.CreatedAt >= startDate &&
                            b.CreatedAt <= endDate)
                .GroupBy(b => new { Year = b.CreatedAt.Year, Month = b.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalRevenue = g.Sum(b => b.TotalPrice),
                    BookingCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            // Tạo danh sách đầy đủ 12 tháng
            var allMonths = new List<MonthlyRevenueDetailDTO>();
            for (int i = 0; i < 12; i++)
            {
                var monthDate = now.AddMonths(-11 + i);
                var monthYear = monthDate.Year;
                var month = monthDate.Month;

                var existingData = monthlyData.FirstOrDefault(m => m.Year == monthYear && m.Month == month);

                allMonths.Add(new MonthlyRevenueDetailDTO
                {
                    Year = monthYear,
                    Month = month,
                    Revenue = existingData?.TotalRevenue ?? 0,
                    BookingCount = existingData?.BookingCount ?? 0
                });
            }

            var totalRevenue = allMonths.Sum(m => m.Revenue);

            return new CompanyMonthlyRevenueResponseDTO
            {
                CompanyId = request.CompanyId,
                TotalRevenue = totalRevenue,
                MonthlyRevenues = allMonths
            };
        }
    }
}
