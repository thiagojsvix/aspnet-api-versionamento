namespace Versionamento.WebApi.V2.ViewModel
{
    /// <summary>
    /// Entidade de Controle de Preço
    /// </summary>
    public class Produto
    {
        public Produto() { }

        public Produto(long id, string nome, decimal preco)
        {
            this.Id = id;
            this.Nome = nome;
            this.Preco = preco;
        }
        

        /// <summary>
        /// Identificação do Produto
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nome do Produto
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Habilidade do Produto
        /// </summary>
        public decimal Preco { get; set; }
        

        public Produto Incluir(long id ) => this.Editar(new Produto() { Id =  id});

        public Produto Editar(Produto entity)
        {
            this.Nome = entity.Nome;
            this.Preco = entity.Preco;
            return this;
        }
    }
}
