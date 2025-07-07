using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.ServiceTasks.Command.DeleteServiceTask;

public class DeleteServiceTaskCommandHandler : IRequestHandler<DeleteServiceTaskCommand, int>
{
    private readonly IServiceTaskRepository _serviceTaskRepository;
    private readonly IAppLogger<DeleteServiceTaskCommandHandler> _logger;
    private readonly IMapper _mapper;

    public DeleteServiceTaskCommandHandler(IServiceTaskRepository serviceTaskRepository, IAppLogger<DeleteServiceTaskCommandHandler> logger, IMapper mapper)
    {
        _serviceTaskRepository = serviceTaskRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(DeleteServiceTaskCommand request, CancellationToken cancellationToken)
    {
        // Validate ServiceTaskID
        _logger.LogInformation($"Validating ServiceTask with ID: {request.ServiceTaskID}");
        var serviceTask = await _serviceTaskRepository.GetByIdAsync(request.ServiceTaskID);
        if (serviceTask == null)
        {
            _logger.LogError($"ServiceTask with ID {request.ServiceTaskID} not found.");
            throw new EntityNotFoundException(typeof(ServiceTask).ToString(), "ID", request.ServiceTaskID.ToString());
        }

        // Delete ServiceTask
        _serviceTaskRepository.Delete(serviceTask);

        // Save Changes
        await _serviceTaskRepository.SaveChangesAsync();
        _logger.LogInformation($"ServiceTask with ID: {request.ServiceTaskID} deleted");

        return request.ServiceTaskID;
    }
}