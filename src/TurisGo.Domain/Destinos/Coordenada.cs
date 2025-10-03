using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;

namespace TurisGo.Destinos
{
    public class Coordenada: ValueObject
    {
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }

        protected Coordenada() { }

        public Coordenada(double latitud, double longitud)
        {
            if (latitud < -90 || latitud > 90)
                throw new ArgumentException("La latitud estar entre -90 y 90 grados.");

            if (longitud < -180 || longitud > 180)
                throw new ArgumentException("La longitud debe estar entre -180 y 180");

            Latitud = latitud;
            Longitud = longitud;
        }

        protected override IEnumerable<object> GetAtomicValues()
        { 
            yield return Latitud;
            yield return Longitud;
        }
    }
}
