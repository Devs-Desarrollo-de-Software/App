using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;

namespace TurisGo.Usuarios
{
    public class PreferenciasNotificacion : ValueObject
    {
        public bool RecibirEnPantalla { get; private set; }
        public bool RecibirPorEmail { get; private set; }
        public FrecuenciaNotificacion Frecuencia { get; private set; }

        public PreferenciasNotificacion() {
            //Valores por defecto
            RecibirEnPantalla = true;
            RecibirPorEmail = false;
            Frecuencia = FrecuenciaNotificacion.Inmediata;
        }

        // Constructor con parámetros
        public PreferenciasNotificacion(
            bool recibirEnPantalla,
            bool recibirPorEmail,
            FrecuenciaNotificacion frecuencia)
        {
            RecibirEnPantalla = recibirEnPantalla;
            RecibirPorEmail = recibirPorEmail;
            Frecuencia = frecuencia;
        }

        // Método para actualizar (crea una nueva instancia porque es inmutable)
        public PreferenciasNotificacion Actualizar(
            bool recibirEnPantalla,
            bool recibirPorEmail,
            FrecuenciaNotificacion frecuencia)
        {
            return new PreferenciasNotificacion(
                recibirEnPantalla,
                recibirPorEmail,
                frecuencia
            );
        }

        // Implementación requerida de ValueObject
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return RecibirEnPantalla;
            yield return RecibirPorEmail;
            yield return Frecuencia;
        }

    }
}
