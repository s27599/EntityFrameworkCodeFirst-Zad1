using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Entities.Models;

public class Medicament
{
    [Key]
    public int IdMedicament { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Dose { get; set; }
    public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

}