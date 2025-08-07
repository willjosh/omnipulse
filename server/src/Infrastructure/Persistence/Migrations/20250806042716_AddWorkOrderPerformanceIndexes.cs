using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrderPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Vehicles_VehicleID1",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_XrefServiceScheduleServiceTasks_ServiceScheduleID",
                table: "XrefServiceScheduleServiceTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_Status",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_VehicleID_Status",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_WorkOrderID_ItemType",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderIssues_IssueID",
                table: "WorkOrderIssues");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderIssues_WorkOrderID",
                table: "WorkOrderIssues");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_AssignedTechnicianID",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Status",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_VehicleGroups_IsActive",
                table: "VehicleGroups");

            migrationBuilder.DropIndex(
                name: "IX_VehicleGroups_Name",
                table: "VehicleGroups");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTasks_IsActive",
                table: "ServiceTasks");

            migrationBuilder.DropIndex(
                name: "IX_ServicePrograms_IsActive",
                table: "ServicePrograms");

            migrationBuilder.DropIndex(
                name: "IX_Issues_CreatedAt",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_InspectionID",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_ResolvedByUserID",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_VehicleID1",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "VehicleID1",
                table: "Issues");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_Status_ScheduledStartDate",
                table: "WorkOrders",
                newName: "IX_WorkOrders_StatusScheduledStart");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_Status_PriorityLevel",
                table: "WorkOrders",
                newName: "IX_WorkOrders_StatusPriority");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_AssignedToUserID_Status",
                table: "WorkOrders",
                newName: "IX_WorkOrders_UserStatus");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_VIN",
                table: "Vehicles",
                newName: "UX_Vehicles_VIN");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_LicensePlate",
                table: "Vehicles",
                newName: "UX_Vehicles_LicensePlate");

            migrationBuilder.AlterColumn<string>(
                name: "TimeIntervalUnit",
                table: "ServiceSchedules",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_XrefServiceScheduleServiceTasks_ScheduleIncludeTask",
                table: "XrefServiceScheduleServiceTasks",
                column: "ServiceScheduleID")
                .Annotation("SqlServer:Include", new[] { "ServiceTaskID" });

            migrationBuilder.CreateIndex(
                name: "IX_XrefServiceProgramVehicles_ProgramAdded",
                table: "XrefServiceProgramVehicles",
                columns: new[] { "ServiceProgramID", "AddedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_XrefServiceProgramVehicles_VehicleAdded",
                table: "XrefServiceProgramVehicles",
                columns: new[] { "VehicleID", "AddedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ActualCompletionStatus",
                table: "WorkOrders",
                columns: new[] { "ActualCompletionDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ActualStartStatus",
                table: "WorkOrders",
                columns: new[] { "ActualStartDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_EndOdometerStatus",
                table: "WorkOrders",
                columns: new[] { "EndOdometer", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_PriorityStatusScheduled",
                table: "WorkOrders",
                columns: new[] { "PriorityLevel", "Status", "ScheduledStartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ScheduledCompletionStatus",
                table: "WorkOrders",
                columns: new[] { "ScheduledCompletionDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ScheduledStartStatusCovering",
                table: "WorkOrders",
                columns: new[] { "ScheduledStartDate", "Status" })
                .Annotation("SqlServer:Include", new[] { "PriorityLevel", "WorkOrderType" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_StartOdometerStatus",
                table: "WorkOrders",
                columns: new[] { "StartOdometer", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_Status_Cancelled",
                table: "WorkOrders",
                column: "Status",
                filter: "Status = 6");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_StatusCreatedCovering",
                table: "WorkOrders",
                columns: new[] { "Status", "CreatedAt" })
                .Annotation("SqlServer:Include", new[] { "Title", "VehicleID", "AssignedToUserID", "PriorityLevel", "WorkOrderType", "ScheduledStartDate", "StartOdometer" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_TitleStatus",
                table: "WorkOrders",
                columns: new[] { "Title", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_TypeStatus",
                table: "WorkOrders",
                columns: new[] { "WorkOrderType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_UserScheduledActive",
                table: "WorkOrders",
                columns: new[] { "AssignedToUserID", "ScheduledStartDate" },
                filter: "Status IN (1, 3)");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_VehicleStatusCovering",
                table: "WorkOrders",
                columns: new[] { "VehicleID", "Status" })
                .Annotation("SqlServer:Include", new[] { "Title", "PriorityLevel", "WorkOrderType", "ScheduledStartDate", "CreatedAt" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkOrder_ActualDates",
                table: "WorkOrders",
                sql: "ActualCompletionDate IS NULL OR ActualStartDate IS NULL OR ActualCompletionDate >= ActualStartDate");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkOrder_ScheduledDates",
                table: "WorkOrders",
                sql: "ScheduledCompletionDate IS NULL OR ScheduledStartDate IS NULL OR ScheduledCompletionDate >= ScheduledStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_HourlyRateLaborHours",
                table: "WorkOrderLineItems",
                columns: new[] { "HourlyRate", "LaborHours" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_InventoryItemCreatedAt",
                table: "WorkOrderLineItems",
                columns: new[] { "InventoryItemID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_InventoryItemQuantity",
                table: "WorkOrderLineItems",
                columns: new[] { "InventoryItemID", "Quantity" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_ItemTypeTotalCost",
                table: "WorkOrderLineItems",
                columns: new[] { "ItemType", "TotalCost" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_ServiceTaskCreatedAtCovering",
                table: "WorkOrderLineItems",
                columns: new[] { "ServiceTaskID", "CreatedAt" })
                .Annotation("SqlServer:Include", new[] { "TotalCost", "LaborHours", "AssignedToUserID" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_ServiceTaskLaborHours",
                table: "WorkOrderLineItems",
                columns: new[] { "ServiceTaskID", "LaborHours" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_UnitPriceQuantity",
                table: "WorkOrderLineItems",
                columns: new[] { "UnitPrice", "Quantity" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_UserCreatedAt",
                table: "WorkOrderLineItems",
                columns: new[] { "AssignedToUserID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_UserLaborHours",
                table: "WorkOrderLineItems",
                columns: new[] { "AssignedToUserID", "LaborHours" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_WorkOrderCreatedAt",
                table: "WorkOrderLineItems",
                columns: new[] { "WorkOrderID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_WorkOrderItemTypeCost",
                table: "WorkOrderLineItems",
                columns: new[] { "WorkOrderID", "ItemType", "TotalCost" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_WorkOrderItemTypeCovering",
                table: "WorkOrderLineItems",
                columns: new[] { "WorkOrderID", "ItemType" })
                .Annotation("SqlServer:Include", new[] { "Quantity", "UnitPrice", "TotalCost", "LaborHours", "ServiceTaskID", "InventoryItemID" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_WorkOrderTotalCost",
                table: "WorkOrderLineItems",
                columns: new[] { "WorkOrderID", "TotalCost" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_AssignedDate",
                table: "WorkOrderIssues",
                column: "AssignedDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_IssueAssignedDate",
                table: "WorkOrderIssues",
                columns: new[] { "IssueID", "AssignedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_IssueCovering",
                table: "WorkOrderIssues",
                column: "IssueID")
                .Annotation("SqlServer:Include", new[] { "WorkOrderID", "AssignedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_WorkOrderAssignedDate",
                table: "WorkOrderIssues",
                columns: new[] { "WorkOrderID", "AssignedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_WorkOrderCovering",
                table: "WorkOrderIssues",
                column: "WorkOrderID")
                .Annotation("SqlServer:Include", new[] { "IssueID", "AssignedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_AssignedTechnician_Null",
                table: "Vehicles",
                column: "AssignedTechnicianID",
                filter: "AssignedTechnicianID IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CreatedAtStatus",
                table: "Vehicles",
                columns: new[] { "CreatedAt", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_EngineHoursStatus",
                table: "Vehicles",
                columns: new[] { "EngineHours", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_GroupNameCovering",
                table: "Vehicles",
                columns: new[] { "VehicleGroupID", "Name" })
                .Annotation("SqlServer:Include", new[] { "Status", "Make", "Model", "Year", "Mileage" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_GroupStatus",
                table: "Vehicles",
                columns: new[] { "VehicleGroupID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_GroupStatusMileage",
                table: "Vehicles",
                columns: new[] { "VehicleGroupID", "Status", "Mileage" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicenseExpirationStatus",
                table: "Vehicles",
                columns: new[] { "LicensePlateExpirationDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicensePlateStatus",
                table: "Vehicles",
                columns: new[] { "LicensePlate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Make",
                table: "Vehicles",
                column: "Make");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_MakeModelYear",
                table: "Vehicles",
                columns: new[] { "Make", "Model", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_MileageStatus",
                table: "Vehicles",
                columns: new[] { "Mileage", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Model",
                table: "Vehicles",
                column: "Model");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_NameStatus",
                table: "Vehicles",
                columns: new[] { "Name", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_PurchaseDateStatus",
                table: "Vehicles",
                columns: new[] { "PurchaseDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_PurchasePriceStatus",
                table: "Vehicles",
                columns: new[] { "PurchasePrice", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Status_Inactive",
                table: "Vehicles",
                column: "Status",
                filter: "Status = 4");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_StatusCreatedCovering",
                table: "Vehicles",
                columns: new[] { "Status", "CreatedAt" })
                .Annotation("SqlServer:Include", new[] { "Name", "Make", "Model", "Year", "VehicleType", "VehicleGroupID", "AssignedTechnicianID", "Mileage" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_StatusMileage",
                table: "Vehicles",
                columns: new[] { "Status", "Mileage" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TechnicianStatus",
                table: "Vehicles",
                columns: new[] { "AssignedTechnicianID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TypeStatus",
                table: "Vehicles",
                columns: new[] { "VehicleType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleType",
                table: "Vehicles",
                column: "VehicleType");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VINStatus",
                table: "Vehicles",
                columns: new[] { "VIN", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Year",
                table: "Vehicles",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleGroups_ActiveCovering",
                table: "VehicleGroups",
                column: "IsActive")
                .Annotation("SqlServer:Include", new[] { "Name", "Description" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleGroups_CreatedAtActive",
                table: "VehicleGroups",
                columns: new[] { "CreatedAt", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleGroups_NameActive",
                table: "VehicleGroups",
                columns: new[] { "Name", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_ActiveWithDetails",
                table: "ServiceTasks",
                column: "IsActive")
                .Annotation("SqlServer:Include", new[] { "Name", "Category", "EstimatedLabourHours", "EstimatedCost", "Description" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_MileageBasedActive",
                table: "ServiceSchedules",
                columns: new[] { "IsActive", "MileageInterval" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_ProgramMileageActive",
                table: "ServiceSchedules",
                columns: new[] { "ServiceProgramID", "IsActive", "MileageInterval" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_ProgramTimeActive",
                table: "ServiceSchedules",
                columns: new[] { "ServiceProgramID", "IsActive", "TimeIntervalValue" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_TimeBasedActive",
                table: "ServiceSchedules",
                columns: new[] { "IsActive", "TimeIntervalValue", "TimeIntervalUnit" });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrograms_ActiveWithName",
                table: "ServicePrograms",
                column: "IsActive")
                .Annotation("SqlServer:Include", new[] { "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_CreatedAtStatus",
                table: "Issues",
                columns: new[] { "CreatedAt", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_InspectionStatus",
                table: "Issues",
                columns: new[] { "InspectionID", "Status" },
                filter: "InspectionID IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ReportedDateStatusCovering",
                table: "Issues",
                columns: new[] { "ReportedDate", "Status" })
                .Annotation("SqlServer:Include", new[] { "PriorityLevel", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ResolvedByDateResolved",
                table: "Issues",
                columns: new[] { "ResolvedByUserID", "ResolvedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ResolvedDateStatus",
                table: "Issues",
                columns: new[] { "ResolvedDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_StatusPriorityReported",
                table: "Issues",
                columns: new[] { "Status", "PriorityLevel", "ReportedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_StatusReportedCovering",
                table: "Issues",
                columns: new[] { "Status", "ReportedDate" })
                .Annotation("SqlServer:Include", new[] { "Title", "PriorityLevel", "Category", "VehicleID", "ReportedByUserID" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_TitleStatus",
                table: "Issues",
                columns: new[] { "Title", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_TitleWithDetails",
                table: "Issues",
                column: "Title")
                .Annotation("SqlServer:Include", new[] { "Description", "Status", "PriorityLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_UpdatedAtStatus",
                table: "Issues",
                columns: new[] { "UpdatedAt", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_UserStatusReported",
                table: "Issues",
                columns: new[] { "ReportedByUserID", "Status", "ReportedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_VehicleStatusReported",
                table: "Issues",
                columns: new[] { "VehicleID", "Status", "ReportedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_XrefServiceScheduleServiceTasks_ScheduleIncludeTask",
                table: "XrefServiceScheduleServiceTasks");

            migrationBuilder.DropIndex(
                name: "IX_XrefServiceProgramVehicles_ProgramAdded",
                table: "XrefServiceProgramVehicles");

            migrationBuilder.DropIndex(
                name: "IX_XrefServiceProgramVehicles_VehicleAdded",
                table: "XrefServiceProgramVehicles");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ActualCompletionStatus",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ActualStartStatus",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_EndOdometerStatus",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_PriorityStatusScheduled",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ScheduledCompletionStatus",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ScheduledStartStatusCovering",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_StartOdometerStatus",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_Status_Cancelled",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_StatusCreatedCovering",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_TitleStatus",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_TypeStatus",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_UserScheduledActive",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_VehicleStatusCovering",
                table: "WorkOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkOrder_ActualDates",
                table: "WorkOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkOrder_ScheduledDates",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_HourlyRateLaborHours",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_InventoryItemCreatedAt",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_InventoryItemQuantity",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_ItemTypeTotalCost",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_ServiceTaskCreatedAtCovering",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_ServiceTaskLaborHours",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_UnitPriceQuantity",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_UserCreatedAt",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_UserLaborHours",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_WorkOrderCreatedAt",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_WorkOrderItemTypeCost",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_WorkOrderItemTypeCovering",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderLineItems_WorkOrderTotalCost",
                table: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderIssues_AssignedDate",
                table: "WorkOrderIssues");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderIssues_IssueAssignedDate",
                table: "WorkOrderIssues");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderIssues_IssueCovering",
                table: "WorkOrderIssues");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderIssues_WorkOrderAssignedDate",
                table: "WorkOrderIssues");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderIssues_WorkOrderCovering",
                table: "WorkOrderIssues");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_AssignedTechnician_Null",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CreatedAtStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_EngineHoursStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_GroupNameCovering",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_GroupStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_GroupStatusMileage",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_LicenseExpirationStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_LicensePlateStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Make",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_MakeModelYear",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_MileageStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Model",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_NameStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_PurchaseDateStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_PurchasePriceStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Status_Inactive",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_StatusCreatedCovering",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_StatusMileage",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TechnicianStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TypeStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VehicleType",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_VINStatus",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Year",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_VehicleGroups_ActiveCovering",
                table: "VehicleGroups");

            migrationBuilder.DropIndex(
                name: "IX_VehicleGroups_CreatedAtActive",
                table: "VehicleGroups");

            migrationBuilder.DropIndex(
                name: "IX_VehicleGroups_NameActive",
                table: "VehicleGroups");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTasks_ActiveWithDetails",
                table: "ServiceTasks");

            migrationBuilder.DropIndex(
                name: "IX_ServiceSchedules_MileageBasedActive",
                table: "ServiceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ServiceSchedules_ProgramMileageActive",
                table: "ServiceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ServiceSchedules_ProgramTimeActive",
                table: "ServiceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ServiceSchedules_TimeBasedActive",
                table: "ServiceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ServicePrograms_ActiveWithName",
                table: "ServicePrograms");

            migrationBuilder.DropIndex(
                name: "IX_Issues_CreatedAtStatus",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_InspectionStatus",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_ReportedDateStatusCovering",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_ResolvedByDateResolved",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_ResolvedDateStatus",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_StatusPriorityReported",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_StatusReportedCovering",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_TitleStatus",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_TitleWithDetails",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_UpdatedAtStatus",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_UserStatusReported",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_VehicleStatusReported",
                table: "Issues");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_UserStatus",
                table: "WorkOrders",
                newName: "IX_WorkOrders_AssignedToUserID_Status");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_StatusScheduledStart",
                table: "WorkOrders",
                newName: "IX_WorkOrders_Status_ScheduledStartDate");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_StatusPriority",
                table: "WorkOrders",
                newName: "IX_WorkOrders_Status_PriorityLevel");

            migrationBuilder.RenameIndex(
                name: "UX_Vehicles_VIN",
                table: "Vehicles",
                newName: "IX_Vehicles_VIN");

            migrationBuilder.RenameIndex(
                name: "UX_Vehicles_LicensePlate",
                table: "Vehicles",
                newName: "IX_Vehicles_LicensePlate");

            migrationBuilder.AlterColumn<string>(
                name: "TimeIntervalUnit",
                table: "ServiceSchedules",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleID1",
                table: "Issues",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_XrefServiceScheduleServiceTasks_ServiceScheduleID",
                table: "XrefServiceScheduleServiceTasks",
                column: "ServiceScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_Status",
                table: "WorkOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_VehicleID_Status",
                table: "WorkOrders",
                columns: new[] { "VehicleID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_WorkOrderID_ItemType",
                table: "WorkOrderLineItems",
                columns: new[] { "WorkOrderID", "ItemType" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_IssueID",
                table: "WorkOrderIssues",
                column: "IssueID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_WorkOrderID",
                table: "WorkOrderIssues",
                column: "WorkOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_AssignedTechnicianID",
                table: "Vehicles",
                column: "AssignedTechnicianID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Status",
                table: "Vehicles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleGroups_IsActive",
                table: "VehicleGroups",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleGroups_Name",
                table: "VehicleGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_IsActive",
                table: "ServiceTasks",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrograms_IsActive",
                table: "ServicePrograms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_CreatedAt",
                table: "Issues",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_InspectionID",
                table: "Issues",
                column: "InspectionID");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ResolvedByUserID",
                table: "Issues",
                column: "ResolvedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_VehicleID1",
                table: "Issues",
                column: "VehicleID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Vehicles_VehicleID1",
                table: "Issues",
                column: "VehicleID1",
                principalTable: "Vehicles",
                principalColumn: "ID");
        }
    }
}