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
        public bool RecibirEnPantalla { get;  set; }
        public bool RecibirPorEmail { get;  set; }
        public FrecuenciaNotificacion Frecuencia { get;  set; }


        // Constructor por defecto - inicializa con valores predeterminados
        // Notificaciones en pantalla activadas, email desactivado, frecuencia inmediata
        public PreferenciasNotificacion() {
            RecibirEnPantalla = true;
            RecibirPorEmail = false;
            Frecuencia = FrecuenciaNotificacion.Inmediata;
        }

        // Constructor con parámetros para crear preferencias personalizadas
        public PreferenciasNotificacion(
            bool recibirEnPantalla,
            bool recibirPorEmail,
            FrecuenciaNotificacion frecuencia)
        {
            RecibirEnPantalla = recibirEnPantalla;
            RecibirPorEmail = recibirPorEmail;
            Frecuencia = frecuencia;
        }

        // Crea una nueva instancia con preferencias actualizadas
        // Sigue el patrón de inmutabilidad de Value Objects
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

        // Implementación requerida por ValueObject para comparación por valor
        // Define qué propiedades determinan la igualdad entre instancias
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return RecibirEnPantalla;
            yield return RecibirPorEmail;
            yield return Frecuencia;
        }

    }
}
