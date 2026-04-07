using AppGroupCar.Data;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Exceptions;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AppGroupCar.Services.Bus.ShowtimeServices
{
    internal class BusShowtimeSeatCreator
    {
        private static readonly Regex HasNumberRegex = new("\\d", RegexOptions.Compiled);

        private readonly ApplicationDbContext _context;

        public BusShowtimeSeatCreator(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<string> ExtractSeatNumbers(string layoutJson)
        {
            using var document = JsonDocument.Parse(layoutJson);
            var seatNumbers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            TraverseJson(document.RootElement, seatNumbers);

            if (seatNumbers.Count == 0)
                throw new AppException(400, "Seat layout does not contain any valid seatNumber");

            return seatNumbers.OrderBy(x => x).ToList();
        }

        public List<Seat> BuildSeatEntities(Guid showtimeId, IEnumerable<string> seatNumbers)
        {
            var now = DateTime.UtcNow;

            return seatNumbers.Select(seatNumber => new Seat
            {
                Id = Guid.NewGuid(),
                SeatId = Guid.NewGuid(),
                ShowtimeId = showtimeId,
                SeatNumber = seatNumber,
                Status = SeatStatus.Available,
                LastUpdatedAt = now
            }).ToList();
        }

        public async Task SaveSeatsAsync(List<Seat> seats)
        {
            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();
        }

        private static void TraverseJson(JsonElement element, HashSet<string> seatNumbers)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        if (property.NameEquals("seatNumber"))
                        {
                            var seatNumber = property.Value.ValueKind switch
                            {
                                JsonValueKind.String => property.Value.GetString(),
                                JsonValueKind.Number => property.Value.GetRawText(),
                                _ => null
                            };

                            if (string.IsNullOrWhiteSpace(seatNumber))
                                continue;

                            seatNumber = seatNumber.Trim();

                            if (string.Equals(seatNumber, "DRV", StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (!HasNumberRegex.IsMatch(seatNumber))
                                continue;

                            seatNumbers.Add(seatNumber);
                        }

                        TraverseJson(property.Value, seatNumbers);
                    }
                    break;

                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                    {
                        TraverseJson(item, seatNumbers);
                    }
                    break;
            }
        }
    }
}
