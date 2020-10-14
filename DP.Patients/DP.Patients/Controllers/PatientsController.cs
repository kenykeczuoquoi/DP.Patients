﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DP.Patients.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DP.Patients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly DpDataContext _context;

        public PatientsController(DpDataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult getAll()
        {
            return Ok(_context.Patients.ToList());
        }
        
    
        
        [HttpPost]
        public IActionResult Add(Patient p)
        {
            _context.Patients.Add(p);
            _context.SaveChanges();

            return Created("api/patients/", p);
        }
    }
}
