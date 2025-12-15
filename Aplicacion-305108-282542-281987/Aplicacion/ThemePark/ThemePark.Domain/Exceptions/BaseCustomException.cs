namespace ThemePark.Exceptions;

public abstract class BaseCustomException : Exception
{
    public string TechnicalDetails { get; }

    protected BaseCustomException(string clientMessage, string technicalDetails)
        : base(clientMessage)
    {
        TechnicalDetails = technicalDetails;
    }

    protected BaseCustomException(string clientMessage, string technicalDetails, Exception innerException)
        : base(clientMessage, innerException)
    {
        TechnicalDetails = technicalDetails;
    }
}
