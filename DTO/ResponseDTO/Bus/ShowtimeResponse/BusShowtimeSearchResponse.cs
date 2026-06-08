namespace AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse
{
    public class BusShowtimeSearchResponse : BusShowtimeResponse
    {
        public string StartPoint { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public decimal RatingCompanyAverage { get; set; }
        public int RatingCompanyCount { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
    }
}
