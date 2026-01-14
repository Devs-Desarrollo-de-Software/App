using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Destinos
{
    public class CoordenadaDto
    {
        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90.")]
        public double Latitud { get; set; }

        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180.")]
        public double Longitud { get; set; }
    }
}
