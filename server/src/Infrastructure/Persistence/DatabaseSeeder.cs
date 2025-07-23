using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence;

/// <summary>
/// Database seeder for development and testing.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds the database if it's empty.
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="userManager">User manager for creating users</param>
    /// <param name="roleManager">Role manager for creating roles</param>
    public static async Task SeedAsync(OmnipulseDatabaseContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Check if data already exists
        if (await context.Vehicles.AnyAsync() || await context.Users.AnyAsync()) return;

        await SeedUsers(context, userManager, roleManager);
        await SeedVehicleGroups(context);
        await SeedVehicles(context);

        // Save all changes
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsers(OmnipulseDatabaseContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Ensure roles exist
        var roles = new[] { "FleetManager", "Technician" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var users = new List<(User user, string password, string role)>
        {
            (new User
            {
                Id = "5e6e0ab3-3a11-403e-adb9-7a25fe678936",
                UserName = "john.smith@omnipulse.com",
                Email = "john.smith@omnipulse.com",
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Smith",
                HireDate = DateTime.UtcNow.AddYears(-2),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MaintenanceHistories = [],
                IssueAttachments = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                VehicleInspections = [],
                Vehicles = []
            }, "TechPass123!", "Technician"),

            (new User
            {
                Id = "952188a4-dc48-4dad-9c7b-c75da50bb241",
                UserName = "sarah.wilson@omnipulse.com",
                Email = "sarah.wilson@omnipulse.com",
                EmailConfirmed = true,
                FirstName = "Sarah",
                LastName = "Wilson",
                HireDate = DateTime.UtcNow.AddYears(-1),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MaintenanceHistories = [],
                IssueAttachments = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                VehicleInspections = [],
                Vehicles = []
            }, "TechPass123!", "Technician"),

            (new User
            {
                Id = "242b941b-4c3e-48b8-b4bb-9055b224a7cc",
                UserName = "mike.johnson@omnipulse.com",
                Email = "mike.johnson@omnipulse.com",
                EmailConfirmed = true,
                FirstName = "Mike",
                LastName = "Johnson",
                HireDate = DateTime.UtcNow.AddYears(-3),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MaintenanceHistories = [],
                IssueAttachments = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                VehicleInspections = [],
                Vehicles = []
            }, "TechPass123!", "Technician"),

            (new User
            {
                Id = "8a1b2c3d-4e5f-6789-abcd-ef0123456789",
                UserName = "alex.turner@omnipulse.com",
                Email = "alex.turner@omnipulse.com",
                EmailConfirmed = true,
                FirstName = "Alex",
                LastName = "Turner",
                HireDate = DateTime.UtcNow.AddMonths(-6),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MaintenanceHistories = [],
                IssueAttachments = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                VehicleInspections = [],
                Vehicles = []
            }, "TechPass123!", "Technician"),

            (new User
            {
                Id = "f1e2d3c4-b5a6-9788-cdef-012345678901",
                UserName = "admin@omnipulse.com",
                Email = "admin@omnipulse.com",
                EmailConfirmed = true,
                FirstName = "Fleet",
                LastName = "Manager",
                HireDate = DateTime.UtcNow.AddYears(-5),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MaintenanceHistories = [],
                IssueAttachments = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                VehicleInspections = [],
                Vehicles = []
            }, "AdminPass123!", "FleetManager")
        };

        foreach (var (user, password, role) in users)
        {
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }

    private static async Task SeedVehicleGroups(OmnipulseDatabaseContext context)
    {
        var vehicleGroups = new List<VehicleGroup>
        {
            new() { ID = 1, Name = "Group 1", Description = "Primary fleet vehicles", IsActive = true },
            new() { ID = 2, Name = "Group 2", Description = "Secondary fleet vehicles", IsActive = true },
            new() { ID = 3, Name = "Group 3", Description = "Electric vehicles", IsActive = true },
            new() { ID = 4, Name = "Group 4", Description = "Heavy duty vehicles", IsActive = true },
            new() { ID = 5, Name = "Group 5", Description = "Passenger vehicles", IsActive = true }
        };

        await context.VehicleGroups.AddRangeAsync(vehicleGroups);
        await context.SaveChangesAsync();
    }

    private static async Task SeedVehicles(OmnipulseDatabaseContext context)
    {
        var vehicles = new List<Vehicle>
        {
            new() {
                ID = 1,
                Name = "Fleet Van 1",
                Make = "Ford",
                Model = "Transit",
                Year = 2021,
                VIN = "WF0XXXTTGXJY12345",
                LicensePlate = "XYZ789",
                LicensePlateExpirationDate = new DateTime(2025, 1, 31),
                VehicleType = VehicleTypeEnum.VAN,
                VehicleGroupID = 1,
                AssignedTechnicianID = "5e6e0ab3-3a11-403e-adb9-7a25fe678936",
                Trim = "350L High Roof",
                Mileage = 42000,
                EngineHours = 1200,
                FuelCapacity = 80,
                FuelType = FuelTypeEnum.DIESEL,
                PurchaseDate = new DateTime(2021, 2, 15),
                PurchasePrice = 48000,
                Status = VehicleStatusEnum.ACTIVE,
                Location = "Sydney Depot",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                VehicleInspections = []
            },

            new() {
                ID = 2,
                Name = "City Car 2",
                Make = "Toyota",
                Model = "Corolla",
                Year = 2020,
                VIN = "JTDBR32E720123456",
                LicensePlate = "ABC123",
                LicensePlateExpirationDate = new DateTime(2024, 12, 31),
                VehicleType = VehicleTypeEnum.CAR,
                VehicleGroupID = 2,
                AssignedTechnicianID = null,
                Trim = "Ascent Sport",
                Mileage = 35000,
                EngineHours = 900,
                FuelCapacity = 50,
                FuelType = FuelTypeEnum.DIESEL,
                PurchaseDate = new DateTime(2020, 4, 10),
                PurchasePrice = 27000,
                Status = VehicleStatusEnum.ACTIVE,
                Location = "Melbourne Branch",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                VehicleInspections = []
            },

            new() {
                ID = 3,
                Name = "Electric Delivery",
                Make = "Hyundai",
                Model = "Ioniq 5",
                Year = 2023,
                VIN = "KMHDH41EXGU123456",
                LicensePlate = "LMN456",
                LicensePlateExpirationDate = new DateTime(2026, 7, 1),
                VehicleType = VehicleTypeEnum.CAR,
                VehicleGroupID = 3,
                AssignedTechnicianID = "952188a4-dc48-4dad-9c7b-c75da50bb241",
                Trim = "Elite EV",
                Mileage = 12000,
                EngineHours = 300,
                FuelCapacity = 0,
                FuelType = FuelTypeEnum.ELECTRIC,
                PurchaseDate = new DateTime(2023, 7, 10),
                PurchasePrice = 65000,
                Status = VehicleStatusEnum.ACTIVE,
                Location = "Brisbane Hub",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                VehicleInspections = []
            },

            new() {
                ID = 4,
                Name = "Heavy Duty Truck",
                Make = "Isuzu",
                Model = "D-MAX",
                Year = 2019,
                VIN = "MPATFS85JHT123456",
                LicensePlate = "JKL321",
                LicensePlateExpirationDate = new DateTime(2024, 9, 30),
                VehicleType = VehicleTypeEnum.TRUCK,
                VehicleGroupID = 4,
                AssignedTechnicianID = "242b941b-4c3e-48b8-b4bb-9055b224a7cc",
                Trim = "SX Crew Cab",
                Mileage = 110000,
                EngineHours = 3500,
                FuelCapacity = 76,
                FuelType = FuelTypeEnum.DIESEL,
                PurchaseDate = new DateTime(2019, 6, 1),
                PurchasePrice = 54000,
                Status = VehicleStatusEnum.MAINTENANCE,
                Location = "Perth Yard",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                VehicleInspections = []
            },

            new() {
                ID = 5,
                Name = "Family SUV",
                Make = "Mazda",
                Model = "CX-5",
                Year = 2021,
                VIN = "JM0KF4WLA00312345",
                LicensePlate = "QWE654",
                LicensePlateExpirationDate = new DateTime(2025, 9, 5),
                VehicleType = VehicleTypeEnum.CAR,
                VehicleGroupID = 5,
                AssignedTechnicianID = "242b941b-4c3e-48b8-b4bb-9055b224a7cc",
                Trim = "GT SP AWD",
                Mileage = 25000,
                EngineHours = 700,
                FuelCapacity = 58,
                FuelType = FuelTypeEnum.PETROL,
                PurchaseDate = new DateTime(2021, 9, 10),
                PurchasePrice = 42000,
                Status = VehicleStatusEnum.ACTIVE,
                Location = "Adelaide Office",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                VehicleInspections = []
            },

            new() {
                ID = 6,
                Name = "Service Truck 2",
                Make = "Chevrolet",
                Model = "Silverado",
                Year = 2022,
                VIN = "1GCRYSE70NZ123456",
                LicensePlate = "RTY987",
                LicensePlateExpirationDate = new DateTime(2025, 8, 15),
                VehicleType = VehicleTypeEnum.TRUCK,
                VehicleGroupID = 1,
                AssignedTechnicianID = "8a1b2c3d-4e5f-6789-abcd-ef0123456789",
                Trim = "LT Crew Cab",
                Mileage = 18000,
                EngineHours = 450,
                FuelCapacity = 90,
                FuelType = FuelTypeEnum.PETROL,
                PurchaseDate = new DateTime(2022, 8, 20),
                PurchasePrice = 58000,
                Status = VehicleStatusEnum.ACTIVE,
                Location = "Darwin Station",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                VehicleInspections = []
            }
        };

        await context.Vehicles.AddRangeAsync(vehicles);
    }
}