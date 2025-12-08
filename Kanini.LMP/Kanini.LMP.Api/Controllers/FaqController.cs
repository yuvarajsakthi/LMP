using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    public class FaqController : ControllerBase
    {
        private readonly IFaqService _faqService;

        public FaqController(IFaqService faqService)
        {
            _faqService = faqService;
        }

        [HttpPost(ApiConstants.Routes.FaqController.Create)]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<FaqDTO>>> CreateFaq([FromForm] FaqDTO faqDto)
        {
            try
            {
                var created = await _faqService.Add(faqDto);
                return Ok(ApiResponse<FaqDTO>.SuccessResponse(created, "FAQ created successfully"));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<FaqDTO>.ErrorResponse("Failed to create FAQ"));
            }
        }

        [HttpGet(ApiConstants.Routes.FaqController.GetAll)]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FaqDTO>>>> GetAllFaqs()
        {
            try
            {
                var faqs = await _faqService.GetAll();
                return Ok(ApiResponse<IEnumerable<FaqDTO>>.SuccessResponse(faqs));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<IEnumerable<FaqDTO>>.ErrorResponse("Failed to retrieve FAQs"));
            }
        }

        [HttpGet(ApiConstants.Routes.FaqController.GetById)]
        [Authorize(Roles = "Manager,Customer")]
        public async Task<ActionResult<ApiResponse<FaqDTO>>> GetFaqById(int id)
        {
            try
            {
                var faq = await _faqService.GetById(new IdDTO { Id = id });
                if (faq == null)
                    return NotFound(ApiResponse<FaqDTO>.ErrorResponse("FAQ not found"));

                return Ok(ApiResponse<FaqDTO>.SuccessResponse(faq));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<FaqDTO>.ErrorResponse("Failed to retrieve FAQ"));
            }
        }

        [HttpGet(ApiConstants.Routes.FaqController.GetByCustomer)]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FaqDTO>>>> GetFaqsByCustomerId(int customerId)
        {
            try
            {
                var faqs = await _faqService.GetByCustomerId(new IdDTO { Id = customerId });
                return Ok(ApiResponse<IEnumerable<FaqDTO>>.SuccessResponse(faqs));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<IEnumerable<FaqDTO>>.ErrorResponse("Failed to retrieve customer FAQs"));
            }
        }

        [HttpPut(ApiConstants.Routes.FaqController.Update)]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ApiResponse<FaqDTO>>> UpdateFaq(int id, [FromForm] FaqDTO faqDto)
        {
            try
            {
                faqDto.Id = id;
                var updated = await _faqService.Update(faqDto);
                return Ok(ApiResponse<FaqDTO>.SuccessResponse(updated, "FAQ updated successfully"));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<FaqDTO>.ErrorResponse("Failed to update FAQ"));
            }
        }

        [HttpDelete(ApiConstants.Routes.FaqController.Delete)]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFaq(int id)
        {
            try
            {
                await _faqService.Delete(new IdDTO { Id = id });
                return Ok(ApiResponse<object>.SuccessResponse(new { message = "FAQ deleted successfully" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to delete FAQ"));
            }
        }
    }
}