using LearnAPI.DB;
using LearnAPI.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LearnAPIDbContext _context;
        public ProductController(IWebHostEnvironment webHostEnvironment, LearnAPIDbContext context)
        {

            _webHostEnvironment = webHostEnvironment;
            _context = context;

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

        [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection fileCollection, int propductId)
        {
            APIResponse response = new APIResponse();
            int passCount = 0, errorCount = 0;
            try
            {

                foreach (var file in fileCollection)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        _context.TblProductImages.Add(new DB.Models.TblProductImage()
                        {
                            Productid = propductId,
                            Productimage = stream.ToArray()
                        });
                        await _context.SaveChangesAsync();
                        passCount++;
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

                List<string> imageExtensions = new List<string> { "jpg", "png" };

                string? validExtension = imageExtensions.FirstOrDefault(extension =>
                    System.IO.File.Exists(Path.Combine(filePath, $"{productCode}.{extension}")));

                if (!string.IsNullOrEmpty(validExtension))
                {
                    imageUrl = $"{hostUrl}Upload/Product/{productCode}/{productCode}.{validExtension}";
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
        [HttpGet("DBGetMultiImage")]
        public async Task<IActionResult> DBGetMultiImage(int productId)
        {
            List<string> imageUrl = new();
            string hostUrl = $"{Request.Scheme}://{Request.Host}//{Request.PathBase}";
            try
            {
                var _productImage = _context.TblProductImages.Where(item => item.Productid == productId).ToList();
                if (_productImage.Count > 0)
                {
                    _productImage.ForEach(item =>
                    {
                        imageUrl.Add(Convert.ToBase64String(item.Productimage));
                    });
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

        [HttpGet("DownloadImage")]
        public async Task<IActionResult> DownloadImage(string productCode)
        {
            string imageUrl = string.Empty;
            string hostUrl = $"{Request.Scheme}://{Request.Host}//{Request.PathBase}";
            try
            {
                string imagePath = GetFilePath(productCode);

                List<string> imageExtensions = new List<string> { "jpg", "png" };

                string? validExtension = imageExtensions.FirstOrDefault(extension =>
                    System.IO.File.Exists(Path.Combine(imagePath, $"{productCode}.{extension}")));

                if (!string.IsNullOrEmpty(validExtension))
                {
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(Path.Combine(imagePath, $"{productCode}.{validExtension}"), FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;

                    string contentType = validExtension == "jpg" ? "image/jpg" : "image/png";
                    return File(stream, contentType, $"{productCode}.{validExtension}");
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
        }
        [HttpGet("DBDownloadImage")]
        public async Task<IActionResult> DBDownloadImage(int productId)
        {
            string imageUrl = string.Empty;
            string hostUrl = $"{Request.Scheme}://{Request.Host}//{Request.PathBase}";
            try
            {
                var _productImage = await _context.TblProductImages.FirstOrDefaultAsync(item => item.Productid == productId);


                if (_productImage!=null)
                {
                    return File(_productImage.Productimage, "iamge/png", $"{productId}.png");
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
        }

        [HttpGet("RemoveImage")]
        public async Task<IActionResult> RemoveImage(string productCode)
        {
            string imageUrl = string.Empty;
            string hostUrl = $"{Request.Scheme}://{Request.Host}//{Request.PathBase}";
            try
            {
                string imagePath = GetFilePath(productCode);

                List<string> imageExtensions = new List<string> { "jpg", "png" };

                string? validExtension = imageExtensions.FirstOrDefault(extension =>
                    System.IO.File.Exists(Path.Combine(imagePath, $"{productCode}.{extension}")));

                if (!string.IsNullOrEmpty(validExtension))
                {
                    System.IO.File.Delete(Path.Combine(imagePath, $"{productCode}.{validExtension}"));
                    return Ok("Image Successfully Deleted");
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
        }

        [HttpGet("MultiRemoveImage")]
        public async Task<IActionResult> MultiRemoveImage(string productCode)
        {
            string imageUrl = string.Empty;
            string hostUrl = $"{Request.Scheme}://{Request.Host}//{Request.PathBase}";
            try
            {
                string imagePath = GetFilePath(productCode);
                if (System.IO.Directory.Exists(imagePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(imagePath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (var file in fileInfos)
                    {

                        file.Delete();
                    }
                    return Ok("Image Successfully Deleted");
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
        }



        [NonAction]
        private string GetFilePath(string productCode)
        {
            return _webHostEnvironment.WebRootPath + "\\Upload\\Product\\" + productCode;
        }
    }
}
