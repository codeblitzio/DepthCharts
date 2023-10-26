using AutoMapper;
using CodingTest.DepthCharts.Exceptions;
using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Repositories;
using FluentValidation;
using MediatR;

namespace CodingTest.DepthCharts.Handlers;

public class RemovePlayerHandler : IRequestHandler<RemovePlayerCommand>
{
    readonly IRepository _repository;
    readonly IMapper _mapper;
    readonly IValidator<RemovePlayerCommand> _validator;
    readonly ILogger<RemovePlayerHandler> _logger;

    public RemovePlayerHandler(IRepository repository, IMapper mapper, IValidator<RemovePlayerCommand> validator,
        ILogger<RemovePlayerHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task Handle(RemovePlayerCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling AddPlayerCommand {@command}", command);

        // use fluent validations

        var result = await _validator.ValidateAsync(command, cancellationToken);

        if (!result.IsValid)
        {
            // the helland middleware will return a problem details response summarising the failure
            throw new BadRequestException($"{result.ToString(" ~ ")}");
        }

        // retrieve the position associated with the request

        var position = await _repository.GetPositionAsync(command.PositionId, cancellationToken);

        var players = position.Players.ToList();

        // remove the player if they already exist in this position
        if (players.Any(p => p.PlayerId == command.PlayerId))
        {
            players.RemoveAll(p => p.PlayerId == command.PlayerId);
        }
        else
        {
            throw new BadRequestException("The Player was not found");
        }

        // assign the newly shuffled list to the position
        position.Players = players;

        // save the position to the repository
        await _repository.SavePositionAsync(position, cancellationToken);

        _logger.LogInformation("Handled RemovePlayerCommand {@command}", command);

        // return a void result from mediatr
        return;
    }
}

