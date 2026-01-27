using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace TurisGo.Destinos
{
    public class Destino : AuditedAggregateRoot<Guid>
    {
        public string Nombre { get; private set; } = null!;

        public string Pais { get; private set; } = null!;

        public int Poblacion { get; private set; }

        public string Imagen { get; private set; } = null!;   // URL de la imagen del destino

        public Coordenada Coordenada { get; private set; }

        // ID de la ciudad en la API externa (GeoDB Cities)
        // Permite vincular el destino guardado localmente con la fuente externa
        public int ApiCityId { get; private set; }


        protected Destino() { }   // Constructor protegido para EF Core


        // Constructor que crea un destino con todas las validaciones necesarias
        public Destino(Guid id, string nombre, string pais, int poblacion, string imagen, Coordenada coordenada, int apiCityId = 0) : base(id)
        {
            if (nombre.IsNullOrEmpty())
            {
                throw new ArgumentNullException("El nombre del destino no debe estar vacio", nameof(nombre));
            }

            if (pais.IsNullOrEmpty())
            {
                throw new ArgumentNullException("El país no debe estar vacio", nameof(pais));
            }

            if (poblacion < 0)
            {
                throw new ArgumentOutOfRangeException("La poblacion no debe ser negativa", nameof(poblacion));
            }

            if (coordenada is null)
            {
                throw new ArgumentNullException("Las coordenadas no debe ser nula", nameof (coordenada));
            }

            Nombre = nombre;
            Pais = pais;
            Poblacion = poblacion;
            Imagen = imagen;
            Coordenada = coordenada;
            ApiCityId = apiCityId;
        }

    } 
}
