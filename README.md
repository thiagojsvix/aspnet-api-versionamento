**Em Constru��o**
![Version](src/Versionamento.WebApi/contents/version.png)
# Versionamento de API

Uma API � um *contrato entre o servi�o e o cliente*, e como tal, n�o deve ser alterada sem notificar o cliente e dar-lhe o prazo necess�rio para adequa��o. N�o estou dizendo que sua API n�o possa evoluir livremente, o que estou propondo � que voc� de ao seu cliente a possibilidade de planejar e realizar a atualiza��o.

Quando precisamos atualizar o contrato de nossa API, e principalmente quando nossa API � de uso publico, normalmente n�o ser� poss�vel for�ar todos os clientes a atualizarem para o novo contrato da API. Geralmente, � necess�rio implantar `novas vers�es` de um servi�o de forma incremental de maneira que vers�es novas e antigas de um contrato de servi�o estejam em execu��o simultaneamente. Portanto, � importante ter uma estrat�gia para o controle de vers�o do servi�o.


Quando as altera��es na API forem pequenas, como adicionar atributos ou par�metros � sua API, os clientes que usam uma API mais antiga devem mudar e trabalhar com a nova vers�o do servi�o. Talvez seja poss�vel fornecer valores padr�o para quaisquer atributos ausentes que sejam necess�rios, e os clientes talvez podem ignorar quaisquer atributos de resposta extra.

Sempre que pudermos manter a mesma vers�o de nossa API � recomendado, pois alterar a vers�o inclui manter a vers�o recente e anterior, novos testes, e compatibilidade de todos os cliente. Veja o que [Martin Fowler](https://martinfowler.com/articles/enterpriseREST.html) diz sobre o assunto.
> The problem is that versioning can significantly complicate understanding, testing, and troubleshooting a system. As soon as you have multiple incompatible versions of the same service used by multiple consumers, developers have to maintain bug fixes in all supported versions. If they are maintained in a single codebase, developers risk breaking an old version by adding a new feature to a new version simply because of shared code pathways. If the versions are independently deployed, the operational footprint becomes more complex to monitor and support.

No entanto, �s vezes, � necess�rio fazer altera��es importantes e incompat�veis em uma API de servi�o. Como talvez n�o seja poss�vel for�ar servi�os ou aplicativos cliente a serem atualizados imediatamente para a nova vers�o, um servi�o deve dar suporte a vers�es mais antigas da API por algum per�odo. Se voc� estiver usando um mecanismo baseado em HTTP como REST, uma abordagem dever� inserir o n�mero de vers�o da API na URL ou no cabe�alho HTTP. Em seguida, � poss�vel decidir entre implementar ambas as vers�es do servi�o simultaneamente dentro da mesma inst�ncia de servi�o ou implantar inst�ncias diferentes que lidam com uma vers�o da API. Uma boa abordagem para isso � o [padr�o mediador](https:/en.wikipedia.org/wiki/Mediator_pattern) (por exemplo, [biblioteca MediatR](https://github.com/jbogard/MediatR)) para desacoplar as diferentes vers�es de implementa��o em manipuladores independentes.


## Configurando o versionamento

Esse projeto tem como abordagem o [versionamento sem�ntico](https://semver.org/). Iremos utilizar o pacote de versionamento muito simples de usar criado pelo time do [ASP.NET Core](https://github.com/microsoft/aspnet-api-versioning). Voc� precisar� instalar o pacote nuget no console do gerenciador de pacotes..

```csharp
Install-Package Microsoft.AspNetCore.Mvc.Versioning
```

No m�todo `ConfigureServices` do seu `startup.cs`, voc� precisa adicionar os servi�os de versionamento de API invocando a extens�o `AddApiVersionHandler`

```csharp
public static IServiceCollection AddApiVersionHandler(this IServiceCollection services)
{
    services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1, 0);
    });
    services.AddVersionedApiExplorer(
        options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

    return services;
}
```

O sinalizador `ReportApiVersions` � opcional, mas pode ser �til. Ele permite que a API retorne vers�es em um cabe�alho de resposta. Ao chamar uma API com esse sinalizador, voc� ver� algo como o seguinte.

![Report A P I Versions](src/Versionamento.WebApi/contents/ReportAPIVersions.png)

`AssumeDefaultVersionWhenUnspecified` (bastante interessante) tamb�m pode ser �til, especialmente ao migrar uma API para controle de vers�o. Sem isso, voc� quebrar� todos os clientes existentes que n�o estejam especificando uma vers�o da API. Se voc� est� recebendo o erro *Uma vers�o da API � necess�ria, mas n�o foi especificada.*, Este ser� o motivo.

`DefaultApiVersion` tamb�m n�o � necess�rio neste caso porque o padr�o � 1.0. Mas achei �til inclu�-lo como um lembrete de que voc� pode *atualizar automaticamente* qualquer cliente que n�o esteja especificando uma vers�o padr�o da API para o mais recente. H� pr�s e contras para fazer isso, mas a op��o est� l�, se voc� quiser.

`GroupNameFormat`

`SubstituteApiVersionInUrl`

## Refer�ncia

[Microsoft REST API Guidelines](https://github.com/microsoft/api-guidelines/blob/vNext/Guidelines.md)  
[Mediator Pattern](https:/en.wikipedia.org/wiki/Mediator_pattern)  
[Semantic Version](https://semver.org/)  
[Enterprise Integration Using REST](https://martinfowler.com/articles/enterpriseREST.html)  