namespace Kolokwium.DTO;

public class ExceptionResponse
{
    public required int StatusCode { get; set; } = StatusCodes.Status400BadRequest;
    
    public required string Message { get; set; } = "Bad request";
    
    public string? StackTrace { get; set; } = string.Empty;
}