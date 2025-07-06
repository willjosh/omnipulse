// server.js - Custom JSON Server with pagination
const jsonServer = require("json-server");
const server = jsonServer.create();
const router = jsonServer.router("db.json");
const middlewares = jsonServer.defaults();

// Add custom middleware for pagination wrapper
server.use(middlewares);

// Custom route for vehicles with pagination wrapper
server.get("/vehicles", (req, res) => {
  const db = router.db;
  const vehicles = db.get("vehicles").value();

  // Get query parameters
  const page = parseInt(req.query.page) || 1;
  const limit = parseInt(req.query.limit) || 10;
  const sortBy = req.query.sortBy || "id";
  const sortOrder = req.query.sortOrder || "asc";
  const search = req.query.search || "";

  // Filter by search if provided
  let filteredVehicles = vehicles;
  if (search) {
    const searchLower = search.toLowerCase();
    filteredVehicles = vehicles.filter(
      vehicle =>
        vehicle.Name.toLowerCase().includes(searchLower) ||
        vehicle.Make.toLowerCase().includes(searchLower) ||
        vehicle.Model.toLowerCase().includes(searchLower) ||
        vehicle.LicensePlate.toLowerCase().includes(searchLower) ||
        vehicle.VIN.toLowerCase().includes(searchLower) ||
        (vehicle.AssignedTechnicianName &&
          vehicle.AssignedTechnicianName.toLowerCase().includes(searchLower)) ||
        vehicle.Location.toLowerCase().includes(searchLower),
    );
  }

  // Sort vehicles
  filteredVehicles.sort((a, b) => {
    let aValue = a[sortBy];
    let bValue = b[sortBy];

    if (aValue == null && bValue == null) return 0;
    if (aValue == null) return sortOrder === "asc" ? 1 : -1;
    if (bValue == null) return sortOrder === "asc" ? -1 : 1;

    if (typeof aValue === "number" && typeof bValue === "number") {
      return sortOrder === "asc" ? aValue - bValue : bValue - aValue;
    }

    const aString = String(aValue).toLowerCase();
    const bString = String(bValue).toLowerCase();

    if (aString < bString) return sortOrder === "asc" ? -1 : 1;
    if (aString > bString) return sortOrder === "asc" ? 1 : -1;
    return 0;
  });

  // Calculate pagination
  const totalCount = filteredVehicles.length;
  const totalPages = Math.ceil(totalCount / limit);
  const startIndex = (page - 1) * limit;
  const endIndex = startIndex + limit;
  const paginatedVehicles = filteredVehicles.slice(startIndex, endIndex);

  // Return response in your expected format
  res.json({
    Items: paginatedVehicles,
    TotalCount: totalCount,
    PageNumber: page,
    PageSize: limit,
    TotalPages: totalPages,
    HasPreviousPage: page > 1,
    HasNextPage: page < totalPages,
  });
});

// Get single vehicle
server.get("/vehicles/:id", (req, res) => {
  const db = router.db;
  const vehicle = db
    .get("vehicles")
    .find({ id: parseInt(req.params.id) })
    .value();

  if (vehicle) {
    res.json(vehicle);
  } else {
    res.status(404).json({ error: "Vehicle not found" });
  }
});

// Use default router for other routes
server.use(router);

const PORT = process.env.PORT || 3000;
server.listen(PORT, () => {
  console.log(`JSON Server is running on http://localhost:${PORT}`);
  console.log(`\nAvailable endpoints:`);
  console.log(`GET /vehicles - Get paginated vehicles`);
  console.log(`GET /vehicles?page=1&limit=5 - Get vehicles with pagination`);
  console.log(`GET /vehicles?search=ford - Search vehicles`);
  console.log(`GET /vehicles?sortBy=Name&sortOrder=desc - Sort vehicles`);
  console.log(`GET /vehicles/:id - Get single vehicle`);
});

module.exports = server;
