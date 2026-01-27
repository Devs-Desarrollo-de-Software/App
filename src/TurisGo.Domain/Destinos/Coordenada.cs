using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;

namespace TurisGo.Destinos
{
    // Value Object que representa una coordenada geográfica
    // Encapsula latitud y longitud con validaciones
    public class Coordenada: ValueObject
    {
        // Latitud: coordenada geográfica vertical (Norte-Sur)
        // Rango válido: -90 (Polo Sur) a +90 (Polo Norte)
        public double Latitud { get; private set; }

        // Longitud: coordenada geográfica horizontal (Este-Oeste)
        // Rango válido: -180 a +180 (Meridiano de Greenwich como referencia)
        public double Longitud { get; private set; }

        // Constructor protegido para EF Core
        protected Coordenada() { }

        // Constructor que valida los rangos de latitud y longitud
        public Coordenada(double latitud, double longitud)
        {
            if (latitud < -90 || latitud > 90)
                throw new ArgumentException("La latitud estar entre -90 y 90 grados.");

            if (longitud < -180 || longitud > 180)
                throw new ArgumentException("La longitud debe estar entre -180 y 180");

            Latitud = latitud;
            Longitud = longitud;
        }

        // Implementación requerida por ValueObject para comparación por valor
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Latitud;
            yield return Longitud;
        }
    }
}
