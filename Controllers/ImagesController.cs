using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace Photon.ImageDb.Controllers;

[ApiController]
[Route("[controller]")]
public class ImagesController : ControllerBase
{
    [HttpPost("{session}")]
    public async Task<IActionResult> PostImage([FromRoute] string session, [FromForm] IFormFile? file)
    {
        if (file == null)
            return BadRequest();

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        var hashBytes = SHA1.HashData(fileBytes);
        var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        Directory.CreateDirectory(Path.Combine("Images", session));
        await using var fileStream = System.IO.File.OpenWrite(Path.Combine("Images", session, hashString));
        await file.CopyToAsync(fileStream);
        return Ok(new { Hash = hashString });
    }
    
    [HttpGet("{session}/{hash}")]
    public IActionResult GetImage([FromRoute] string session, [FromRoute] string hash)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Images", session, hash);
        if (!System.IO.File.Exists(path))
            return NotFound();
        return PhysicalFile(path, "image/jpeg");
    }
}