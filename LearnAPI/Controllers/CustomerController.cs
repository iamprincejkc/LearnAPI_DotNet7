using ClosedXML.Excel;
using LearnAPI.Model;
using LearnAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Data;

namespace LearnAPI.Controllers
{
    //[EnableCors("CorsPolicy")]
    //[DisableCors]
    [EnableRateLimiting("fixedWindow")]
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CustomerController(ICustomerService customerService, IWebHostEnvironment webHostEnvironment)
        {
            _customerService = customerService;
            _webHostEnvironment = webHostEnvironment;   
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await this._customerService.GetAll();
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                string excelPath = $"{GetFilePath()}\\CustomerInfo.xlsx";
                DataTable dt = new();
                dt.Columns.Add("Code", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Phone", typeof(string));
                dt.Columns.Add("CreditLimit", typeof(decimal));
                var data = await this._customerService.GetAll();
                if (data != null && data.Count > 0)
                {
                    data.ForEach(item =>
                    {
                        dt.Rows.Add(item.Code, item.Name, item.Email, item.Phone, item.CreditLimit);
                    });
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.AddWorksheet(dt, "Customer Info");
                    using(MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        if (System.IO.File.Exists(excelPath))
                        {
                            System.IO.File.Delete(excelPath);
                        }
                        wb.SaveAs(excelPath);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer.xlsx");
                    }

                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [NonAction]
        private string GetFilePath()
        {
            return _webHostEnvironment.WebRootPath + "\\Export";
        }
    }
}
