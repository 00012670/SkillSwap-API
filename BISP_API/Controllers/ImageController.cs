﻿using BISP_API.Context;
using BISP_API.Helper;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController: ControllerBase
    {
        private readonly BISPdbContext _dbContext;
        private readonly IWebHostEnvironment hostingEnv;

        public ImageController(BISPdbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            hostingEnv = environment;

        }
        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string imgId)
        {
            string Imageurl = string.Empty;
            string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            try
            {
                string Filepath = GetFilePath(imgId);
                string imagepath = Filepath + "\\" + imgId + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    Imageurl = hosturl + "/Uploads/SKill/" + imgId + "/" + imgId + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
            }
            return Ok(Imageurl);
        }



        [HttpPost("UploadImage")]
        public async Task<ActionResult> UploadImage(IFormFile formFile, string imgId)
        {
            APIResponse response = new();
            try
            {
                string Filepath = GetFilePath(imgId);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }
                string imagepath = Filepath + "\\" + imgId + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using FileStream stream = System.IO.File.Create(imagepath);
                await formFile.CopyToAsync(stream);
                response.ResponseCode = 200;
                response.Result = "pass";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Ok(response);
        }


        [HttpDelete("RemoveImage")]
        public async Task<IActionResult> RemoveImage(string imgId)
        {
            try
            {
                string Filepath = GetFilePath(imgId);
                string Imagepath = Filepath + "\\" + imgId + ".png";
                if (System.IO.File.Exists(Imagepath))
                {
                    System.IO.File.Delete(Imagepath);
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [HttpGet("GetDBImage")]
        public async Task<IActionResult> GetDBImage(string imgcode)
        {
            List<string> Imageurl = new();
            try
            {
                var _productimage = this._dbContext.Images.Where(item => item.Imgcode == imgcode).ToList();
                if (_productimage != null && _productimage.Count > 0)
                {
                    _productimage.ForEach(item =>
                    {
                        Imageurl.Add(Convert.ToBase64String(item.Img));
                    });
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception)
            {
            }
            return Ok(Imageurl);

        }


        [HttpPut("DBUploadImage")]
        public async Task<IActionResult> DBMUploadImage(IFormFileCollection filecollection, string imgoCode)
        {
            APIResponse response = new();
            int passcount = 0; int errorcount = 0;
            try
            {
                foreach (var file in filecollection)
                {
                    using MemoryStream stream = new();
                    await file.CopyToAsync(stream);
                    this._dbContext.Images.Add(new Image()
                    {
                        Imgcode = imgoCode,
                        Img = stream.ToArray()
                    });
                    await this._dbContext.SaveChangesAsync();
                    passcount++;
                }


            }
            catch (Exception ex)
            {
                errorcount++;
                response.Message = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = passcount + " Files uploaded & " + errorcount + " files failed";
            return Ok(response);
        }



        [NonAction]
        public string GetFilePath(string imgId)
        {
            return this.hostingEnv.WebRootPath + "\\Uploads\\Skill\\" + imgId;
        }
    }
}