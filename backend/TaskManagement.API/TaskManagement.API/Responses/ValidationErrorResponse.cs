namespace TaskManagement.API.Responses;
public class ValidationErrorResponse : ErrorResponse
{
    private Dictionary<string, string[]> _errors = [];

    public Dictionary<string, string[]> Errors
    {
        get => _errors;
        set => _errors = value 
            ?? throw new ArgumentNullException(nameof(value));
    }
}