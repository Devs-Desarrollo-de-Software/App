public class Calificacion : AuditedAggregateRoot<Guid>, IUserOwned
{
    public int Puntuacion { get; private set; }

    public string Comentario { get; private set; }

    public Guid DestinoId { get; private set; }

    public Guid UsuarioId { get; set; }   // Para filtrar por usuario

    protected Calificacion() { } // Constructor protegido para EF Core

    public Calificacion(Guid id, Guid destinoId, Guid usuarioId, int puntuacion, string comentario) : base(id)
    {
        if (puntuacion < 1 || puntuacion > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(puntuacion), "La puntuación debe estar entre 1 y 5.");
        }

        if (DestinoId == Guid.Empty)
        {
            throw new ArgumentException("El ID del destino no puede estar vacío.", nameof(DestinoId));
        }

        if (UsuarioId == Guid.Empty)
        {
            throw new ArgumentException("El ID del usuario no puede estar vacío.", nameof(UsuarioId));
        }

        Puntuacion = puntuacion;
        Comentario = comentario.Trim();
        DestinoId = destinoId;
        UsuarioId = usuarioId;
    }

}