namespace Kromer.Models.Api.Internal;

public class PlayerUpdateRequest
{
    public required Guid Uuid { get; set; }
    
    public required string Username { get; set; }
}