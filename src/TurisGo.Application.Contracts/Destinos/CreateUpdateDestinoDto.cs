using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Validation;

namespace TurisGo.Destinos
{

    public class CoordenadaDto
    {
        [Range(-90,90, ErrorMessage = "La latitud debe estar entre -90 y 90.")]
        public double Latitud {  get; set; }

        [Range(-180,180, ErrorMessage = "La longitud debe estar entre -180 y 180.")]
        public double Longitud { get; set; }
    }

    public class CreateUpdateDestinoDto
    {

        [Required]
        [StringLength(100, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-ZáÁéÉíÍóÓúÚñÑ\s]+$", 
            ErrorMessage = "El nombre solo puede contener letras y espacios." )]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-ZáÁéÉíÍóÓúÚñÑ\s]+$",
            ErrorMessage = "El pais solo puede contener letras y espacios.")]
        public string Pais { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La poblacion debe ser mayor a 0.")]
        public int Poblacion { get; set; }

        [Required]
        [Url]
        public string Imagen { get; set; } = null!;

        [Required]
        public CoordenadaDto Coordenada { get; set; }

    }
}
