using Microsoft.AspNetCore.Mvc;
using WebApplication3.Entities;
using WebApplication3.Entities.Models;
using WebApplication3.Entities.Models.DTO;

namespace WebApplication3.Controllers;

public class ClinicController
{
    [ApiController]
[Route("api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly ClinicDbContext _context;

    public PrescriptionsController(ClinicDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] AddPrescriptionDTO request)
    {
        if (request.Medicaments == null || request.Medicaments.Count == 0 || request.Medicaments.Count > 10)
        {
            return BadRequest("Recepta musi zawierać od 1 do 10 leków.");
        }

        var patient = await _context.Patients.FindAsync(request.IdPatient);
        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Birthdate = request.Birthdate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        var doctor = await _context.Doctors.FindAsync(request.IdDoctor);
        if (doctor == null)
        {
            return BadRequest("Podany lekarz nie istnieje.");
        }

        var prescription = new Prescription
        {
            Date = request.Date,
            DueDate = request.DueDate,
            Patient = patient,
            Doctor = doctor,
            PrescriptionMedicaments = new List<PrescriptionMedicament>()
        };

        foreach (var medicamentId in request.Medicaments)
        {
            var medicament = await _context.Medicaments.FindAsync(medicamentId);
            if (medicament == null)
            {
                return BadRequest($"Lek nie istnieje.");
            }

            prescription.PrescriptionMedicaments.Add(new PrescriptionMedicament
            {
                Medicament = medicament
            });
        }

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return Ok(prescription);
    }
}
}