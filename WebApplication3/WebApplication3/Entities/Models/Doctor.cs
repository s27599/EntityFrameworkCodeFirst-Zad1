namespace WebApplication3.Entities.Models;

public class Doctor
{
    public int IdDoctor { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Specialty { get; set; }
    public ICollection<Prescription> Prescriptions { get; set; }

}