using Microsoft.AspNetCore.Mvc;
using Kanini.LMP.Application.Services.Interfaces;

namespace Kanini.LMP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WhatsAppTestController : ControllerBase
{
    private readonly IWhatsAppService _whatsAppService;

    public WhatsAppTestController(IWhatsAppService whatsAppService)
    {
        _whatsAppService = whatsAppService;
    }

    [HttpGet("config-status")]
    public IActionResult GetConfigStatus()
    {
        var config = HttpContext.RequestServices.GetService<IConfiguration>();
        return Ok(new 
        {
            hasAccessToken = !string.IsNullOrEmpty(config?["WhatsApp:AccessToken"]),
            hasPhoneNumberId = !string.IsNullOrEmpty(config?["WhatsApp:PhoneNumberId"]),
            baseUrl = config?["WhatsApp:BaseUrl"],
            apiVersion = config?["WhatsApp:ApiVersion"]
        });
    }

    [HttpPost("send-test")]
    public async Task<IActionResult> SendTestMessage([FromBody] TestMessageRequest request)
    {
        var result = await _whatsAppService.SendWhatsAppMessageAsync(request.PhoneNumber, request.Message);
        
        if (result)
        {
            return Ok(new { success = true, message = "WhatsApp message sent successfully" });
        }
        
        return BadRequest(new { success = false, error = "Failed to send WhatsApp message. Check logs for details." });
    }
}

public class TestMessageRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}