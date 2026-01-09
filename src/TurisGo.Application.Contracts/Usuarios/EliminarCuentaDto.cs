using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Usuarios
{
    public class EliminarCuentaDto
    {
        [Required(ErrorMessage = "La contraseña es requerida para confirmar la eliminación.")]
        public string Password { get; set; }
    }
}
