using GPMigratorApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using FutureNHS.Api.Configuration;
using GPMigratorApp.Data;
using GPMigratorApp.GPConnect;
using GPMigratorApp.Services.Interfaces;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GPMigratorApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGPConnectService _gpConnectService;
        private readonly AppSettings _appSettings;
        private readonly IStoreRecordService _storeRecordService;

        
        public HomeController(ILogger<HomeController> logger,IStoreRecordService storeRecordService, IOptionsSnapshot<AppSettings> appSettings, IGPConnectService gpConnectService)
        {
            _logger = logger;
            _gpConnectService = gpConnectService;
            _appSettings = appSettings.Value;
            _storeRecordService = storeRecordService;
        }

        public IActionResult Index()
        {
            var search = new Search();
            search.NhsNumber = "9690937278";
            return View(search);
        }
        
        public IActionResult ExampleData()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexPost(Search search, CancellationToken cancellationToken)
        {
            var watch = new System.Diagnostics.Stopwatch();
            
            watch.Start();
            
            if (search.NhsNumber.IsNullOrEmpty())
            {
                return View("Index", search);
            }

            search.NhsNumber = search.NhsNumber.Trim();

            if (search.NhsNumber == "9465698490")
            {
                search.Response = await _gpConnectService.GetLocalFile();
                watch.Stop();
                search.TimeTaken = watch.ElapsedMilliseconds;
                return View("Index",search);
            }


            var request = CreateRequest(search.NhsNumber.Trim());

            var options = new JsonSerializerOptions();
            options.WriteIndented = true;
            
            string jsonString = JsonSerializer.Serialize(request);

            try
            {
                var result = await _gpConnectService.SendRequestAsync(HttpMethod.Post,
                    "/Patient/$gpc.getstructuredrecord", Guid.NewGuid().ToString(), _appSettings.ConsumerASID,
                    _appSettings.ProviderASID, JsonContent.Create(request));

                search.Request = request;
                search.Response = result;
            }
            catch (BadHttpRequestException exception)
            {
                ViewData.ModelState.AddModelError("NhsNumber", exception.Message);
            }

            await _storeRecordService.StoreRecord(search.Response, cancellationToken);
            watch.Stop();
            search.TimeTaken = watch.ElapsedMilliseconds;
            return View("Index",search);
        }

        private StructuredRecordRequest CreateRequest(string nhsNumber)
        {
            var data = new StructuredRecordRequest
            {
                resourceType = "Parameters",
                parameter = new List<Parameter>
                {
                    new()
                    {
                        name = "patientNHSNumber",
                        valueIdentifier = new ValueIdentifier
                        {
                            system = "https://fhir.nhs.uk/Id/nhs-number",
                            value = nhsNumber
                        }
                    },
                    new()
                    {
                        name = "includeAllergies",
                        part = new List<Part>
                        {
                            new Part
                            {
                                name = "includeResolvedAllergies",
                                valueBoolean = true
                            }
                        }
                    },
                    new Parameter
                    {
                        name = "includeMedication"
                    },
                    new Parameter
                    {
                        name = "includeConsultations",
                        part = new List<Part>
                        {
                            new Part
                            {
                                name = "includeNumberOfMostRecent",
                                valueInteger = 999
                            }
                        }
                    },
                    new Parameter
                    {
                        name = "includeProblems"
                    },
                    new Parameter
                    {
                        name = "includeImmunisations"
                    },
                    new Parameter
                    {
                        name = "includeUncategorisedData"
                    },
                    new Parameter
                    {
                        name = "includeInvestigations"
                    },
                    new Parameter
                    {
                        name = "includeReferrals"
                    }
                }
            };
            return data;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}