using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class ClientDTO
{
    [Required]
    [RegularExpression("^[a-zA-Z]+$")]
    public string FirstName { get; set; }
    [Required]
    [RegularExpression("^[a-zA-Z]+$")]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [Phone]
    public string Telephone { get; set; }
    [Required]
    [RegularExpression("^[0-9]{11}$")]
    public string Pesel { get; set; }
}