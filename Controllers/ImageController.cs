using Microsoft.AspNetCore.Mvc;

namespace Photon.ImageDb.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    [HttpGet("")]
    public string Get()
    {
        return "Hello World";
    }
}