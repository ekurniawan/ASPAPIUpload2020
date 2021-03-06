﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SampleBackend.Interface;

namespace SampleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadPicController : ControllerBase
    {
        private IImageWriter _imageWritter;
        public UploadPicController(IImageWriter imageWritter)
        {
            _imageWritter = imageWritter;
        }

        public IActionResult GetData()
        {
            return Content("Hello World !");
        }

        [HttpPost]
        public async Task<IActionResult> PostUserImage(IFormFile file)
        {
            var result = await _imageWritter.UploadImage(file);
            return new ObjectResult(result);
        }

       
        [HttpGet("Download/{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           Path.Combine("wwwroot", "images"), filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}