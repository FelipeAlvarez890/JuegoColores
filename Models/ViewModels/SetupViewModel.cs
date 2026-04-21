using System.ComponentModel.DataAnnotations;

namespace JuegoColores.Models.ViewModels
{
    public class SetupViewModel
    {
        [Required]
        public JuegoColores.Models.GameMode SelectedMode { get; set; } = JuegoColores.Models.GameMode.Collaborative;

        [Required]
        [MinLength(2, ErrorMessage = "Debe haber al menos 2 jugadores")]
        public List<string> PlayerNames { get; set; } = new List<string>();
    }
}
