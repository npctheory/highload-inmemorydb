using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Entities;

public class User
{
    public string Id { get; set; }

    public string PasswordHash {get; set; }

    public string FirstName { get; set; }

    public string SecondName { get; set; }

    public DateTime Birthdate { get; set; }

    public string Biography { get; set; }

    public string City { get; set; }
}