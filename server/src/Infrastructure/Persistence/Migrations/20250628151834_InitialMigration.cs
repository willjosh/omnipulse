using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItem",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeightKG = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompatibleVehicleTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemLocation",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemLocation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServicePrograms",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OEMTag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrimaryMeterType = table.Column<int>(type: "int", nullable: false),
                    SecondaryMeterType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePrograms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTasks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EstimatedLabourHours = table.Column<double>(type: "float", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTasks", x => x.ID);
                    table.CheckConstraint("CK_ServiceTask_EstimatedCost", "EstimatedCost >= 0");
                    table.CheckConstraint("CK_ServiceTask_EstimatedLabourHours", "EstimatedLabourHours >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_User_HireDate", "HireDate >= '1900-01-01' AND HireDate <= GETDATE()");
                    table.CheckConstraint("CK_User_Timestamps", "UpdatedAt >= CreatedAt");
                });

            migrationBuilder.CreateTable(
                name: "VehicleGroups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckListItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionTypeID = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InputType = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                name: "Inventory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryItemID = table.Column<int>(type: "int", nullable: false),
                    InventoryItemLocationID = table.Column<int>(type: "int", nullable: false),
                    QuantityOnHand = table.Column<int>(type: "int", nullable: false),
                    MinStockLevel = table.Column<int>(type: "int", nullable: false),
                    MaxStockLevel = table.Column<int>(type: "int", nullable: false),
                    ReorderPoint = table.Column<int>(type: "int", nullable: false),
                    LastRestockedDate = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Inventory_InventoryItemLocation_InventoryItemLocationID",
                        column: x => x.InventoryItemLocationID,
                        principalTable: "InventoryItemLocation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventory_InventoryItem_InventoryItemID",
                        column: x => x.InventoryItemID,
                        principalTable: "InventoryItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSchedules",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceProgramID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IntervalMileage = table.Column<int>(type: "int", nullable: false),
                    IntervalDays = table.Column<int>(type: "int", nullable: false),
                    IntervalHours = table.Column<int>(type: "int", nullable: false),
                    BufferMileage = table.Column<int>(type: "int", nullable: false),
                    BufferDays = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSchedules", x => x.ID);
                    table.CheckConstraint("CK_ServiceSchedule_BufferDays", "BufferDays >= 0");
                    table.CheckConstraint("CK_ServiceSchedule_BufferMileage", "BufferMileage >= 0");
                    table.CheckConstraint("CK_ServiceSchedule_IntervalDays", "IntervalDays > 0");
                    table.CheckConstraint("CK_ServiceSchedule_IntervalHours", "IntervalHours >= 0");
                    table.CheckConstraint("CK_ServiceSchedule_IntervalMileage", "IntervalMileage > 0");
                    table.ForeignKey(
                        name: "FK_ServiceSchedules_ServicePrograms_ServiceProgramID",
                        column: x => x.ServiceProgramID,
                        principalTable: "ServicePrograms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Make = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    VIN = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LicensePlateExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    VehicleGroupID = table.Column<int>(type: "int", nullable: false),
                    AssignedTechnicianID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Trim = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Mileage = table.Column<double>(type: "float", nullable: false),
                    EngineHours = table.Column<double>(type: "float", nullable: false),
                    FuelCapacity = table.Column<double>(type: "float", nullable: false),
                    FuelType = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchasePrice = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.ID);
                    table.CheckConstraint("CK_Vehicle_EngineHours", "EngineHours >= 0");
                    table.CheckConstraint("CK_Vehicle_FuelCapacity", "FuelCapacity > 0");
                    table.CheckConstraint("CK_Vehicle_Mileage", "Mileage >= 0");
                    table.CheckConstraint("CK_Vehicle_PurchasePrice", "PurchasePrice >= 0");
                    table.CheckConstraint("CK_Vehicle_Year", "Year > 1885 AND Year <= 2100");
                    table.ForeignKey(
                        name: "FK_Vehicles_Users_AssignedTechnicianID",
                        column: x => x.AssignedTechnicianID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleGroups_VehicleGroupID",
                        column: x => x.VehicleGroupID,
                        principalTable: "VehicleGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceScheduleTasks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceScheduleID = table.Column<int>(type: "int", nullable: false),
                    ServiceTaskID = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    ServiceTaskID1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceScheduleTasks", x => x.ID);
                    table.CheckConstraint("CK_ServiceScheduleTask_SequenceNumber", "SequenceNumber > 0");
                    table.ForeignKey(
                        name: "FK_ServiceScheduleTasks_ServiceSchedules_ServiceScheduleID",
                        column: x => x.ServiceScheduleID,
                        principalTable: "ServiceSchedules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceScheduleTasks_ServiceTasks_ServiceTaskID",
                        column: x => x.ServiceTaskID,
                        principalTable: "ServiceTasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceScheduleTasks_ServiceTasks_ServiceTaskID1",
                        column: x => x.ServiceTaskID1,
                        principalTable: "ServiceTasks",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "FuelPurchases",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    PurchasedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdometerReading = table.Column<double>(type: "float", nullable: false),
                    Volume = table.Column<double>(type: "float", nullable: false),
                    PricePerUnit = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    FuelStation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelPurchases", x => x.ID);
                    table.CheckConstraint("CK_FuelPurchase_OdometerReading", "OdometerReading >= 0");
                    table.CheckConstraint("CK_FuelPurchase_PricePerUnit", "PricePerUnit > 0");
                    table.CheckConstraint("CK_FuelPurchase_PurchaseDate", "PurchaseDate >= '2000-01-01' AND PurchaseDate <= GETDATE()");
                    table.CheckConstraint("CK_FuelPurchase_TotalCost", "TotalCost > 0");
                    table.CheckConstraint("CK_FuelPurchase_Volume", "Volume > 0");
                    table.ForeignKey(
                        name: "FK_FuelPurchases_Users_PurchasedByUserId",
                        column: x => x.PurchasedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FuelPurchases_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    IssueNumber = table.Column<int>(type: "int", nullable: false),
                    ReportedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    PriorityLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResolvedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<int>(type: "int", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VehicleID1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.ID);
                    table.CheckConstraint("CK_Issue_ResolvedDate", "ResolvedDate IS NULL OR ResolvedDate >= CreatedAt");
                    table.ForeignKey(
                        name: "FK_Issues_Users_ReportedByUserID",
                        column: x => x.ReportedByUserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issues_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issues_Vehicles_VehicleID1",
                        column: x => x.VehicleID1,
                        principalTable: "Vehicles",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ServiceReminders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    ServiceScheduleID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueMileage = table.Column<double>(type: "float", nullable: false),
                    DueEngineHours = table.Column<double>(type: "float", nullable: false),
                    PriorityLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastNotificationSentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotificationCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceReminders", x => x.ID);
                    table.CheckConstraint("CK_ServiceReminder_CompletedDate", "CompletedDate IS NULL OR (IsCompleted = 1 AND CompletedDate >= CreatedAt)");
                    table.CheckConstraint("CK_ServiceReminder_DueDate", "DueDate >= CreatedAt");
                    table.CheckConstraint("CK_ServiceReminder_DueEngineHours", "DueEngineHours >= 0");
                    table.CheckConstraint("CK_ServiceReminder_DueMileage", "DueMileage >= 0");
                    table.CheckConstraint("CK_ServiceReminder_NotificationCount", "NotificationCount >= 0");
                    table.ForeignKey(
                        name: "FK_ServiceReminders_ServiceSchedules_ServiceScheduleID",
                        column: x => x.ServiceScheduleID,
                        principalTable: "ServiceSchedules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceReminders_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleAlerts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AlertType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AlertLevel = table.Column<int>(type: "int", nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    AcknowledgedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDismissed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleAlerts", x => x.ID);
                    table.CheckConstraint("CK_VehicleAlert_AcknowledgedAt", "AcknowledgedAt IS NULL OR (IsAcknowledged = 1 AND AcknowledgedAt >= CreatedAt)");
                    table.CheckConstraint("CK_VehicleAlert_AcknowledgedByUserID", "AcknowledgedByUserID IS NULL OR IsAcknowledged = 1");
                    table.CheckConstraint("CK_VehicleAlert_ExpiresAt", "ExpiresAt IS NULL OR ExpiresAt > CreatedAt");
                    table.ForeignKey(
                        name: "FK_VehicleAlerts_Users_AcknowledgedByUserID",
                        column: x => x.AcknowledgedByUserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleAlerts_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleAssignment",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    AssignedToUserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnassignedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AssignmentType = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleAssignment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VehicleAssignment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleAssignment_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleDocuments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    UploadedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleDocuments", x => x.ID);
                    table.CheckConstraint("CK_VehicleDocument_ExpiryDate", "ExpiryDate IS NULL OR ExpiryDate >= CreatedAt");
                    table.CheckConstraint("CK_VehicleDocument_FileSize", "FileSize > 0");
                    table.ForeignKey(
                        name: "FK_VehicleDocuments_Users_UploadedByUserID",
                        column: x => x.UploadedByUserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleDocuments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VehicleDocuments_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleImages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    UploadedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleImages", x => x.ID);
                    table.CheckConstraint("CK_VehicleImage_FileSize", "FileSize > 0");
                    table.ForeignKey(
                        name: "FK_VehicleImages_Users_UploadedByUserID",
                        column: x => x.UploadedByUserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleImages_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleInspections",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    InspectionTypeID = table.Column<int>(type: "int", nullable: false),
                    TechnicianID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MileageAtInspection = table.Column<double>(type: "float", nullable: false),
                    OverallStatus = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                name: "VehicleServicePrograms",
                columns: table => new
                {
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    ServiceProgramID = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleServicePrograms", x => new { x.VehicleID, x.ServiceProgramID });
                    table.CheckConstraint("CK_VehicleServiceProgram_AssignedDate", "AssignedDate >= '2000-01-01' AND AssignedDate <= GETDATE()");
                    table.ForeignKey(
                        name: "FK_VehicleServicePrograms_ServicePrograms_ServiceProgramID",
                        column: x => x.ServiceProgramID,
                        principalTable: "ServicePrograms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleServicePrograms_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueAttachments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueID = table.Column<int>(type: "int", nullable: false),
                    UploadedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueAttachments", x => x.ID);
                    table.CheckConstraint("CK_IssueAttachment_FileSize", "FileSize > 0");
                    table.ForeignKey(
                        name: "FK_IssueAttachments_Issues_IssueID",
                        column: x => x.IssueID,
                        principalTable: "Issues",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueAttachments_Users_UploadedByUserID",
                        column: x => x.UploadedByUserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueAttachments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    ServiceReminderID = table.Column<int>(type: "int", nullable: false),
                    AssignedToUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    WorkOrderType = table.Column<int>(type: "int", nullable: false),
                    PriorityLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    ActualCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    EstimatedHours = table.Column<double>(type: "float", nullable: true),
                    ActualHours = table.Column<double>(type: "float", nullable: true),
                    ScheduledStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartOdometer = table.Column<double>(type: "float", nullable: false),
                    EndOdometer = table.Column<double>(type: "float", nullable: true),
                    ServiceReminderID1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.ID);
                    table.CheckConstraint("CK_WorkOrder_ActualCost", "ActualCost IS NULL OR ActualCost >= 0");
                    table.CheckConstraint("CK_WorkOrder_ActualHours", "ActualHours IS NULL OR ActualHours >= 0");
                    table.CheckConstraint("CK_WorkOrder_Dates", "ActualStartDate IS NULL OR ScheduledStartDate IS NULL OR ActualStartDate >= ScheduledStartDate");
                    table.CheckConstraint("CK_WorkOrder_EndOdometer", "EndOdometer IS NULL OR EndOdometer >= StartOdometer");
                    table.CheckConstraint("CK_WorkOrder_EstimatedCost", "EstimatedCost IS NULL OR EstimatedCost >= 0");
                    table.CheckConstraint("CK_WorkOrder_StartOdometer", "StartOdometer >= 0");
                    table.ForeignKey(
                        name: "FK_WorkOrders_ServiceReminders_ServiceReminderID",
                        column: x => x.ServiceReminderID,
                        principalTable: "ServiceReminders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrders_ServiceReminders_ServiceReminderID1",
                        column: x => x.ServiceReminderID1,
                        principalTable: "ServiceReminders",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WorkOrders_Users_AssignedToUserID",
                        column: x => x.AssignedToUserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Vehicles_VehicleID",
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
                    VehicleInspectionID = table.Column<int>(type: "int", nullable: false),
                    CheckListItemID = table.Column<int>(type: "int", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    Status = table.Column<int>(type: "int", nullable: false),
                    TextResponse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiresAttention = table.Column<bool>(type: "bit", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    WorkOrderID = table.Column<int>(type: "int", nullable: false),
                    GeneratedByUserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.WorkOrderID);
                    table.CheckConstraint("CK_Invoice_PaymentDate", "PaymentDate IS NULL OR PaymentDate >= InvoiceDate");
                    table.CheckConstraint("CK_Invoice_TaxAmount", "TaxAmount >= 0");
                    table.CheckConstraint("CK_Invoice_TotalAmount", "TotalAmount >= 0");
                    table.ForeignKey(
                        name: "FK_Invoices_Users_GeneratedByUserID",
                        column: x => x.GeneratedByUserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_WorkOrders_WorkOrderID",
                        column: x => x.WorkOrderID,
                        principalTable: "WorkOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceHistories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleID = table.Column<int>(type: "int", nullable: false),
                    WorkOrderID = table.Column<int>(type: "int", nullable: false),
                    ServiceTaskID = table.Column<int>(type: "int", nullable: false),
                    TechnicianID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MileageAtService = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LabourHours = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WorkOrderID1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceHistories", x => x.ID);
                    table.CheckConstraint("CK_MaintenanceHistory_Cost", "Cost >= 0");
                    table.CheckConstraint("CK_MaintenanceHistory_LabourHours", "LabourHours >= 0");
                    table.CheckConstraint("CK_MaintenanceHistory_MileageAtService", "MileageAtService >= 0");
                    table.CheckConstraint("CK_MaintenanceHistory_ServiceDate", "ServiceDate >= CreatedAt");
                    table.ForeignKey(
                        name: "FK_MaintenanceHistories_ServiceTasks_ServiceTaskID",
                        column: x => x.ServiceTaskID,
                        principalTable: "ServiceTasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaintenanceHistories_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenanceHistories_WorkOrders_WorkOrderID",
                        column: x => x.WorkOrderID,
                        principalTable: "WorkOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaintenanceHistories_WorkOrders_WorkOrderID1",
                        column: x => x.WorkOrderID1,
                        principalTable: "WorkOrders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderIssues",
                columns: table => new
                {
                    WorkOrderID = table.Column<int>(type: "int", nullable: false),
                    IssueID = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderIssues", x => new { x.WorkOrderID, x.IssueID });
                    table.ForeignKey(
                        name: "FK_WorkOrderIssues_Issues_IssueID",
                        column: x => x.IssueID,
                        principalTable: "Issues",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderIssues_WorkOrders_WorkOrderID",
                        column: x => x.WorkOrderID,
                        principalTable: "WorkOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderLineItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkOrderID = table.Column<int>(type: "int", nullable: false),
                    InventoryItemID = table.Column<int>(type: "int", nullable: false),
                    ServiceTaskID = table.Column<int>(type: "int", nullable: false),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false, computedColumnSql: "Quantity * UnitCost"),
                    LaborHours = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderLineItems", x => x.ID);
                    table.CheckConstraint("CK_WorkOrderLineItem_LaborHours", "LaborHours IS NULL OR LaborHours >= 0");
                    table.CheckConstraint("CK_WorkOrderLineItem_Quantity", "Quantity > 0");
                    table.CheckConstraint("CK_WorkOrderLineItem_UnitCost", "UnitCost >= 0");
                    table.ForeignKey(
                        name: "FK_WorkOrderLineItems_InventoryItem_InventoryItemID",
                        column: x => x.InventoryItemID,
                        principalTable: "InventoryItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrderLineItems_ServiceTasks_ServiceTaskID",
                        column: x => x.ServiceTaskID,
                        principalTable: "ServiceTasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrderLineItems_WorkOrders_WorkOrderID",
                        column: x => x.WorkOrderID,
                        principalTable: "WorkOrders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransaction",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryID = table.Column<int>(type: "int", nullable: false),
                    MaintenanceHistoryID = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedByUserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkOrderID = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransaction", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InventoryTransaction_Inventory_InventoryID",
                        column: x => x.InventoryID,
                        principalTable: "Inventory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryTransaction_MaintenanceHistories_MaintenanceHistoryID",
                        column: x => x.MaintenanceHistoryID,
                        principalTable: "MaintenanceHistories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryTransaction_WorkOrders_WorkOrderID",
                        column: x => x.WorkOrderID,
                        principalTable: "WorkOrders",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "749b52f3-45b7-4613-bfa6-1fd13790ef01", null, "FleetManager", "FLEETMANAGER" },
                    { "996d88fd-4d3b-4920-a4ad-40ab4b941b04", null, "Technician", "TECHNICIAN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

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
                name: "IX_FuelPurchases_PurchaseDate",
                table: "FuelPurchases",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPurchases_PurchaseDate_VehicleId",
                table: "FuelPurchases",
                columns: new[] { "PurchaseDate", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_FuelPurchases_PurchasedByUserId",
                table: "FuelPurchases",
                column: "PurchasedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPurchases_ReceiptNumber",
                table: "FuelPurchases",
                column: "ReceiptNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelPurchases_VehicleId",
                table: "FuelPurchases",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPurchases_VehicleId_PurchaseDate",
                table: "FuelPurchases",
                columns: new[] { "VehicleId", "PurchaseDate" });

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
                name: "IX_Inventory_InventoryItemID",
                table: "Inventory",
                column: "InventoryItemID");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_InventoryItemLocationID",
                table: "Inventory",
                column: "InventoryItemLocationID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransaction_InventoryID",
                table: "InventoryTransaction",
                column: "InventoryID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransaction_MaintenanceHistoryID",
                table: "InventoryTransaction",
                column: "MaintenanceHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransaction_WorkOrderID",
                table: "InventoryTransaction",
                column: "WorkOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_GeneratedByUserID",
                table: "Invoices",
                column: "GeneratedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceDate",
                table: "Invoices",
                column: "InvoiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PaymentDate",
                table: "Invoices",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Status",
                table: "Invoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_CreatedAt",
                table: "IssueAttachments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_FileType",
                table: "IssueAttachments",
                column: "FileType");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_IssueID",
                table: "IssueAttachments",
                column: "IssueID");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_UploadedByUserID",
                table: "IssueAttachments",
                column: "UploadedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_UserId",
                table: "IssueAttachments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_Category",
                table: "Issues",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_CreatedAt",
                table: "Issues",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_IssueNumber",
                table: "Issues",
                column: "IssueNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_PriorityLevel",
                table: "Issues",
                column: "PriorityLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_ReportedByUserID",
                table: "Issues",
                column: "ReportedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_Status",
                table: "Issues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_VehicleID",
                table: "Issues",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_VehicleID1",
                table: "Issues",
                column: "VehicleID1");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_CreatedAt",
                table: "MaintenanceHistories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_ServiceDate",
                table: "MaintenanceHistories",
                column: "ServiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_ServiceTaskID",
                table: "MaintenanceHistories",
                column: "ServiceTaskID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_TechnicianID",
                table: "MaintenanceHistories",
                column: "TechnicianID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_UserId",
                table: "MaintenanceHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_VehicleID",
                table: "MaintenanceHistories",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_WorkOrderID",
                table: "MaintenanceHistories",
                column: "WorkOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceHistories_WorkOrderID1",
                table: "MaintenanceHistories",
                column: "WorkOrderID1");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrograms_IsActive",
                table: "ServicePrograms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrograms_Name",
                table: "ServicePrograms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrograms_OEMTag",
                table: "ServicePrograms",
                column: "OEMTag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServicePrograms_OEMTag_IsActive",
                table: "ServicePrograms",
                columns: new[] { "OEMTag", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_DueDate",
                table: "ServiceReminders",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_IsCompleted",
                table: "ServiceReminders",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_IsCompleted_DueDate",
                table: "ServiceReminders",
                columns: new[] { "IsCompleted", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_PriorityLevel",
                table: "ServiceReminders",
                column: "PriorityLevel");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_ServiceScheduleID",
                table: "ServiceReminders",
                column: "ServiceScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_Status",
                table: "ServiceReminders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_Status_DueDate",
                table: "ServiceReminders",
                columns: new[] { "Status", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_VehicleID",
                table: "ServiceReminders",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_VehicleID_DueDate",
                table: "ServiceReminders",
                columns: new[] { "VehicleID", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReminders_VehicleID_IsCompleted",
                table: "ServiceReminders",
                columns: new[] { "VehicleID", "IsCompleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_IsActive",
                table: "ServiceSchedules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_IsActive_Name",
                table: "ServiceSchedules",
                columns: new[] { "IsActive", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_Name",
                table: "ServiceSchedules",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_ServiceProgramID",
                table: "ServiceSchedules",
                column: "ServiceProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_ServiceProgramID_IsActive",
                table: "ServiceSchedules",
                columns: new[] { "ServiceProgramID", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedules_ServiceProgramID_Name",
                table: "ServiceSchedules",
                columns: new[] { "ServiceProgramID", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScheduleTasks_IsMandatory",
                table: "ServiceScheduleTasks",
                column: "IsMandatory");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScheduleTasks_ServiceScheduleID",
                table: "ServiceScheduleTasks",
                column: "ServiceScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScheduleTasks_ServiceScheduleID_IsMandatory",
                table: "ServiceScheduleTasks",
                columns: new[] { "ServiceScheduleID", "IsMandatory" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScheduleTasks_ServiceScheduleID_SequenceNumber",
                table: "ServiceScheduleTasks",
                columns: new[] { "ServiceScheduleID", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScheduleTasks_ServiceScheduleID_ServiceTaskID",
                table: "ServiceScheduleTasks",
                columns: new[] { "ServiceScheduleID", "ServiceTaskID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScheduleTasks_ServiceTaskID",
                table: "ServiceScheduleTasks",
                column: "ServiceTaskID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScheduleTasks_ServiceTaskID1",
                table: "ServiceScheduleTasks",
                column: "ServiceTaskID1");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_Category",
                table: "ServiceTasks",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_Category_IsActive",
                table: "ServiceTasks",
                columns: new[] { "Category", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_EstimatedCost",
                table: "ServiceTasks",
                column: "EstimatedCost");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_EstimatedLabourHours",
                table: "ServiceTasks",
                column: "EstimatedLabourHours");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_IsActive",
                table: "ServiceTasks",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_IsActive_EstimatedCost",
                table: "ServiceTasks",
                columns: new[] { "IsActive", "EstimatedCost" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_Name",
                table: "ServiceTasks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName",
                table: "Users",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName_LastName",
                table: "Users",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_HireDate",
                table: "Users",
                column: "HireDate");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive_Email",
                table: "Users",
                columns: new[] { "IsActive", "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive_HireDate",
                table: "Users",
                columns: new[] { "IsActive", "HireDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastName",
                table: "Users",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_AcknowledgedByUserID",
                table: "VehicleAlerts",
                column: "AcknowledgedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_AlertLevel",
                table: "VehicleAlerts",
                column: "AlertLevel");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_AlertLevel_IsAcknowledged",
                table: "VehicleAlerts",
                columns: new[] { "AlertLevel", "IsAcknowledged" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_AlertType",
                table: "VehicleAlerts",
                column: "AlertType");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_CreatedAt",
                table: "VehicleAlerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_CreatedByUserID",
                table: "VehicleAlerts",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_CreatedByUserID_IsAcknowledged",
                table: "VehicleAlerts",
                columns: new[] { "CreatedByUserID", "IsAcknowledged" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_ExpiresAt",
                table: "VehicleAlerts",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_ExpiresAt_IsAcknowledged",
                table: "VehicleAlerts",
                columns: new[] { "ExpiresAt", "IsAcknowledged" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_IsAcknowledged",
                table: "VehicleAlerts",
                column: "IsAcknowledged");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_IsAcknowledged_IsDismissed",
                table: "VehicleAlerts",
                columns: new[] { "IsAcknowledged", "IsDismissed" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_IsDismissed",
                table: "VehicleAlerts",
                column: "IsDismissed");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_VehicleID",
                table: "VehicleAlerts",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAlerts_VehicleID_IsAcknowledged",
                table: "VehicleAlerts",
                columns: new[] { "VehicleID", "IsAcknowledged" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAssignment_UserId",
                table: "VehicleAssignment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAssignment_VehicleID",
                table: "VehicleAssignment",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDocuments_CreatedAt",
                table: "VehicleDocuments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDocuments_DocumentType",
                table: "VehicleDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDocuments_ExpiryDate",
                table: "VehicleDocuments",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDocuments_UploadedByUserID",
                table: "VehicleDocuments",
                column: "UploadedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDocuments_UserId",
                table: "VehicleDocuments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDocuments_VehicleID",
                table: "VehicleDocuments",
                column: "VehicleID");

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
                name: "IX_VehicleImages_CreatedAt",
                table: "VehicleImages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_UploadedByUserID",
                table: "VehicleImages",
                column: "UploadedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_VehicleID",
                table: "VehicleImages",
                column: "VehicleID");

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

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_AssignedTechnicianID",
                table: "Vehicles",
                column: "AssignedTechnicianID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicensePlate",
                table: "Vehicles",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Status",
                table: "Vehicles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleGroupID",
                table: "Vehicles",
                column: "VehicleGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VIN",
                table: "Vehicles",
                column: "VIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServicePrograms_AssignedDate",
                table: "VehicleServicePrograms",
                column: "AssignedDate");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServicePrograms_IsActive",
                table: "VehicleServicePrograms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServicePrograms_IsActive_AssignedDate",
                table: "VehicleServicePrograms",
                columns: new[] { "IsActive", "AssignedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServicePrograms_ServiceProgramID_IsActive",
                table: "VehicleServicePrograms",
                columns: new[] { "ServiceProgramID", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServicePrograms_VehicleID_IsActive",
                table: "VehicleServicePrograms",
                columns: new[] { "VehicleID", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_IssueID",
                table: "WorkOrderIssues",
                column: "IssueID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_WorkOrderID",
                table: "WorkOrderIssues",
                column: "WorkOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_InventoryItemID",
                table: "WorkOrderLineItems",
                column: "InventoryItemID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_ItemType",
                table: "WorkOrderLineItems",
                column: "ItemType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_ServiceTaskID",
                table: "WorkOrderLineItems",
                column: "ServiceTaskID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_WorkOrderID",
                table: "WorkOrderLineItems",
                column: "WorkOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_WorkOrderID_ItemType",
                table: "WorkOrderLineItems",
                columns: new[] { "WorkOrderID", "ItemType" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ActualStartDate",
                table: "WorkOrders",
                column: "ActualStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_AssignedToUserID",
                table: "WorkOrders",
                column: "AssignedToUserID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_AssignedToUserID_Status",
                table: "WorkOrders",
                columns: new[] { "AssignedToUserID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_CreatedAt",
                table: "WorkOrders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_PriorityLevel",
                table: "WorkOrders",
                column: "PriorityLevel");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ScheduledStartDate",
                table: "WorkOrders",
                column: "ScheduledStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ServiceReminderID",
                table: "WorkOrders",
                column: "ServiceReminderID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ServiceReminderID1",
                table: "WorkOrders",
                column: "ServiceReminderID1");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_Status",
                table: "WorkOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_Status_PriorityLevel",
                table: "WorkOrders",
                columns: new[] { "Status", "PriorityLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_Status_ScheduledStartDate",
                table: "WorkOrders",
                columns: new[] { "Status", "ScheduledStartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_VehicleID",
                table: "WorkOrders",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_VehicleID_Status",
                table: "WorkOrders",
                columns: new[] { "VehicleID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_WorkOrderNumber",
                table: "WorkOrders",
                column: "WorkOrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_WorkOrderType",
                table: "WorkOrders",
                column: "WorkOrderType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "FuelPurchases");

            migrationBuilder.DropTable(
                name: "InspectionAttachments");

            migrationBuilder.DropTable(
                name: "InspectionChecklistResponses");

            migrationBuilder.DropTable(
                name: "InventoryTransaction");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "IssueAttachments");

            migrationBuilder.DropTable(
                name: "ServiceScheduleTasks");

            migrationBuilder.DropTable(
                name: "VehicleAlerts");

            migrationBuilder.DropTable(
                name: "VehicleAssignment");

            migrationBuilder.DropTable(
                name: "VehicleDocuments");

            migrationBuilder.DropTable(
                name: "VehicleImages");

            migrationBuilder.DropTable(
                name: "VehicleServicePrograms");

            migrationBuilder.DropTable(
                name: "WorkOrderIssues");

            migrationBuilder.DropTable(
                name: "WorkOrderLineItems");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CheckListItems");

            migrationBuilder.DropTable(
                name: "VehicleInspections");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "MaintenanceHistories");

            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.DropTable(
                name: "InspectionTypes");

            migrationBuilder.DropTable(
                name: "InventoryItemLocation");

            migrationBuilder.DropTable(
                name: "InventoryItem");

            migrationBuilder.DropTable(
                name: "ServiceTasks");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "ServiceReminders");

            migrationBuilder.DropTable(
                name: "ServiceSchedules");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "ServicePrograms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VehicleGroups");
        }
    }
}