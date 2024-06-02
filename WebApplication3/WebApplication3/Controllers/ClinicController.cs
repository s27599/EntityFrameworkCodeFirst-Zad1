using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Entities;
using WebApplication3.Entities.Models;
using WebApplication3.Entities.Models.DTO;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicController : ControllerBase
{
    
   
        private readonly ClinicDbContext _context;

        public ClinicController(ClinicDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription(AddPrescriptionDTO request)
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

            return Created();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Doctor)
                .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.PrescriptionMedicaments)
                .ThenInclude(pm => pm.Medicament)
                .FirstOrDefaultAsync(p => p.IdPatient == id);

            if (patient == null)
            {
                return NotFound("Pacjent nie istnieje.");
            }

            var response = new GetPatient
            {
                IdPatient = patient.IdPatient,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Birthdate = patient.Birthdate,
                Prescriptions = patient.Prescriptions
                    .OrderBy(p => p.DueDate)
                    .Select(p => new PrescriptionDto
                    {
                        IdPrescription = p.IdPrescription,
                        Date = p.Date,
                        DueDate = p.DueDate,
                        Doctor = new DoctorDto
                        {
                            IdDoctor = p.Doctor.IdDoctor,
                            FirstName = p.Doctor.FirstName,
                            LastName = p.Doctor.LastName,
                            Specialty = p.Doctor.Specialty
                        },
                        Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentDto
                        {
                            IdMedicament = pm.Medicament.IdMedicament,
                            Name = pm.Medicament.Name,
                            Description = pm.Medicament.Description,
                            Dose = pm.Medicament.Dose
                        }).ToList()
                    }).ToList()
            };

            return Ok(response);
        }
    
}