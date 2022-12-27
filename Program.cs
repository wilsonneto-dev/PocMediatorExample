using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("PocApp"));
builder.Services.AddScoped<CreateAccountUseCase>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkEF>();

//builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
//builder.Services.AddScoped<IDomainEventListener<AccountCreatedEvent>, SendEmailForAccountCreatedEventListener>();
//builder.Services.AddScoped<IDomainEventListener<AccountCreatedEvent>, CreateIntegrationEventCreatedEventListener>();
//builder.Services.AddScoped<IDomainEventListener<AccountSuspendedEvent>, OtherAccountSuspendedEventListener>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/accounts",
    async ([FromBody] CreateAccountInput input, [FromServices] CreateAccountUseCase useCase) 
        => await useCase.Handle(input))
    .WithOpenApi();

app.MapGet("/accounts",
    async (AppDbContext context) => await context.Accounts.ToListAsync())
    .WithOpenApi();

app.Run();
