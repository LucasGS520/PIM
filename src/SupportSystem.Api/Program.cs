// Program.cs - Ponto de entrada da aplicação ASP.NET Core

using SupportSystem.Infrastructure; // Extensões para registro da camada de infraestrutura (mapeadas em AddInfrastructure)

var builder = WebApplication.CreateBuilder(args);

// Registro de serviços no contêiner de DI (Dependency Injection)
// - Controllers: habilita o uso de controllers para APIs REST
// - EndpointsApiExplorer e SwaggerGen: habilitam geração de documentação OpenAPI/Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de serviços específicos da infraestrutura da aplicação.
// Implementação esperada em SupportSystem.Infrastructure como método de extensão.
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Pipeline de middleware
// Em ambiente de desenvolvimento expõe UI e JSON do Swagger para facilitar testes e documentação.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisições HTTP para HTTPS para segurança de transporte.
app.UseHttpsRedirection();

// Middleware de autorização (autenticação/autorização devem ser configuradas separadamente, se necessário).
app.UseAuthorization();

// Mapeia rotas para controllers registrados.
app.MapControllers();

// Executa a aplicação web (bloqueante até encerramento).
app.Run();
