using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.DTOs.ResponseDto;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CompanyResponseDto>>> GetAll()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyResponseDto>> GetById(Guid id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
                return NotFound();

            return Ok(company);
        }

        [HttpPost]
        public async Task<ActionResult<CompanyResponseDto>> Create([FromBody] Core.DTOs.Company.CreateCompanyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = await _companyService.CreateCompanyAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CompanyResponseDto>> Update(Guid id, [FromBody] Core.DTOs.Company.UpdateCompanyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = await _companyService.UpdateCompanyAsync(id, dto);
            if (company == null)
                return NotFound();

            return Ok(company);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _companyService.DeleteCompanyAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
