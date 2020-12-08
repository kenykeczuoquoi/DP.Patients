using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DP.Patients.Model;
using DP.Patients.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DP.Patients.Services.ServiceBusSender;

namespace DP.Patients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ServiceBusSender _sender;

        public PatientsController(DpDataContext context, ServiceBusSender sender)
        {
            _context = context;
            _sender = sender;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult getAll()
        {
            return Ok(_context.Patients.ToList());
        }
        
    
        
        [HttpPost]
        public async Task<IActionResult> Add(Patient p)
        {
            _context.Patients.Add(p);
            _context.SaveChanges();

            await _sender.SendMessage(new MessagePayLoad() { EventName = "NewUserRegistered", UserEmail = "szymon.warchulski@gmail.com" });

            return Created("api/patients/", p);
        }

        [HttpPut]
        [AllowAnonymous]
        public IActionResult InvalidAction()
        {
            throw new InvalidOperationException("Testowy wyjątek");
        }
    }
}
