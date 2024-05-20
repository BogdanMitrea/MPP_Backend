using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MPP_BackEnd.Repositories;
using System.Drawing;
using System;
using System.Net;
using System.Numerics;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
            var phoneModel = this._repositoryPhone.GetPhone(id);
            if (phoneModel == null)
            {
                return NotFound(); // Return 404 Not Found if phone with given id is not found
            }
            return Ok(phoneModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddNewPhone([FromBody] PhoneModel phone)
        {
            this._repositoryPhone.AddPhone(phone);
            return Ok(phone); // Returns the added phone object as JSON
        }

        [Authorize]
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

        [Authorize]
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

        [HttpGet("store/{storeid}")]
        public IActionResult GetAllPhonesById(int storeid)
        {
            var phones = this._repositoryPhone.GetPhonesByStore(storeid);
            //if (phones.Count == 0)
            //{
            //    return NotFound();
            //}
            return Ok(phones);
        }

        
        [HttpGet("{pageSize}/{page}")]
        public IActionResult GetPhoneModels(int page = 1, int pageSize = 5)
        {
            var userIdentity = HttpContext.User.Identity;
            var phoneModels = this._repositoryPhone.GetPagedPhones(page, pageSize);
            var totalPages = this._repositoryPhone.getPhonesCount() % pageSize==0 ? this._repositoryPhone.getPhonesCount() / pageSize : this._repositoryPhone.getPhonesCount() / pageSize+1;
            return Ok(new { Data = phoneModels, TotalPages = totalPages});
        }
    }
}
