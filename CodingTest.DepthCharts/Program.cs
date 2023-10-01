using System.Net;
using System.Reflection;
using AutoMapper;
using CodingTest.DepthCharts.Exceptions;
using CodingTest.DepthCharts.Extensions;
using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Models;
using CodingTest.DepthCharts.Repositories;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Models = CodingTest.DepthCharts.Models;
using Serilog;
using CodingTest.DepthCharts.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("App"));

// configure hellang exception handling middleware
builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (ctx, ex) => false;
    options.ShouldLogUnhandledException = (context, ex, problem) => false;
    options.GetTraceId = ctx => null;

    options.Map<BadRequestException>(ex => new ProblemDetails
    {
        Title = "Bad Request",
        Status = (int)HttpStatusCode.BadRequest,
        Detail = ex.Message
    });

    options.Map<RepositoryException>(ex => new ProblemDetails
    {
        Title = "Internal Server Error",
        Status = (int)HttpStatusCode.InternalServerError,
        Detail = ex.Message
    });

    options.Map<Exception>(ex => new ProblemDetails
    {
        Title = "Internal Server Error",
        Status = (int)HttpStatusCode.InternalServerError,
        Detail = "Oops, something went wrong"
    });
});

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// configure serilog as it will provide structured logging
builder.Host.UseSerilog((hostContext, logger) => {
    logger.Configure();
});

builder.Services.AddSingleton<IRepository, MockRepository>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseProblemDetails();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

// use Minimal API endpoints

app.MapGet("/GetDepthChart", async (IMapper mapper, IMediator mediator, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    var result = await mediator.Send(new GetDepthChartQuery(), cancellationToken);
    return Results.Ok(mapper.Map<Models.GetDepthChartResponse>(result));
});

app.MapGet("/GetTrailingPlayers", async (IMapper mapper, IMediator mediator, ILogger<Program> logger, int playerId, string positionId, CancellationToken cancellationToken) =>
{
    var result = await mediator.Send(new GetTrailingPlayersQuery { PlayerId = playerId, PositionId = positionId }, cancellationToken);
    return Results.Ok(mapper.Map<Models.GetTrailingPlayersResponse>(result));
});

app.MapPost("/AddPlayer", async (IMapper mapper, IMediator mediator, ILogger<Program> logger, AddPlayerRequest request, CancellationToken cancellationToken) =>
{
    await mediator.Send(mapper.Map<AddPlayerCommand>(request), cancellationToken);
    return Results.Ok();
});

app.MapDelete("/RemovePlayer", async (IMapper mapper, IMediator mediator, ILogger<Program> logger, int playerId, string positionId, CancellationToken cancellationToken) =>
{
    await mediator.Send(new RemovePlayerCommand { PlayerId = playerId, PositionId = positionId }, cancellationToken);
    return Results.Ok();
});

// let's rock!

app.Run();
