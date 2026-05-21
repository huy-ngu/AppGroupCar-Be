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
    }
}
