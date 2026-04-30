using Microsoft.AspNetCore.Mvc;
using Almoxarifado.Domain;
using Almoxarifado.API.Services.Interfaces;

namespace Almoxarifado.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _estoqueService;
        public EstoqueController(IEstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var produtos = await _estoqueService.ObterTodos();
            return Ok(produtos);
        }
    }
}