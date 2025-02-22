using System.ComponentModel.DataAnnotations;


namespace UserManagementAPI.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public required string UserName { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string EmailAddress { get; set; }
}

