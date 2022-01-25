namespace ValNet.Requests;

public class ApiError
{
    public string httpStatus { get; set; }
    public string errorCode { get; set; }
    public string message { get; set; }
}