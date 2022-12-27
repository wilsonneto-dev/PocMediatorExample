using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PocMediatorExample.Mediator;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("PocApp"));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkEF>();
// builder.Services.AddScoped<CreateAccountUseCase>();

//builder.Services.AddScoped<IHandler<CreateAccountInput, CreateAccountOutput>, CreateAccountUseCase>();
//builder.Services.AddScoped<IMediator>(provider => new Mediator(provider,
//    new Dictionary<Type, Type> { 
//        { typeof(CreateAccountInput), typeof(IHandler<CreateAccountInput, CreateAccountOutput>) } }));

builder.Services.AddMediator(ServiceLifetime.Scoped, typeof(CreateAccountUseCase));

//builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
//builder.Services.AddScoped<IDomainEventListener<AccountCreatedEvent>, SendEmailForAccountCreatedEventListener>();
//builder.Services.AddScoped<IDomainEventListener<AccountCreatedEvent>, CreateIntegrationEventCreatedEventListener>();
//builder.Services.AddScoped<IDomainEventListener<AccountSuspendedEvent>, OtherAccountSuspendedEventListener>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapPost("/accounts",
    async ([FromBody] CreateAccountInput input, [FromServices] IMediator mediator) 
        => await mediator.Send(input))
    .WithOpenApi();

app.MapGet("/accounts",
    async (AppDbContext context) => await context.Accounts.ToListAsync())
    .WithOpenApi();

app.Run();
