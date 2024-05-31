namespace WebApplication3.Entities.Models;

public class PrescriptionMedicament
{
    public int IdPrescription { get; set; }
    public Prescription Prescription { get; set; }
    public int IdMedicament { get; set; }
    public Medicament Medicament { get; set; }
}