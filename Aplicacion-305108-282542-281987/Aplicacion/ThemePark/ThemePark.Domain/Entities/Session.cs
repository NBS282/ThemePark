using ThemePark.Exceptions;
namespace ThemePark.Entities;

public class Session
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    private string _token = string.Empty;
    public string Token
    {
        get => _token;
        set
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new InvalidUserDataException("Token");
            }

            _token = value;
        }
    }

    private DateTime _expirationDate;
    public DateTime ExpirationDate
    {
        get => _expirationDate;
        set
        {
            ValidateDateIntegrity(_createdAt, value);
            _expirationDate = value;
        }
    }

    private DateTime _createdAt;
    public DateTime CreatedAt
    {
        get => _createdAt;
        set
        {
            ValidateDateIntegrity(value, _expirationDate);
            _createdAt = value;
        }
    }

    private void ValidateDateIntegrity(DateTime createdAt, DateTime expirationDate)
    {
        if(createdAt != DateTime.MinValue && expirationDate != DateTime.MinValue && createdAt > expirationDate)
        {
            throw new InvalidUserDataException("La fecha de creación no puede ser posterior a la fecha de expiración", "Creation date cannot be after expiration date");
        }
    }
}
