using GPMigratorApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GPMigratorApp.ViewComponents
{
    public class Patient: ViewComponent
    {
        private readonly ILogger<Patient> _logger;
        
        private readonly IPatientService _patientService;

        public Patient(ILogger<Patient> logger, IPatientService patientService)
        {
            _logger = logger;
            _patientService = patientService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string nhsNumber, CancellationToken cancellationToken)
        {
            var patient = await _patientService.GetPatientAsync(nhsNumber, cancellationToken);
            return View(patient);
            
            
        }

    }


}