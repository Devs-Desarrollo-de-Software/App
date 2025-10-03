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
        public string Imagen { get; private set; } = null!;

        public Coordenada Coordenada { get; private set; }
        protected Destino() { }

        public Destino(Guid id, string nombre, string pais, int poblacion, string imagen, Coordenada coordenada) : base(id)
        {
            Nombre = nombre;
            Pais = pais;
            Poblacion = poblacion;
            Imagen = imagen;
            Coordenada = coordenada ?? throw new ArgumentException(nameof(coordenada));
        }

    } 
}
