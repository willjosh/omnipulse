using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVehicleEngineHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceReminders_ServicePrograms_ServiceProgramID",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_EngineHoursStatus",
                table: "Vehicles");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Vehicle_EngineHours",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_ServiceSchedules_ServiceProgramID_Name",
                table: "ServiceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_PriorityLevel",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_ServiceProgramID",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_VehicleID_DueDate",
                table: "ServiceReminders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceReminder_DueDate",
                table: "ServiceReminders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceReminder_DueMileage",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "EngineHours",
                table: "Vehicles");

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
                name: "PriorityLevel",
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

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ServiceSchedules",
                newName: "IsSoftDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_TimeBasedActive",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_TimeBased");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_ServiceProgramID_IsActive",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_ServiceProgramID_IsSoftDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_ProgramTimeActive",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_ProgramTime");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_ProgramMileageActive",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_ProgramMileage");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_MileageBasedActive",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_MileageBased");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_IsActive_Name",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_IsSoftDeleted_Name");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_IsActive",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_IsSoftDeleted");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ServiceReminders",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "ServiceReminders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_ServiceProgramID_Name",
                table: "ServiceSchedules",
                columns: new[] { "ServiceProgramID", "Name" },
                unique: true,
                filter: "[IsSoftDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceSchedule_XOR_Constraint",
                table: "ServiceSchedules",
                sql: "(TimeIntervalValue IS NOT NULL AND TimeIntervalUnit IS NOT NULL AND MileageInterval IS NULL) OR (TimeIntervalValue IS NULL AND TimeIntervalUnit IS NULL AND MileageInterval IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_DueDate_Status",
                table: "ServiceReminders",
                columns: new[] { "DueDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_DueMileage",
                table: "ServiceReminders",
                column: "DueMileage");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_DueMileage_Status",
                table: "ServiceReminders",
                columns: new[] { "DueMileage", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_ServiceScheduleID_Status",
                table: "ServiceReminders",
                columns: new[] { "ServiceScheduleID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_Status_DueMileage",
                table: "ServiceReminders",
                columns: new[] { "Status", "DueMileage" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_VehicleID_ServiceScheduleID_Status_Unique",
                table: "ServiceReminders",
                columns: new[] { "VehicleID", "ServiceScheduleID", "Status" },
                unique: true,
                filter: "[Status] = 'UPCOMING'");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_VehicleID_Status",
                table: "ServiceReminders",
                columns: new[] { "VehicleID", "Status" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceReminder_DueMileage",
                table: "ServiceReminders",
                sql: "DueMileage IS NULL OR DueMileage >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceReminder_HasDueTarget",
                table: "ServiceReminders",
                sql: "DueDate IS NOT NULL OR DueMileage IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceSchedules_ServiceProgramID_Name",
                table: "ServiceSchedules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceSchedule_XOR_Constraint",
                table: "ServiceSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_DueDate_Status",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_DueMileage",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_DueMileage_Status",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_ServiceScheduleID_Status",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_Status_DueMileage",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_VehicleID_ServiceScheduleID_Status_Unique",
                table: "ServiceReminders");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReminders_VehicleID_Status",
                table: "ServiceReminders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceReminder_DueMileage",
                table: "ServiceReminders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ServiceReminder_HasDueTarget",
                table: "ServiceReminders");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "ServiceReminders");

            migrationBuilder.RenameColumn(
                name: "IsSoftDeleted",
                table: "ServiceSchedules",
                newName: "IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_TimeBased",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_TimeBasedActive");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_ServiceProgramID_IsSoftDeleted",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_ServiceProgramID_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_ProgramTime",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_ProgramTimeActive");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_ProgramMileage",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_ProgramMileageActive");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_MileageBased",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_MileageBasedActive");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_IsSoftDeleted_Name",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_IsActive_Name");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceSchedules_IsSoftDeleted",
                table: "ServiceSchedules",
                newName: "IX_ServiceSchedules_IsActive");

            migrationBuilder.AddColumn<double>(
                name: "EngineHours",
                table: "Vehicles",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ServiceReminders",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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
                name: "PriorityLevel",
                table: "ServiceReminders",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_EngineHoursStatus",
                table: "Vehicles",
                columns: new[] { "EngineHours", "Status" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Vehicle_EngineHours",
                table: "Vehicles",
                sql: "EngineHours >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_ServiceProgramID_Name",
                table: "ServiceSchedules",
                columns: new[] { "ServiceProgramID", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_PriorityLevel",
                table: "ServiceReminders",
                column: "PriorityLevel");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_ServiceProgramID",
                table: "ServiceReminders",
                column: "ServiceProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_VehicleID_DueDate",
                table: "ServiceReminders",
                columns: new[] { "VehicleID", "DueDate" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceReminder_DueDate",
                table: "ServiceReminders",
                sql: "DueDate >= CreatedAt");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ServiceReminder_DueMileage",
                table: "ServiceReminders",
                sql: "DueMileage >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceReminders_ServicePrograms_ServiceProgramID",
                table: "ServiceReminders",
                column: "ServiceProgramID",
                principalTable: "ServicePrograms",
                principalColumn: "ID");
        }
    }
}