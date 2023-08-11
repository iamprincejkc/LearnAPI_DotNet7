using LearnAPI.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IWebHostEnvironment webHostEnvironment)
        {

            _webHostEnvironment = webHostEnvironment;

        }
        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string productCode)
        {
            APIResponse response = new APIResponse();
            try
            {
                string filePath = GetFilePath(productCode);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string[] permittedExtensions = { ".jpg", ".png" };
                var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                {
                    return BadRequest("Invalid File");
                }

                string imagePath = filePath + "\\" + productCode + System.IO.Path.GetExtension(formFile.FileName);
                if (!System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await formFile.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Message = "Success";
                    response.Data = "pass";
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 500;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection fileCollection, string productCode)
        {
            APIResponse response = new APIResponse();
            int passCount = 0, errorCount = 0;
            try
            {
                string filePath = GetFilePath(productCode);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                foreach (var file in fileCollection)
                {
                    string[] permittedExtensions = { ".jpg", ".png" };
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                    {
                        errorCount++;
                        continue;
                    }


                    string imagePath = filePath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    try
                    {
                        using (FileStream stream = System.IO.File.Create(imagePath))
                        {
                            await file.CopyToAsync(stream);
                            passCount++;
                        }
                    }
                    catch (Exception)
                    {
                        errorCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                errorCount++;
            }
            response.ResponseCode = 200;
            response.Message = "Success";
            response.Data = $"Files Uploaded {passCount} & Files Failed {errorCount}";

            return Ok(response);
        }

        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string productCode)
        {
            string imageUrl = string.Empty;
            string hostUrl = $"{Request.Scheme}://{Request.Host}//{Request.PathBase}";
            try
            {
                string filePath = GetFilePath(productCode);
                string imagePath = filePath + "\\" + productCode;

                if (System.IO.File.Exists(imagePath + ".jpg"))
                {
                    imageUrl = hostUrl + "Upload/Product/" + productCode + "/" + productCode + ".jpg";
                }
                else if (System.IO.File.Exists(imagePath + ".png"))
                {
                    imageUrl = hostUrl + "Upload/Product/" + productCode + "/" + productCode + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok(imageUrl);
        }


        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string productCode)
        {
            List<string> imageUrl = new();
            string hostUrl = $"{Request.Scheme}://{Request.Host}//{Request.PathBase}";
            try
            {
                string filePath = GetFilePath(productCode);

                if (System.IO.Directory.Exists(filePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (var file in fileInfos)
                    {
                        string fileName = file.Name;
                        string imagePath = filePath + "\\" + fileName;
                        if (System.IO.File.Exists(imagePath))
                        {
                            string _imageUrl = hostUrl + "Upload/Product/" + productCode + "/" + fileName;
                            imageUrl.Add(_imageUrl);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok(imageUrl);
        }

        [NonAction]
        private string GetFilePath(string productCode)
        {
            return _webHostEnvironment.WebRootPath + "\\Upload\\Product\\" + productCode;
        }
    }
}
