using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Versionamento.WebApi.V1.ViewModel;

namespace Versionamento.WebApi.V1.Controllers
{
    /// <summary>
    /// Representa o servico RestFul para Produto
    /// </summary>
    [ApiVersion("1.0")]
    [ApiVersion("0.9", Deprecated = true)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private static readonly List<Produto> Produtos = new List<Produto>()
        {
            new Produto(1, "Tomate", 4.99M, "" ),
            new Produto(2, "Pao France", 12.99M,  ""),
            new Produto(3, "Macarrao Pene", 7.21M , ""),
            new Produto(4, "Oleo Lubrificante", 23.45M, "" )
        };

        /// <summary>
        /// Obter lista de Produtos
        /// </summary>
        [HttpGet]
        [MapToApiVersion("1.0")]
        public ActionResult<IEnumerable<Produto>> ObterProdutos()
        {
            return Produtos.ToList();
        }

        /// <summary>
        /// Obter Produto por Id
        /// </summary>
        /// <param name="id">id de registro do Produto</param>
        [HttpGet("{id}")]
        [MapToApiVersion("0.9")]
        public IActionResult ObterProduto(long id)
        {
            return Ok(Produtos.FirstOrDefault(x => x.Id == id));
        }
    }
}