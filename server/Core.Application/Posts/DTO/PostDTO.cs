using System;

namespace Core.Application.Posts.DTO;
public class PostDTO
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}