using Microsoft.AspNetCore.Identity;

namespace CinemaApp.Data.Models;

public class AppUser : IdentityUser
{
    // Mirrors the Identity role for easy querying without joining AspNetUserRoles.
    // Always keep in sync with the Identity role assignment.
    public Role Role { get; set; } = Role.User;
}
