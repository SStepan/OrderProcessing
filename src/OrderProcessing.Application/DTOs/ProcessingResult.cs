namespace OrderProcessing.Application.DTOs;

public class ProcessingResult
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; }
    
    public static ProcessingResult Success() => new ProcessingResult 
    { 
        IsSuccess = true 
    };
    
    public static ProcessingResult Failure(string errorMessage) => new ProcessingResult
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}