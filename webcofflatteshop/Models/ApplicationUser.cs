using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webcofflatteshop.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(300)]
    public string Address { get; set; } = string.Empty;
}
