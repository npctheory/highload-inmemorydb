using System.ComponentModel.DataAnnotations;

namespace Core.Infrastructure.Snapshots;

public class FriendshipSnapshot
{
    public string UserId { get; set; }
    public string FriendId { get; set; }
}

