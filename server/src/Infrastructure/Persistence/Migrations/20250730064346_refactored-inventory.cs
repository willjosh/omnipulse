using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class refactoredinventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_InventoryItemLocations_InventoryItemLocationID",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_MaintenanceHistories_MaintenanceHistoryID",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceHistories_ServiceTasks_ServiceTaskID",
                table: "MaintenanceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceHistories_Vehicles_VehicleID",
                table: "MaintenanceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleAssignment_Users_UserId",
                table: "VehicleAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_XrefServiceProgramVehicles_Users_UserId",
                table: "XrefServiceProgramVehicles");

            migrationBuilder.DropIndex(
                name: "IX_XrefServiceProgramVehicles_UserId",
                table: "XrefServiceProgramVehicles");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceHistories_TechnicianID",
                table: "MaintenanceHistories");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceHistories_VehicleID",
                table: "MaintenanceHistories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MaintenanceHistory_Cost",
                table: "MaintenanceHistories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MaintenanceHistory_LabourHours",
                table: "MaintenanceHistories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_MaintenanceHistory_MileageAtService",
                table: "MaintenanceHistories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Issue_ResolvedDate",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_MaintenanceHistoryID",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "XrefServiceProgramVehicles");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "MaintenanceHistories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MaintenanceHistories");

            migrationBuilder.DropColumn(
                name: "LabourHours",
                table: "MaintenanceHistories");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "MaintenanceHistories");

            migrationBuilder.DropColumn(
                name: "TechnicianID",
                table: "MaintenanceHistories");

            migrationBuilder.DropColumn(
                name: "VehicleID",
                table: "MaintenanceHistories");

            migrationBuilder.DropColumn(
                name: "MaintenanceHistoryID",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ReorderPoint",
                table: "Inventories");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualCompletionDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledCompletionDate",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "VehicleAssignment",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceTaskID",
                table: "MaintenanceHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "InventoryTransactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "InventoryTransactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "InventoryTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PerformedByUserID",
                table: "InventoryTransactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "InventoryTransactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "InventoryItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "Inventories",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "QuantityOnHand",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MinStockLevel",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaxStockLevel",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastRestockedDate",
                table: "Inventories",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "InventoryItemLocationID",
                table: "Inventories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "NeedsReorder",
                table: "Inventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ActualCompletionDate",
                table: "WorkOrders",
                column: "ActualCompletionDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ScheduledCompletionDate",
                table: "WorkOrders",
                column: "ScheduledCompletionDate");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkOrder_CompletionDates",
                table: "WorkOrders",
                sql: "ActualCompletionDate IS NULL OR ScheduledCompletionDate IS NULL OR ActualCompletionDate >= ScheduledCompletionDate");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Issue_ResolvedDate",
                table: "Issues",
                sql: "ResolvedDate IS NULL OR ResolvedDate >= ReportedDate");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_CreatedAt",
                table: "InventoryTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_PerformedByUserID",
                table: "InventoryTransactions",
                column: "PerformedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_TransactionType",
                table: "InventoryTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_UpdatedAt",
                table: "InventoryTransactions",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_UserId",
                table: "InventoryTransactions",
                column: "UserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InventoryTransaction_Quantity_Positive",
                table: "InventoryTransactions",
                sql: "[Quantity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InventoryTransaction_TotalCost_NonNegative",
                table: "InventoryTransactions",
                sql: "[TotalCost] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_InventoryTransaction_UnitCost_NonNegative",
                table: "InventoryTransactions",
                sql: "[UnitCost] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CreatedAt",
                table: "Inventories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Item_Cost",
                table: "Inventories",
                columns: new[] { "InventoryItemID", "UnitCost" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ItemPerformance",
                table: "Inventories",
                columns: new[] { "InventoryItemID", "LastRestockedDate", "QuantityOnHand" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_LastRestockedDate",
                table: "Inventories",
                column: "LastRestockedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Location_Item",
                table: "Inventories",
                columns: new[] { "InventoryItemLocationID", "InventoryItemID" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Location_Stock",
                table: "Inventories",
                columns: new[] { "InventoryItemLocationID", "QuantityOnHand" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_LocationSummary",
                table: "Inventories",
                columns: new[] { "InventoryItemLocationID", "QuantityOnHand", "UnitCost" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_LowStock",
                table: "Inventories",
                columns: new[] { "QuantityOnHand", "MinStockLevel", "InventoryItemID" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_NeedsReorder",
                table: "Inventories",
                column: "NeedsReorder",
                filter: "[NeedsReorder] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_RestockDate_Item",
                table: "Inventories",
                columns: new[] { "LastRestockedDate", "InventoryItemID" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_StockLevels",
                table: "Inventories",
                columns: new[] { "QuantityOnHand", "MinStockLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_StockMovement",
                table: "Inventories",
                columns: new[] { "UpdatedAt", "QuantityOnHand", "InventoryItemID" });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_UnitCost",
                table: "Inventories",
                column: "UnitCost");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_UpdatedAt",
                table: "Inventories",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ValueAnalysis",
                table: "Inventories",
                columns: new[] { "QuantityOnHand", "UnitCost", "InventoryItemID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_InventoryItemLocations_InventoryItemLocationID",
                table: "Inventories",
                column: "InventoryItemLocationID",
                principalTable: "InventoryItemLocations",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Users_PerformedByUserID",
                table: "InventoryTransactions",
                column: "PerformedByUserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Users_UserId",
                table: "InventoryTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceHistories_ServiceTasks_ServiceTaskID",
                table: "MaintenanceHistories",
                column: "ServiceTaskID",
                principalTable: "ServiceTasks",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleAssignment_Users_UserId",
                table: "VehicleAssignment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_InventoryItemLocations_InventoryItemLocationID",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Users_PerformedByUserID",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Users_UserId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceHistories_ServiceTasks_ServiceTaskID",
                table: "MaintenanceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleAssignment_Users_UserId",
                table: "VehicleAssignment");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ActualCompletionDate",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ScheduledCompletionDate",
                table: "WorkOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkOrder_CompletionDates",
                table: "WorkOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Issue_ResolvedDate",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_CreatedAt",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_PerformedByUserID",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_TransactionType",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_UpdatedAt",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_UserId",
                table: "InventoryTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_InventoryTransaction_Quantity_Positive",
                table: "InventoryTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_InventoryTransaction_TotalCost_NonNegative",
                table: "InventoryTransactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_InventoryTransaction_UnitCost_NonNegative",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_CreatedAt",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Item_Cost",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_ItemPerformance",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_LastRestockedDate",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Location_Item",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Location_Stock",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_LocationSummary",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_LowStock",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_NeedsReorder",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_RestockDate_Item",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_StockLevels",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_StockMovement",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_UnitCost",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_UpdatedAt",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_ValueAnalysis",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ActualCompletionDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ScheduledCompletionDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "NeedsReorder",
                table: "Inventories");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "XrefServiceProgramVehicles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "VehicleAssignment",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceTaskID",
                table: "MaintenanceHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "MaintenanceHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MaintenanceHistories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LabourHours",
                table: "MaintenanceHistories",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "MaintenanceHistories",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicianID",
                table: "MaintenanceHistories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VehicleID",
                table: "MaintenanceHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "InventoryTransactions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "InventoryTransactions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "InventoryTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PerformedByUserID",
                table: "InventoryTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceHistoryID",
                table: "InventoryTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "InventoryTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "InventoryTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "InventoryItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "Inventories",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "QuantityOnHand",
                table: "Inventories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "MinStockLevel",
                table: "Inventories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "MaxStockLevel",
                table: "Inventories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "LastRestockedDate",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InventoryItemLocationID",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReorderPoint",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_XrefServiceProgramVehicles_UserId",
                table: "XrefServiceProgramVehicles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_TechnicianID",
                table: "MaintenanceHistories",
                column: "TechnicianID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_VehicleID",
                table: "MaintenanceHistories",
                column: "VehicleID");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MaintenanceHistory_Cost",
                table: "MaintenanceHistories",
                sql: "Cost >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MaintenanceHistory_LabourHours",
                table: "MaintenanceHistories",
                sql: "LabourHours >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_MaintenanceHistory_MileageAtService",
                table: "MaintenanceHistories",
                sql: "MileageAtService >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Issue_ResolvedDate",
                table: "Issues",
                sql: "ResolvedDate IS NULL OR ResolvedDate >= CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_MaintenanceHistoryID",
                table: "InventoryTransactions",
                column: "MaintenanceHistoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_InventoryItemLocations_InventoryItemLocationID",
                table: "Inventories",
                column: "InventoryItemLocationID",
                principalTable: "InventoryItemLocations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_MaintenanceHistories_MaintenanceHistoryID",
                table: "InventoryTransactions",
                column: "MaintenanceHistoryID",
                principalTable: "MaintenanceHistories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceHistories_ServiceTasks_ServiceTaskID",
                table: "MaintenanceHistories",
                column: "ServiceTaskID",
                principalTable: "ServiceTasks",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceHistories_Vehicles_VehicleID",
                table: "MaintenanceHistories",
                column: "VehicleID",
                principalTable: "Vehicles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleAssignment_Users_UserId",
                table: "VehicleAssignment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_XrefServiceProgramVehicles_Users_UserId",
                table: "XrefServiceProgramVehicles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}