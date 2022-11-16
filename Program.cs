
using System.Net;
using System.Reflection;
using FluentValidation;
using MediatR;
using mediatr_todos_mini;
using Microsoft.AspNetCore.Diagnostics;



var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly())
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthBehavior<,>))
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddValidatorsFromAssembly(typeof(PostTodoCommandValidator).Assembly);
builder.Services.AddSingleton<TodosService>();

#endregion


#region Swagger setup

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>{
    c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" });
});

#endregion

var app = builder.Build();

#region Swagger config

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

#endregion

#region Exception handling

app.UseExceptionHandler((errorApp)=>{
    errorApp.Run(async(context) =>
    {
        var feat= context.Features.Get<IExceptionHandlerFeature>();
        if (feat?.Error is ValidationException ve)
        {
            context.Response.StatusCode = (int) HttpStatusCode.UnprocessableEntity;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ve.Errors.Select(x => new
                { Property = x.PropertyName, Error = x.ErrorMessage }));
        }
            
        context.Response.StatusCode =  500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new{Erorr="Something went wrong!"});
    });
});

#endregion

#region Api mappings

app.MapPost("api/todos-post", async (IMediator mediator,PostTodoCommand todoCommand, CancellationToken cancellationToken) 
    => Results.Ok(await mediator.Send(todoCommand, cancellationToken)));

app.MapGet("api/todos-get", async (IMediator mediator, CancellationToken cancellationToken) 
    => Results.Ok(await mediator.Send(new GetTodosQuery(), cancellationToken)));

app.MapPost("api/register", async (IMediator mediator,UserRegistrationCommand registerCommand, 
    CancellationToken cancellationToken) =>{
    var created = await mediator.Send(registerCommand, cancellationToken);
    return !created ? Results.NotFound() : Results.Created("",null);
});


app.Run();

#endregion

