namespace TaskManagement.API.Responses;

public class SuccessResponse<T> : ApiResponse<T>
{
   public SuccessResponse(T data, string message = "Success")
{
    ArgumentNullException.ThrowIfNull(data);

    Success = true;
    Message = message;
    Data = data;
}
}