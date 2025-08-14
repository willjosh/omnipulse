using Application.Features.WorkOrderLineItem.Models;
using Application.Features.WorkOrders.Command.CompleteWorkOrder;
using Application.Features.WorkOrders.Command.CreateWorkOrder;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.WorkOrders;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(WorkOrder))]
public class CompleteWorkOrderIntegrationTests : BaseIntegrationTest
{
    public CompleteWorkOrderIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_CreateAndComplete_WorkOrder_With_Inventory_Deducted()
    {
        // ===== Arrange =====
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var technicianId = await CreateTechnicianAsync();

        // Inventory setup: item, location, inventory stock
        var itemId = await CreateInventoryItemAsync();
        var locationId = await CreateInventoryItemLocationAsync();

        const int initialQty = 10;
        var inventoryId = await EnsureInventoryAsync(itemId, locationId, initialQty);

        var serviceTaskId = await CreateServiceTaskAsync();

        // Work order with one ITEM line that will deduct inventory upon completion
        const int usedQty = 3;
        var schedStart = DateTime.UtcNow.AddMinutes(5);
        var actualStart = schedStart.AddMinutes(10);
        var schedComplete = schedStart.AddDays(1);
        var actualComplete = schedComplete.AddMinutes(30);

        var workOrderId = await Sender.Send(new CreateWorkOrderCommand(
            VehicleID: vehicleId,
            AssignedToUserID: technicianId.ToString(),
            Title: $"WO {Faker.Random.AlphaNumeric(6)}",
            Description: "Change oil and filter",
            WorkOrderType: WorkTypeEnum.SCHEDULED,
            PriorityLevel: PriorityLevelEnum.MEDIUM,
            Status: WorkOrderStatusEnum.CREATED,
            ScheduledStartDate: schedStart,
            ActualStartDate: actualStart,
            ScheduledCompletionDate: schedComplete,
            ActualCompletionDate: actualComplete,
            StartOdometer: 12_345,
            EndOdometer: 12_400,
            IssueIdList: [],
            WorkOrderLineItems:
            [
                new CreateWorkOrderLineItemDTO
                {
                    InventoryItemID = itemId,
                    ServiceTaskID = serviceTaskId,
                    AssignedToUserID = technicianId.ToString(),
                    ItemType = LineItemTypeEnum.ITEM,
                    Description = "Oil Filter",
                    Quantity = usedQty,
                    UnitPrice = 25.00m,
                    HourlyRate = null,
                    LaborHours = null
                }
            ]
        ));

        // ===== Act =====
        var completedId = await Sender.Send(new CompleteWorkOrderCommand(workOrderId));

        // ===== Assert =====
        completedId.Should().Be(workOrderId);

        var wo = await DbContext.WorkOrders.AsNoTracking().FirstAsync(w => w.ID == workOrderId);
        wo.Status.Should().Be(WorkOrderStatusEnum.COMPLETED);

        var inventory = await DbContext.Inventories.AsNoTracking().FirstAsync(i => i.ID == inventoryId);
        inventory.QuantityOnHand.Should().Be(initialQty - usedQty);

        var transactions = await DbContext.InventoryTransactions
            .Where(t => t.InventoryID == inventoryId)
            .ToListAsync();

        transactions.Should().Contain(t =>
            t.TransactionType == TransactionTypeEnum.OUT &&
            t.Quantity == usedQty &&
            t.PerformedByUserID == technicianId.ToString());
    }

    [Fact]
    public async Task Should_Throw_When_Completing_Already_Completed_WorkOrder()
    {
        // ===== Arrange =====
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var technicianId = await CreateTechnicianAsync();

        var schedStart = DateTime.UtcNow.AddMinutes(5);
        var actualStart = schedStart.AddMinutes(10);
        var schedComplete = schedStart.AddDays(1);
        var actualComplete = schedComplete.AddMinutes(30);

        var woId = await Sender.Send(new CreateWorkOrderCommand(
            VehicleID: vehicleId,
            AssignedToUserID: technicianId.ToString(),
            Title: $"WO {Faker.Random.AlphaNumeric(6)}",
            Description: "Already completed work",
            WorkOrderType: WorkTypeEnum.SCHEDULED,
            PriorityLevel: PriorityLevelEnum.MEDIUM,
            Status: WorkOrderStatusEnum.COMPLETED,
            ScheduledStartDate: schedStart,
            ActualStartDate: actualStart,
            ScheduledCompletionDate: schedComplete,
            ActualCompletionDate: actualComplete,
            StartOdometer: 1000,
            EndOdometer: 1100,
            IssueIdList: [],
            WorkOrderLineItems: []
        ));

        // ===== Act & Assert =====
        await FluentActions.Invoking(async () => await Sender.Send(new CompleteWorkOrderCommand(woId)))
            .Should().ThrowAsync<WorkOrderAlreadyCompletedException>();
    }
}