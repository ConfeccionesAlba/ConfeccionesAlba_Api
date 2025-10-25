namespace ConfeccionesAlba_Api.Models.Interfaces;

public interface IAuditableEntity
{
    DateTime CreatedOn { get; set; }
    DateTime UpdatedOn { get; set; }
}