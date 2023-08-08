using LearnAPI.Model;
using LearnAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LearnAPI.Controllers
{
    //[EnableCors("CorsPolicy")]
    //[DisableCors]
    [EnableRateLimiting("fixedWindow")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data  = await this._customerService.GetAll();
            if (data == null) 
                return NotFound();
            return Ok(data);
        }
        [DisableRateLimiting]
        [HttpGet]
        public async Task<IActionResult> GetByCode(int code)
        {
            var data = await this._customerService.GetByCode(code);
            if (data == null)
                return NotFound();
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerModel customerModel)
        {
            var data = await this._customerService.Create(customerModel);
            if (data == null)
                return NotFound();
            return Ok(data);
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveByCode(int code)
        {
            var data = await this._customerService.RemoveByCode(code);
            if (data == null)
                return NotFound();
            return Ok(data);
        }
        [HttpPut]
        public async Task<IActionResult> Update(CustomerModel customerModel)
        {
            var data = await this._customerService.Update(customerModel);
            if (data == null)
                return NotFound();
            return Ok(data);
        }
    }
}
