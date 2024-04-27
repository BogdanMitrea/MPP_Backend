﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MPP_BackEnd.Repositories;
using System.Drawing;
using System;
using System.Net;
using System.Numerics;
using System.Xml.Linq;

namespace MPP_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoneController : ControllerBase
    {
        private readonly IRepositoryPhone _repositoryPhone;
        public PhoneController(IRepositoryPhone repositoryPhone)
        {
            this._repositoryPhone = repositoryPhone;

        }

        [HttpGet]
        public IEnumerable<PhoneModel> GetAllPhones()
        {
            return this._repositoryPhone.GetAllPhones();
        }

        [HttpGet("{id}")]
        public IActionResult GetPhoneById(int id)
        {
            var phoneModel = this._repositoryPhone.GetAllPhones().FirstOrDefault(p => p.Id == id);
            if (phoneModel == null)
            {
                return NotFound(); // Return 404 Not Found if phone with given id is not found
            }
            return Ok(phoneModel);
        }

        [HttpPost]
        public IActionResult AddNewPhone([FromBody] PhoneModel phone)
        {
            this._repositoryPhone.AddPhone(phone);
            return Ok(phone); // Returns the added phone object as JSON
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePhone(int id)
        {
            if (this._repositoryPhone.DeletePhone(id))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPut("{id}")]
        public IActionResult UpdatePhone(int id, [FromBody] PhoneModel newmodel)
        {
            if (this._repositoryPhone.UpdatePhone(id, newmodel))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
