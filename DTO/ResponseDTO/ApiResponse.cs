namespace AppGroupCar.DTO.ResponseDTO
{
    public class ApiResponse<T>
    {
        public int Code { get; set; } = 200;
        public string Message { get; set; } = "Success";
        public T Data { get; set; }
    }
}
