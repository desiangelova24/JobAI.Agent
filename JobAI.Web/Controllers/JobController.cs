using JobAI.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobAI.Web.Controllers
{
    public class JobController : Controller
    {
        private readonly JobService _jobService;

        public JobController(JobService jobService)
        {
            _jobService = jobService;
        }
        public async Task<IActionResult> Index()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return View(jobs); 
        }
    }
}
