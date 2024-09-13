using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class DialogMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string SenderId { get; set; } = string.Empty;
    
    [Required]
    public string ReceiverId { get; set; } = string.Empty;
    
    [Required]
    public string Text { get; set; } = string.Empty;
    
    public bool IsRead { get; set; } = false;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
