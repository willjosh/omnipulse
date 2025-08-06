using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceReminders.Command.AddServiceReminderToExistingWorkOrder;

using Domain.Entities;

using FluentValidation;

using Moq;

namespace Application.Test.ServiceReminders.CommandTest.AddServiceReminderToExistingWorkOrder;

public class AddServiceReminderToExistingWorkOrderCommandHandlerTest
{
    private readonly Mock<IServiceReminderRepository> _mockServiceReminderRepository;
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IValidator<AddServiceReminderToExistingWorkOrderCommand>> _mockValidator;
    private readonly Mock<IAppLogger<AddServiceReminderToExistingWorkOrderCommandHandler>> _mockLogger;
    private readonly AddServiceReminderToExistingWorkOrderCommandHandler _handler;

    public AddServiceReminderToExistingWorkOrderCommandHandlerTest()
    {
        _mockServiceReminderRepository = new Mock<IServiceReminderRepository>();
        _mockWorkOrderRepository = new Mock<IWorkOrderRepository>();
        _mockValidator = new Mock<IValidator<AddServiceReminderToExistingWorkOrderCommand>>();
        _mockLogger = new Mock<IAppLogger<AddServiceReminderToExistingWorkOrderCommandHandler>>();

        _handler = new AddServiceReminderToExistingWorkOrderCommandHandler(
            _mockServiceReminderRepository.Object,
            _mockWorkOrderRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenServiceReminderNotFound()
    {
        // Arrange
        var command = new AddServiceReminderToExistingWorkOrderCommand(
            ServiceReminderID: 999,
            WorkOrderID: 1);

        _mockValidator.Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockServiceReminderRepository.Setup(x => x.GetServiceReminderWithDetailsAsync(999))
            .ReturnsAsync((ServiceReminder?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}