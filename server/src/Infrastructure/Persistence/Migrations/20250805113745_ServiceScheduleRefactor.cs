using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ServiceScheduleRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_ServiceReminders_ServiceReminderID",
                table: "WorkOrders");

            migrationBuilder.DropTable(
                name: "InspectionAttachments");

            migrationBuilder.DropTable(
                name: "InspectionChecklistResponses");

            migrationBuilder.DropTable(
                name: "CheckListItems");

            migrationBuilder.DropTable(
                name: "VehicleInspections");

            migrationBuilder.DropTable(
                name: "InspectionTypes");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_ServiceReminderID",
                table: "WorkOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkOrder_CompletionDates",
                table: "WorkOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkOrder_Dates",
                table: "WorkOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceSchedule_FirstServiceTimeValue",
                table: "ServiceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_IsCompleted",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_IsCompleted_DueDate",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_VehicleID_IsCompleted",
                table: "ServiceReminders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceReminder_CompletedDate",
                table: "ServiceReminders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceReminder_DueEngineHours",
                table: "ServiceReminders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceReminder_NotificationCount",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_Issues_IssueNumber",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ServiceReminderID",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "FirstServiceTimeUnit",
                table: "ServiceSchedules");

            migrationBuilder.DropColumn(
                name: "FirstServiceTimeValue",
                table: "ServiceSchedules");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "DueEngineHours",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "LastNotificationSentDate",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "NotificationCount",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "IssueNumber",
                table: "Issues");

            migrationBuilder.AddColumn<double>(
                name: "VehicleMileageAtAssignment",
                table: "XrefServiceProgramVehicles",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstServiceDate",
                table: "ServiceSchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DueMileage",
                table: "ServiceReminders",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "ServiceReminders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<double>(
                name: "MeterVariance",
                table: "ServiceReminders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MileageBuffer",
                table: "ServiceReminders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MileageInterval",
                table: "ServiceReminders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceProgramID",
                table: "ServiceReminders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceProgramName",
                table: "ServiceReminders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceScheduleName",
                table: "ServiceReminders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TimeBufferUnit",
                table: "ServiceReminders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeBufferValue",
                table: "ServiceReminders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeIntervalUnit",
                table: "ServiceReminders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeIntervalValue",
                table: "ServiceReminders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkOrderID",
                table: "ServiceReminders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InspectionID",
                table: "Issues",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InspectionForms",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionForms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InspectionFormItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionFormID = table.Column<int>(type: "int", nullable: false),
                    ItemLabel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ItemInstructions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    InspectionFormItemTypeEnum = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionFormItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InspectionFormItems_InspectionForms_InspectionFormID",
                        column: x => x.InspectionFormID,
                        principalTable: "InspectionForms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionFormID = table.Column<int>(type: "int", nullable: false),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    TechnicianID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InspectionStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InspectionEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdometerReading = table.Column<double>(type: "float", nullable: true),
                    VehicleCondition = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SnapshotFormTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SnapshotFormDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.ID);
                    table.CheckConstraint("CK_Inspection_InspectionStartTime", "InspectionStartTime >= '2000-01-01' AND InspectionStartTime <= GETDATE()");
                    table.CheckConstraint("CK_Inspection_OdometerReading", "OdometerReading >= 0");
                    table.CheckConstraint("CK_Inspection_Times", "InspectionEndTime >= InspectionStartTime");
                    table.ForeignKey(
                        name: "FK_Inspections_InspectionForms_InspectionFormID",
                        column: x => x.InspectionFormID,
                        principalTable: "InspectionForms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inspections_Users_TechnicianID",
                        column: x => x.TechnicianID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inspections_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionPassFailItems",
                columns: table => new
                {
                    InspectionID = table.Column<int>(type: "int", nullable: false),
                    InspectionFormItemID = table.Column<int>(type: "int", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SnapshotItemLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SnapshotItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnapshotItemInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnapshotIsRequired = table.Column<bool>(type: "bit", nullable: false),
                    SnapshotInspectionFormItemTypeEnum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionPassFailItems", x => new { x.InspectionID, x.InspectionFormItemID });
                    table.ForeignKey(
                        name: "FK_InspectionPassFailItems_InspectionFormItems_InspectionFormItemID",
                        column: x => x.InspectionFormItemID,
                        principalTable: "InspectionFormItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionPassFailItems_Inspections_InspectionID",
                        column: x => x.InspectionID,
                        principalTable: "Inspections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_ServiceProgramID",
                table: "ServiceReminders",
                column: "ServiceProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_WorkOrderID",
                table: "ServiceReminders",
                column: "WorkOrderID");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceReminder_CompletedDate",
                table: "ServiceReminders",
                sql: "CompletedDate IS NULL OR CompletedDate >= CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_InspectionID",
                table: "Issues",
                column: "InspectionID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_InspectionFormID",
                table: "InspectionFormItems",
                column: "InspectionFormID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_InspectionFormID_ItemLabel",
                table: "InspectionFormItems",
                columns: new[] { "InspectionFormID", "ItemLabel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_IsRequired",
                table: "InspectionFormItems",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionForms_CreatedAt",
                table: "InspectionForms",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionForms_IsActive",
                table: "InspectionForms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionForms_Title",
                table: "InspectionForms",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_InspectionFormItemID",
                table: "InspectionPassFailItems",
                column: "InspectionFormItemID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_Passed",
                table: "InspectionPassFailItems",
                column: "Passed");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_InspectionFormID",
                table: "Inspections",
                column: "InspectionFormID");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_InspectionFormID_InspectionStartTime",
                table: "Inspections",
                columns: new[] { "InspectionFormID", "InspectionStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_InspectionStartTime",
                table: "Inspections",
                column: "InspectionStartTime");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_TechnicianID",
                table: "Inspections",
                column: "TechnicianID");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_TechnicianID_InspectionStartTime",
                table: "Inspections",
                columns: new[] { "TechnicianID", "InspectionStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_VehicleCondition",
                table: "Inspections",
                column: "VehicleCondition");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_VehicleID",
                table: "Inspections",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_VehicleID_InspectionStartTime",
                table: "Inspections",
                columns: new[] { "VehicleID", "InspectionStartTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceReminders_ServicePrograms_ServiceProgramID",
                table: "ServiceReminders",
                column: "ServiceProgramID",
                principalTable: "ServicePrograms",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceReminders_WorkOrders_WorkOrderID",
                table: "ServiceReminders",
                column: "WorkOrderID",
                principalTable: "WorkOrders",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceReminders_ServicePrograms_ServiceProgramID",
                table: "ServiceReminders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceReminders_WorkOrders_WorkOrderID",
                table: "ServiceReminders");

            migrationBuilder.DropTable(
                name: "InspectionPassFailItems");

            migrationBuilder.DropTable(
                name: "InspectionFormItems");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "InspectionForms");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_ServiceProgramID",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_WorkOrderID",
                table: "ServiceReminders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceReminder_CompletedDate",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_Issues_InspectionID",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "VehicleMileageAtAssignment",
                table: "XrefServiceProgramVehicles");

            migrationBuilder.DropColumn(
                name: "FirstServiceDate",
                table: "ServiceSchedules");

            migrationBuilder.DropColumn(
                name: "MeterVariance",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "MileageBuffer",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "MileageInterval",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "ServiceProgramID",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "ServiceProgramName",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "ServiceScheduleName",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "TimeBufferUnit",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "TimeBufferValue",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "TimeIntervalUnit",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "TimeIntervalValue",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "WorkOrderID",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "InspectionID",
                table: "Issues");

            migrationBuilder.AddColumn<int>(
                name: "ServiceReminderID",
                table: "WorkOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstServiceTimeUnit",
                table: "ServiceSchedules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FirstServiceTimeValue",
                table: "ServiceSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DueMileage",
                table: "ServiceReminders",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "ServiceReminders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ServiceReminders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DueEngineHours",
                table: "ServiceReminders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "ServiceReminders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastNotificationSentDate",
                table: "ServiceReminders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationCount",
                table: "ServiceReminders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ServiceReminders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IssueNumber",
                table: "Issues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InspectionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CheckListItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionTypeID = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InputType = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckListItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CheckListItems_InspectionTypes_InspectionTypeID",
                        column: x => x.InspectionTypeID,
                        principalTable: "InspectionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleInspections",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionTypeID = table.Column<int>(type: "int", nullable: false),
                    TechnicianID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    MileageAtInspection = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OverallStatus = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleInspections", x => x.ID);
                    table.CheckConstraint("CK_VehicleInspection_InspectionDate", "InspectionDate >= '2000-01-01' AND InspectionDate <= GETDATE()");
                    table.CheckConstraint("CK_VehicleInspection_MileageAtInspection", "MileageAtInspection >= 0");
                    table.CheckConstraint("CK_VehicleInspection_Times", "EndTime >= StartTime");
                    table.ForeignKey(
                        name: "FK_VehicleInspections_InspectionTypes_InspectionTypeID",
                        column: x => x.InspectionTypeID,
                        principalTable: "InspectionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleInspections_Users_TechnicianID",
                        column: x => x.TechnicianID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleInspections_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionAttachments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckListItemID = table.Column<int>(type: "int", nullable: false),
                    VehicleInspectionID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionAttachments", x => x.ID);
                    table.CheckConstraint("CK_InspectionAttachment_FileSize", "FileSize > 0");
                    table.ForeignKey(
                        name: "FK_InspectionAttachments_CheckListItems_CheckListItemID",
                        column: x => x.CheckListItemID,
                        principalTable: "CheckListItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionAttachments_VehicleInspections_VehicleInspectionID",
                        column: x => x.VehicleInspectionID,
                        principalTable: "VehicleInspections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionChecklistResponses",
                columns: table => new
                {
                    VehicleInspectionID = table.Column<int>(type: "int", nullable: false),
                    ChecklistItemID = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiresAttention = table.Column<bool>(type: "bit", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TextResponse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionChecklistResponses", x => new { x.VehicleInspectionID, x.ChecklistItemID });
                    table.ForeignKey(
                        name: "FK_InspectionChecklistResponses_CheckListItems_ChecklistItemID",
                        column: x => x.ChecklistItemID,
                        principalTable: "CheckListItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionChecklistResponses_VehicleInspections_VehicleInspectionID",
                        column: x => x.VehicleInspectionID,
                        principalTable: "VehicleInspections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ServiceReminderID",
                table: "WorkOrders",
                column: "ServiceReminderID");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkOrder_CompletionDates",
                table: "WorkOrders",
                sql: "ActualCompletionDate IS NULL OR ScheduledCompletionDate IS NULL OR ActualCompletionDate >= ScheduledCompletionDate");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkOrder_Dates",
                table: "WorkOrders",
                sql: "ActualStartDate IS NULL OR ScheduledStartDate IS NULL OR ActualStartDate >= ScheduledStartDate");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceSchedule_FirstServiceTimeValue",
                table: "ServiceSchedules",
                sql: "FirstServiceTimeValue >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_IsCompleted",
                table: "ServiceReminders",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_IsCompleted_DueDate",
                table: "ServiceReminders",
                columns: new[] { "IsCompleted", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_VehicleID_IsCompleted",
                table: "ServiceReminders",
                columns: new[] { "VehicleID", "IsCompleted" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceReminder_CompletedDate",
                table: "ServiceReminders",
                sql: "CompletedDate IS NULL OR (IsCompleted = 1 AND CompletedDate >= CreatedAt)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceReminder_DueEngineHours",
                table: "ServiceReminders",
                sql: "DueEngineHours >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceReminder_NotificationCount",
                table: "ServiceReminders",
                sql: "NotificationCount >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssueNumber",
                table: "Issues",
                column: "IssueNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckListItems_Category",
                table: "CheckListItems",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CheckListItems_InspectionTypeID",
                table: "CheckListItems",
                column: "InspectionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_CheckListItems_InspectionTypeID_ItemName",
                table: "CheckListItems",
                columns: new[] { "InspectionTypeID", "ItemName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckListItems_IsMandatory",
                table: "CheckListItems",
                column: "IsMandatory");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionAttachments_CheckListItemID",
                table: "InspectionAttachments",
                column: "CheckListItemID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionAttachments_CreatedAt",
                table: "InspectionAttachments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionAttachments_FileType",
                table: "InspectionAttachments",
                column: "FileType");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionAttachments_VehicleInspectionID",
                table: "InspectionAttachments",
                column: "VehicleInspectionID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionAttachments_VehicleInspectionID_CheckListItemID",
                table: "InspectionAttachments",
                columns: new[] { "VehicleInspectionID", "CheckListItemID" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionChecklistResponses_ChecklistItemID",
                table: "InspectionChecklistResponses",
                column: "ChecklistItemID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionChecklistResponses_RequiresAttention",
                table: "InspectionChecklistResponses",
                column: "RequiresAttention");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionChecklistResponses_ResponseDate",
                table: "InspectionChecklistResponses",
                column: "ResponseDate");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionChecklistResponses_Status",
                table: "InspectionChecklistResponses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionTypes_CreatedAt",
                table: "InspectionTypes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionTypes_Name",
                table: "InspectionTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_InspectionDate",
                table: "VehicleInspections",
                column: "InspectionDate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_InspectionTypeID",
                table: "VehicleInspections",
                column: "InspectionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_InspectionTypeID_InspectionDate",
                table: "VehicleInspections",
                columns: new[] { "InspectionTypeID", "InspectionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_IsPassed",
                table: "VehicleInspections",
                column: "IsPassed");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_OverallStatus",
                table: "VehicleInspections",
                column: "OverallStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_TechnicianID",
                table: "VehicleInspections",
                column: "TechnicianID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_TechnicianID_InspectionDate",
                table: "VehicleInspections",
                columns: new[] { "TechnicianID", "InspectionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_VehicleID",
                table: "VehicleInspections",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInspections_VehicleID_InspectionDate",
                table: "VehicleInspections",
                columns: new[] { "VehicleID", "InspectionDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_ServiceReminders_ServiceReminderID",
                table: "WorkOrders",
                column: "ServiceReminderID",
                principalTable: "ServiceReminders",
                principalColumn: "ID");
        }
    }
}