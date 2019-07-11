using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Versionamento.WebApi.V2.ViewModel;

namespace Versionamento.WebApi.V2.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Representa o servico RestFul para Produto
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private static readonly List<Produto> Produtos = new List<Produto>()
        {
            new Produto(1, "Tomate", 4.99M ),
            new Produto(2, "Pao France", 12.99M ),
            new Produto(3, "Macarrao Pene", 7.21M ),
            new Produto(4, "Oleo Lubrificante", 23.45M )
        };

        /// <summary>
        /// Obter Lista de Produto
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> ObterPessoaGetv1()
        {
            return this.Ok(Produtos.ToList());
        }

        /// <summary>
        /// Obter Produto por Id
        /// </summary>
        /// <param name="id">Id de registro do Produto</param>
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            return Ok(Produtos.FirstOrDefault(x => x.Id == id));
        }

        /// <summary>
        /// Incluir novo Produto
        /// </summary>
        /// <param name="produto">Produto que será incluido</param>
        [HttpPost]
        public IActionResult Post(Produto produto)
        {
            var entity = produto.Incluir(Produtos.Max(x => x.Id) + 1);
            Produtos.Add(entity);
            return CreatedAtAction("Post", new {produto.Id }, produto);
        }

        /// <summary>
        /// Editar Vigandor 
        /// </summary>
        /// <param name="id">Id do Vingador que deseja alterar</param>
        /// <param name="produto">Produto que será alterado</param>
        [HttpPut("{id}")]
        public void Put(long id, Produto produto)
        {
            var produtoAtual = Produtos.FirstOrDefault(x => x.Id == id);
            var index = Produtos.FindIndex(x => x.Id == id);
            Produtos[index] = produtoAtual?.Editar(produto);
        }

        /// <summary>
        /// Remove Produto
        /// </summary>
        /// <param name="id">Id do Produto que será removido</param>
        [HttpDelete("{id}")]
        public void Delete(long id)
        {
            Produtos.RemoveAt(Produtos.FindIndex(x => x.Id == id));
        }
    }
}