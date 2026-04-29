using System.ComponentModel.DataAnnotations;

namespace Jegymester.DTOs // Használhatod ezt a namespace-t, hogy ne kelljen importálni
{
    public class PurchaseDto
    {
        public int ScreeningId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(\+36|06)[0-9]{9}$", ErrorMessage = "Érvénytelen telefonszám formátum!")]
        public string PhoneNumber { get; set; }

        public List<string> Seats { get; set; }
    }
}