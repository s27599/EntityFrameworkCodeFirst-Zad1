namespace WebApplication3.Entities.Models;

public class Medicament
{
    public int IdMedicament { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Dose { get; set; }
    public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

}