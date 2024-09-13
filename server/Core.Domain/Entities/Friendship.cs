using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class Friendship
{
    public string UserId { get; set; }
    public string FriendId { get; set; }
}

