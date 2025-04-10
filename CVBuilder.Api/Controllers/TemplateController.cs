﻿using Amazon.S3.Model;
using Amazon.S3;
using CVBuilder.Core.Models;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Org.BouncyCastle.Math.EC.ECCurve;
using CVBuilder.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CVBuilder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }     
        [HttpGet("files")]
        [Authorize] 
        public async Task<IActionResult> GetTemplates()
        {
            var files = await _templateService.GetAllTamplatesAsync();
            return Ok(files);
        }
        [HttpGet("{index}")]
        public async Task<IActionResult> GetFile(int index)
        {
            var fileUrl = await _templateService.GetFileAsync(index);

            if (string.IsNullOrEmpty(fileUrl))
                return NotFound("File not found");

            return Ok(fileUrl);
        }


        //להפוך את זה ללפי ID
        //[HttpGet("first")]
        //public async Task<IActionResult> GetFirstFile()
        //{
        //    var fileKey = await _templateService.GetFirstFileAsync();
        //    if (fileKey == null)
        //        return NotFound("לא נמצאו קבצים ב-S3");

        //    return Ok(fileKey);
        //}

    }
}
