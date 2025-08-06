using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreIndexing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inspections_TechnicianID_InspectionStartTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_VehicleCondition",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_VehicleID_InspectionStartTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_Passed",
                table: "InspectionPassFailItems");

            migrationBuilder.RenameIndex(
                name: "IX_Inspections_InspectionFormID_InspectionStartTime",
                table: "Inspections",
                newName: "IX_Inspections_FormStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotFormTitle",
                table: "Inspections",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotFormDescription",
                table: "Inspections",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Inspections",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotItemLabel",
                table: "InspectionPassFailItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotItemInstructions",
                table: "InspectionPassFailItems",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotItemDescription",
                table: "InspectionPassFailItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_Condition_NotSafe",
                table: "Inspections",
                column: "VehicleCondition",
                filter: "VehicleCondition = 3");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_ConditionStartTime",
                table: "Inspections",
                columns: new[] { "VehicleCondition", "InspectionStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_CreatedAtCondition",
                table: "Inspections",
                columns: new[] { "CreatedAt", "VehicleCondition" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_DateRangeCondition",
                table: "Inspections",
                columns: new[] { "InspectionStartTime", "InspectionEndTime", "VehicleCondition" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_FormCondition",
                table: "Inspections",
                columns: new[] { "InspectionFormID", "VehicleCondition" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_FormTechnician",
                table: "Inspections",
                columns: new[] { "InspectionFormID", "TechnicianID" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_FormTitleStartTime",
                table: "Inspections",
                columns: new[] { "SnapshotFormTitle", "InspectionStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_InspectionEndTime",
                table: "Inspections",
                column: "InspectionEndTime");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_NotesStartTime",
                table: "Inspections",
                columns: new[] { "Notes", "InspectionStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_OdometerReading",
                table: "Inspections",
                column: "OdometerReading");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_OdometerStartTime",
                table: "Inspections",
                columns: new[] { "OdometerReading", "InspectionStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_StartEndTime",
                table: "Inspections",
                columns: new[] { "InspectionStartTime", "InspectionEndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_StartTimeCondition",
                table: "Inspections",
                columns: new[] { "InspectionStartTime", "VehicleCondition" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_StartTimeCreatedCovering",
                table: "Inspections",
                columns: new[] { "InspectionStartTime", "CreatedAt" })
                .Annotation("SqlServer:Include", new[] { "VehicleID", "TechnicianID", "VehicleCondition", "InspectionFormID", "OdometerReading", "SnapshotFormTitle" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_TechnicianCondition",
                table: "Inspections",
                columns: new[] { "TechnicianID", "VehicleCondition" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_TechnicianEndTime",
                table: "Inspections",
                columns: new[] { "TechnicianID", "InspectionEndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_TechnicianStartTimeCovering",
                table: "Inspections",
                columns: new[] { "TechnicianID", "InspectionStartTime" })
                .Annotation("SqlServer:Include", new[] { "VehicleID", "VehicleCondition", "InspectionEndTime", "OdometerReading", "SnapshotFormTitle" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_UpdatedAtStartTime",
                table: "Inspections",
                columns: new[] { "UpdatedAt", "InspectionStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_UserId",
                table: "Inspections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_VehicleCondition",
                table: "Inspections",
                columns: new[] { "VehicleID", "VehicleCondition" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_VehicleOdometer",
                table: "Inspections",
                columns: new[] { "VehicleID", "OdometerReading" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_VehicleStartTimeCovering",
                table: "Inspections",
                columns: new[] { "VehicleID", "InspectionStartTime" })
                .Annotation("SqlServer:Include", new[] { "TechnicianID", "VehicleCondition", "InspectionEndTime", "OdometerReading", "SnapshotFormTitle" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_CommentPassed",
                table: "InspectionPassFailItems",
                columns: new[] { "Comment", "Passed" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_Failed",
                table: "InspectionPassFailItems",
                column: "Passed",
                filter: "Passed = 0");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_FormItemPassed",
                table: "InspectionPassFailItems",
                columns: new[] { "InspectionFormItemID", "Passed" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_FormItemPassedRequired",
                table: "InspectionPassFailItems",
                columns: new[] { "InspectionFormItemID", "Passed", "SnapshotIsRequired" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_FormItemRequiredCovering",
                table: "InspectionPassFailItems",
                columns: new[] { "InspectionFormItemID", "SnapshotIsRequired" })
                .Annotation("SqlServer:Include", new[] { "Passed", "SnapshotItemLabel" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_InspectionCovering",
                table: "InspectionPassFailItems",
                column: "InspectionID")
                .Annotation("SqlServer:Include", new[] { "InspectionFormItemID", "Passed", "Comment", "SnapshotItemLabel", "SnapshotIsRequired" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_InspectionFailed",
                table: "InspectionPassFailItems",
                columns: new[] { "InspectionID", "Passed" },
                filter: "Passed = 0")
                .Annotation("SqlServer:Include", new[] { "SnapshotItemLabel", "Comment", "SnapshotIsRequired" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_InspectionRequired",
                table: "InspectionPassFailItems",
                columns: new[] { "InspectionID", "SnapshotIsRequired" },
                filter: "SnapshotIsRequired = 1");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_ItemType",
                table: "InspectionPassFailItems",
                column: "SnapshotInspectionFormItemTypeEnum");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_LabelPassed",
                table: "InspectionPassFailItems",
                columns: new[] { "SnapshotItemLabel", "Passed" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_PassedRequired",
                table: "InspectionPassFailItems",
                columns: new[] { "Passed", "SnapshotIsRequired" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_RequiredPassed",
                table: "InspectionPassFailItems",
                columns: new[] { "SnapshotIsRequired", "Passed" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_TypePassed",
                table: "InspectionPassFailItems",
                columns: new[] { "SnapshotInspectionFormItemTypeEnum", "Passed" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_Active",
                table: "InspectionFormItems",
                column: "IsActive",
                filter: "IsActive = 1");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_CreatedAtForm",
                table: "InspectionFormItems",
                columns: new[] { "CreatedAt", "InspectionFormID" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_FormActiveCovering",
                table: "InspectionFormItems",
                columns: new[] { "InspectionFormID", "IsActive" })
                .Annotation("SqlServer:Include", new[] { "ItemLabel", "ItemDescription", "IsRequired", "InspectionFormItemTypeEnum", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_FormRequired_True",
                table: "InspectionFormItems",
                columns: new[] { "InspectionFormID", "IsRequired" },
                filter: "IsRequired = 1");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_FormType",
                table: "InspectionFormItems",
                columns: new[] { "InspectionFormID", "InspectionFormItemTypeEnum" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_ItemType",
                table: "InspectionFormItems",
                column: "InspectionFormItemTypeEnum");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_LabelForm",
                table: "InspectionFormItems",
                columns: new[] { "ItemLabel", "InspectionFormID" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_RequiredActive",
                table: "InspectionFormItems",
                columns: new[] { "IsRequired", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionFormItems_UpdatedAtActive",
                table: "InspectionFormItems",
                columns: new[] { "UpdatedAt", "IsActive" });

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Users_UserId",
                table: "Inspections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Users_UserId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_Condition_NotSafe",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_ConditionStartTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_CreatedAtCondition",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_DateRangeCondition",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_FormCondition",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_FormTechnician",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_FormTitleStartTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_InspectionEndTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_NotesStartTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_OdometerReading",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_OdometerStartTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_StartEndTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_StartTimeCondition",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_StartTimeCreatedCovering",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_TechnicianCondition",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_TechnicianEndTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_TechnicianStartTimeCovering",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_UpdatedAtStartTime",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_UserId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_VehicleCondition",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_VehicleOdometer",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_VehicleStartTimeCovering",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_CommentPassed",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_Failed",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_FormItemPassed",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_FormItemPassedRequired",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_FormItemRequiredCovering",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_InspectionCovering",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_InspectionFailed",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_InspectionRequired",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_ItemType",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_LabelPassed",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_PassedRequired",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_RequiredPassed",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionPassFailItems_TypePassed",
                table: "InspectionPassFailItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionFormItems_Active",
                table: "InspectionFormItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionFormItems_CreatedAtForm",
                table: "InspectionFormItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionFormItems_FormActiveCovering",
                table: "InspectionFormItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionFormItems_FormRequired_True",
                table: "InspectionFormItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionFormItems_FormType",
                table: "InspectionFormItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionFormItems_ItemType",
                table: "InspectionFormItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionFormItems_LabelForm",
                table: "InspectionFormItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionFormItems_RequiredActive",
                table: "InspectionFormItems");

            migrationBuilder.DropIndex(
                name: "IX_InspectionFormItems_UpdatedAtActive",
                table: "InspectionFormItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Inspections");

            migrationBuilder.RenameIndex(
                name: "IX_Inspections_FormStartTime",
                table: "Inspections",
                newName: "IX_Inspections_InspectionFormID_InspectionStartTime");

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotFormTitle",
                table: "Inspections",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotFormDescription",
                table: "Inspections",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotItemLabel",
                table: "InspectionPassFailItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotItemInstructions",
                table: "InspectionPassFailItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotItemDescription",
                table: "InspectionPassFailItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_TechnicianID_InspectionStartTime",
                table: "Inspections",
                columns: new[] { "TechnicianID", "InspectionStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_VehicleCondition",
                table: "Inspections",
                column: "VehicleCondition");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_VehicleID_InspectionStartTime",
                table: "Inspections",
                columns: new[] { "VehicleID", "InspectionStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionPassFailItems_Passed",
                table: "InspectionPassFailItems",
                column: "Passed");
        }
    }
}