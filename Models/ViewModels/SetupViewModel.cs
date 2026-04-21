using System.ComponentModel.DataAnnotations;

namespace JuegoColores.Models.ViewModels
{
    public class SetupViewModel
    {
        [Required]
        [MinLength(2, ErrorMessage = "Debe haber al menos 2 jugadores")]
        public List<string> PlayerNames { get; set; } = new List<string>();
    }
}
