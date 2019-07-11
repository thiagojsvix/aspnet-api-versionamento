![Version](src/Versionamento.WebApi/contents/version.png)
# Versionamento de API

Uma API � um *contrato entre o servi�o e o cliente*, e como tal, n�o deve ser alterada sem notificar o cliente e dar-lhe o prazo necess�rio para adequa��o. N�o estou dizendo que sua API n�o possa evoluir livremente, o que estou propondo � que voc� de ao seu cliente a possibilidade de planejar e realizar a atualiza��o.

Quando precisamos atualizar o contrato de nossa API, e principalmente quando nossa API � de uso publico, normalmente n�o ser� poss�vel for�ar todos os clientes a atualizarem para o novo contrato da API. Geralmente, � necess�rio implantar `novas vers�es` de um servi�o de forma incremental, de maneira que vers�es novas e antigas de um contrato de servi�o estejam em execu��o simultaneamente. Portanto, � importante ter uma estrat�gia para o controle de vers�o do servi�o.


Quando as altera��es na API forem pequenas, como adicionar atributos ou par�metros � sua API, os clientes que usam uma API mais antiga devem mudar e trabalhar com a nova vers�o do servi�o. Talvez seja poss�vel fornecer valores padr�o para quaisquer atributos ausentes que sejam necess�rios, e os clientes talvez possam ignorar quaisquer atributos de resposta extra.

Sempre que pudermos manter a compatibilidade de nossa API � recomendado, pois quebrar a compatibilidade inclui manter a vers�o recente e anterior, novos testes, e compatibilidade de todos os cliente. Veja o que [Martin Fowler](https://martinfowler.com/articles/enterpriseREST.html) diz sobre o assunto.
> The problem is that versioning can significantly complicate understanding, testing, and troubleshooting a system. As soon as you have multiple incompatible versions of the same service used by multiple consumers, developers have to maintain bug fixes in all supported versions. If they are maintained in a single codebase, developers risk breaking an old version by adding a new feature to a new version simply because of shared code pathways. If the versions are independently deployed, the operational footprint becomes more complex to monitor and support.

No entanto, �s vezes, � necess�rio fazer altera��es importantes e incompat�veis em uma API de servi�o. Como talvez n�o seja poss�vel for�ar servi�os ou aplicativos cliente a serem atualizados imediatamente para a nova vers�o, um servi�o deve dar suporte a vers�es mais antigas da API por algum per�odo. Se voc� estiver usando um mecanismo baseado em HTTP como REST, uma abordagem dever� inserir o n�mero de vers�o da API na URL ou no cabe�alho HTTP. Em seguida, � poss�vel decidir entre implementar ambas as vers�es do servi�o simultaneamente dentro da mesma inst�ncia de servi�o ou implantar inst�ncias diferentes que lidam com uma vers�o da API. Uma boa abordagem para isso � o [padr�o mediador](https:/en.wikipedia.org/wiki/Mediator_pattern) (por exemplo, [biblioteca MediatR](https://github.com/jbogard/MediatR)) para desacoplar as diferentes vers�es de implementa��o em manipuladores independentes.


## Configurando o versionamento

Esse projeto tem como abordagem o [versionamento sem�ntico](https://semver.org/). Iremos utilizar o pacote de versionamento muito simples de usar criado pelo time do [ASP.NET Core](https://github.com/microsoft/aspnet-api-versioning). Voc� precisar� instalar o pacote nuget no console do gerenciador de pacotes..

```csharp
Install-Package Microsoft.AspNetCore.Mvc.Versioning
```

No m�todo `ConfigureServices` do seu `startup.cs`, voc� precisa adicionar os servi�os de versionamento de API invocando a extens�o `AddApiVersionHandler`

```csharp
public static class ApiVersionAddExtensionHandler
{
    public static IServiceCollection AddApiVersionHandler(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.Conventions.Controller<V2.Controllers.ProdutoController>().HasApiVersion(new ApiVersion(2, 0));
        });
        services.AddVersionedApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
```

`ReportApiVersions` � opcional, mas pode ser �til. Ele permite que a API retorne vers�es em um cabe�alho de resposta. Ao chamar uma API com esse sinalizador, voc� ver� algo como o seguinte.

![Report A P I Versions](src/Versionamento.WebApi/contents/ReportAPIVersions.png)

`AssumeDefaultVersionWhenUnspecified` (bastante interessante) tamb�m pode ser �til, especialmente ao migrar uma API para controle de vers�o. Sem isso, voc� quebrar� todos os clientes existentes que n�o estejam especificando uma vers�o da API. Se voc� est� recebendo o erro *Uma vers�o da API � necess�ria, mas n�o foi especificada.*, Este ser� o motivo.

`DefaultApiVersion` tamb�m n�o � necess�rio neste caso porque o padr�o � 1.0. Mas achei �til inclu�-lo como um lembrete de que voc� pode *atualizar automaticamente* qualquer cliente que n�o esteja especificando uma vers�o padr�o da API para o mais recente. H� pr�s e contras para fazer isso, mas a op��o est� l�, se voc� quiser.

`GroupNameFormat` define o formato de API que ser� utilizado. Como est� configurado, indica de a vers�o ser�: v[MAJOR].[MINOR].[PATCH], veja [Semantic Version](https://semver.org/) para mais informa��es.

`SubstituteApiVersionInUrl` s� � necess�rio definir essa op��o quando o versionamento for por `URL`. Essa op��o tamb�m � muito interessante quando usamos ferramentadas de documenta��o como o [OpenAPI Specification](https://github.com/OAI/OpenAPI-Specification)

## Associando Vers�o usando conven��o
Se voc� tiver muitas vers�es de API e quizer manter o versionamento e um lugar centralizado para facilitar a manunte��o, voc� pode abrir m�o de colocar o atributo `ApiVersion` em todos os controladores e utlizar a configura��o descrita no c�digo abaixo.

![URl Vesion](src/Versionamento.WebApi/contents/Conventions.png)

Como pode ser observado na **linha 15** estamos definido que para o `Controller Produto` do namespace `V2` a vers�o **2.0**.

Observem abaixo que o `Controller Produto` possu� apenas o atributo de rota e n�o as especifica��o de vers�o.
  
![URl Vesion](src/Versionamento.WebApi/contents/Controller_Produto_v2.png)

E como pode ser observado na documenta��o do [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore), todas as rotas para cada um dos servi�os est�o com a vers�o 2.0.

![URl Vesion](src/Versionamento.WebApi/contents/Swagger_Produto_v2.png)

## API Neutra

Em muitos caso possuiamos APIs apenas para manter o status da aplica��o, um belo exemplo ser� o monitoramento de sa�da do servi�o. Para esse tipo de API n�o faz muito sentido mantermos o versionamento, j� que dificilmente essas APIs sofrer�o mudan�as que quebre o contrato. 

A forma de implementar a configura��o da API Neutra � atrav�s do atributo `ApiVersionNeutral`

```csharp
[ApiVersionNeutral]
[Route("api/[controller]")]
public class StatusCodeController : ControllerBase
{
    [HttpGet]
    public IActionResult GetStatusCode() => this.Ok();
}
```

Conforme pode ser observado no trecho de c�digo acima, foi adicionado o atributo `ApiVersionNeutral` e na rota n�o exite nenhuma informa��o de vers�o, logo a url de acesso a esse servi�o seria conforme demostrado na image abaixo.

![URl Vesion](src/Versionamento.WebApi/contents/ApiVersionNeutral.png)

Uma considera��o importante. Voc� deve ter percebido que citamos o termo `Monitoramento de Sa�de do Servi�o` mas n�o abordamos o assunto. Como essa assunto � bem amplo e muit�ssimo importante, faz sentido criarmos um artigo apenas para ele.

## O uso do atributo ApiVersion

J� foi mencionado diversas vezes o atributo `ApiVersion` inclusive j� falamos at� como evitar o seu uso, mas agora iremos explicar como utilizado.

Esse atributo � muito simples, possui um parametro `String` onde informamos a vers�o que ser� associada a API e o parametro opcional `Deprecated` que quando definido com `True` indicamos a inten��o de descontinuar tal API.

Em um controlador podemos ter v�rios atributos `ApiVersion` indicado que temos no mesmo controlador diferente vers�o de API. Quando fazemos o uso de varios atributos `ApiVersion` faz se necess�rio usar o atributo `MapToApiVersion` no m�todo especificio.

Como pode ser observado abaixo cada um dos m�todos foi decorado com um atributo `MapToApiVersion`, indicando que cada um deles � responsavel por uma vers�o especifica da API

```csharp
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
```

# Conclus�o

Embora � recomendado tentar mantermos o m�ximo poss�vel a compatibilidade do contrato da API, em alguns cen�rios fica inviavel e para isso que devemos utilizar o versionamento. Com ele conseguimos garantir a continuidade do servi�o prestado a nossos cliente e ao mesmo tempo a evolu��o de nossos servi�o.


## Refer�ncia

[Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)  
[Semantic Version](https://semver.org/)  
[Mediator Pattern](https://en.wikipedia.org/wiki/Mediator_pattern)  
[OpenAPI Specification](https://github.com/OAI/OpenAPI-Specification)  
[Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines/blob/vNext/Guidelines.md)  
[Enterprise Integration Using REST](https://martinfowler.com/articles/enterpriseREST.html)  
[API design: Which version of versioning is right for you?](https://cloud.google.com/blog/products/gcp/api-design-which-version-of-versioning-is-right-for-you)  
[The Web API Checklist -- 43 Things To Think About When Designing, Testing, and Releasing your API](https://mathieu.fenniak.net/the-api-checklist/)
