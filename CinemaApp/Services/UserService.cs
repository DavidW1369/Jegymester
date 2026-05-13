using CinemaApp.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CinemaApp.Services;

public class UserService(UserManager<AppUser> userManager)
{
    public Task<AppUser?> GetByIdAsync(string id) =>
        userManager.FindByIdAsync(id);

    /// <summary>
    /// Updates a user's contact email and phone number.
    /// Uses UserManager so normalisation and security stamp are handled correctly.
    /// </summary>
    public async Task<(bool Success, string? Error)> UpdateContactInfoAsync(
        string userId, string newEmail, string newPhone)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return (false, "User not found.");

        var setEmail  = await userManager.SetEmailAsync(user, newEmail);
        if (!setEmail.Succeeded)
            return (false, string.Join(", ", setEmail.Errors.Select(e => e.Description)));

        // Keep UserName in sync with email (Identity uses email as username here).
        await userManager.SetUserNameAsync(user, newEmail);

        var setPhone = await userManager.SetPhoneNumberAsync(user, newPhone);
        if (!setPhone.Succeeded)
            return (false, string.Join(", ", setPhone.Errors.Select(e => e.Description)));

        return (true, null);
    }
}
