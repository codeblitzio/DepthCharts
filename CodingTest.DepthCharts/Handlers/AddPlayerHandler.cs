using AutoMapper;
using CodingTest.DepthCharts.Entities;
using CodingTest.DepthCharts.Exceptions;
using CodingTest.DepthCharts.Messages;
using CodingTest.DepthCharts.Repositories;
using FluentValidation;
using MediatR;

namespace CodingTest.DepthCharts.Handlers;

public class AddPlayerHandler : IRequestHandler<AddPlayerCommand, Unit>
{
    readonly IRepository _repository;
    readonly IMapper _mapper;
    readonly IValidator<AddPlayerCommand> _validator;
    readonly ILogger<AddPlayerHandler> _logger;

    public AddPlayerHandler(IRepository repository, IMapper mapper, IValidator<AddPlayerCommand> validator, ILogger<AddPlayerHandler> logger)
	{
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
	}

    public async Task<Unit> Handle(AddPlayerCommand command, CancellationToken cancellationToken)
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
        if (players.Any(p => p.PlayerId == command.Player.PlayerId))
        {
            players.RemoveAll(p => p.PlayerId == command.Player.PlayerId);
        }

        // check the requested depth, if null or 0 then append
        // also check that the requested depth doesn't exceed the list length, if so then let's append
        if (command.Depth == null || command.Depth > players.Count)
        {
            // append to the end of list
            players.Add(_mapper.Map<Player>(command.Player));
        }
        else
        {
            // insert at requested depth
            players.Insert(command.Depth.Value, _mapper.Map<Player>(command.Player));
        }

        // assign the newly shuffled list to the position
        position.Players = players;

        // save the position to the repository
        await _repository.SavePositionAsync(position, cancellationToken);

        _logger.LogInformation("Handled AddPlayerCommand {@command}", command);

        // return a void result from mediatr
        return Unit.Value;
    }
}

