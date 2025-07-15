// server.js - Custom JSON Server with pagination
const jsonServer = require("json-server");
const server = jsonServer.create();
const router = jsonServer.router("db.json");
const middlewares = jsonServer.defaults();

// Add custom middleware for pagination wrapper
server.use(middlewares);

// Ensure body parsing for custom routes
server.use(jsonServer.bodyParser);

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

// Create a new vehicle
server.post("/vehicles", (req, res) => {
  const db = router.db;
  const vehicles = db.get("vehicles");
  const newVehicle = req.body;

  // Generate new ID
  const maxId = vehicles
    .value()
    .reduce((max, vehicle) => Math.max(max, vehicle.id || 0), 0);
  newVehicle.id = maxId + 1;

  // Add the new vehicle
  vehicles.push(newVehicle).write();

  res.status(201).json(newVehicle);
});

// Update vehicle
server.put("/vehicles/:id", (req, res) => {
  const db = router.db;
  const vehicleId = parseInt(req.params.id);
  const updatedVehicle = req.body;

  const vehicle = db.get("vehicles").find({ id: vehicleId });

  if (vehicle.value()) {
    vehicle.assign(updatedVehicle).write();
    res.json(vehicle.value());
  } else {
    res.status(404).json({ error: "Vehicle not found" });
  }
});

// Archive vehicle
server.post("/vehicles/deactivate/:id", (req, res) => {
  const db = router.db;
  const vehicleId = parseInt(req.params.id);

  const vehicle = db.get("vehicles").find({ id: vehicleId });

  if (vehicle.value()) {
    vehicle.assign({ IsActive: false }).write();
    res.json({
      message: "Vehicle archived successfully",
      vehicle: vehicle.value(),
    });
  } else {
    res.status(404).json({ error: "Vehicle not found" });
  }
});

// Custom route for issues with pagination wrapper
server.get("/issues", (req, res) => {
  const db = router.db;
  const issues = db.get("issues").value();

  // Get query parameters
  const page = parseInt(req.query.page) || 1;
  const limit = parseInt(req.query.limit) || 10;
  const sortBy = req.query.sortBy || "id";
  const sortOrder = req.query.sortOrder || "asc";
  const search = req.query.search || "";

  // Filter by search if provided
  let filteredIssues = issues;
  if (search) {
    const searchLower = search.toLowerCase();
    filteredIssues = issues.filter(
      issue =>
        (issue.Title &&
          String(issue.Title).toLowerCase().includes(searchLower)) ||
        (issue.Description &&
          String(issue.Description).toLowerCase().includes(searchLower)) ||
        (issue.Status &&
          String(issue.Status).toLowerCase().includes(searchLower)) ||
        (issue.Reporter &&
          String(issue.Reporter).toLowerCase().includes(searchLower)) ||
        (issue.AssignedTechnicianName &&
          String(issue.AssignedTechnicianName)
            .toLowerCase()
            .includes(searchLower)) ||
        (issue.VehicleName &&
          String(issue.VehicleName).toLowerCase().includes(searchLower)),
    );
  }

  // Sort issues
  filteredIssues.sort((a, b) => {
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
  const totalCount = filteredIssues.length;
  const totalPages = Math.ceil(totalCount / limit);
  const startIndex = (page - 1) * limit;
  const endIndex = startIndex + limit;
  const paginatedIssues = filteredIssues.slice(startIndex, endIndex);

  // Return response in your expected format
  res.json({
    Items: paginatedIssues,
    TotalCount: totalCount,
    PageNumber: page,
    PageSize: limit,
    TotalPages: totalPages,
    HasPreviousPage: page > 1,
    HasNextPage: page < totalPages,
  });
});

// Update issue
server.put("/issues/:id", (req, res) => {
  const db = router.db;
  const issueId = parseInt(req.params.id);
  const updatedIssue = req.body;

  const issue = db.get("issues").find({ id: issueId });

  if (issue.value()) {
    // Map VehicleID to VehicleName
    const vehicles = db.get("vehicles").value();
    const vehicle = vehicles.find(v => v.id === updatedIssue.VehicleID);
    updatedIssue.VehicleName = vehicle
      ? vehicle.Name || vehicle.VehicleName
      : "";

    // Map ReportedByUserID to ReportedByUserName
    const users = db.get("technicians").value();
    const reportedByUser = users.find(
      u => u.id === updatedIssue.ReportedByUserID,
    );
    updatedIssue.ReportedByUserName = reportedByUser
      ? `${reportedByUser.FirstName} ${reportedByUser.LastName}`
      : "";

    // Map ResolvedByUserID to ResolvedByUserName (if not null)
    if (updatedIssue.ResolvedByUserID) {
      const resolvedByUser = users.find(
        u => u.id === updatedIssue.ResolvedByUserID,
      );
      updatedIssue.ResolvedByUserName = resolvedByUser
        ? `${resolvedByUser.FirstName} ${resolvedByUser.LastName}`
        : "";
    } else {
      updatedIssue.ResolvedByUserName = null;
    }

    issue.assign(updatedIssue).write();
    res.json(issue.value());
  } else {
    res.status(404).json({ error: "Issue not found" });
  }
});

server.post("/issues", (req, res) => {
  const db = router.db;
  const issues = db.get("issues");
  const vehicles = db.get("vehicles").value();
  const users = db.get("technicians").value();

  // Find next id and IssueNumber
  const allIssues = issues.value();
  const nextId = allIssues.length
    ? Math.max(...allIssues.map(i => i.id)) + 1
    : 1;
  const nextIssueNumber = allIssues.length
    ? Math.max(...allIssues.map(i => i.IssueNumber)) + 1
    : 1001;

  // Look up vehicle name
  const vehicle = vehicles.find(v => v.id === req.body.VehicleID);
  const vehicleName = vehicle ? vehicle.Name || vehicle.VehicleName : "";

  // Look up user name
  const user = users.find(u => u.id === req.body.ReportedByUserID);
  const reportedByUserName = user ? `${user.FirstName} ${user.LastName}` : "";

  // Compose new issue
  const newIssue = {
    id: nextId,
    IssueNumber: nextIssueNumber,
    VehicleID: req.body.VehicleID,
    VehicleName: vehicleName,
    Title: req.body.Title,
    Description: req.body.Description,
    Category: req.body.Category,
    PriorityLevel: req.body.PriorityLevel,
    Status: req.body.Status,
    ReportedByUserID: req.body.ReportedByUserID,
    ReportedByUserName: reportedByUserName,
    ReportedDate: req.body.ReportedDate,
    ResolvedDate: null,
    ResolvedByUserID: null,
    ResolvedByUserName: null,
    ResolutionNotes: null,
  };

  issues.push(newIssue).write();
  res.status(201).json(newIssue);
});

// Get single issue
server.get("/issues/:id", (req, res) => {
  const db = router.db;
  const issue = db
    .get("issues")
    .find({ id: parseInt(req.params.id) })
    .value();

  if (issue) {
    res.json(issue);
  } else {
    res.status(404).json({ error: "Issue not found" });
  }
});

// Create a new vehicle group
server.post("/vehicleGroups", (req, res) => {
  try {
    const db = router.db;
    const vehicleGroups = db.get("vehicleGroups");
    const newVehicleGroup = req.body;

    if (!newVehicleGroup) {
      return res.status(400).json({ error: "Request body is required" });
    }

    const allGroups = vehicleGroups.value();
    const maxId =
      allGroups.length > 0
        ? Math.max(...allGroups.map(group => parseInt(group.id) || 0))
        : 0;

    newVehicleGroup.id = maxId + 1;

    vehicleGroups.push(newVehicleGroup).write();

    res.status(201).json(newVehicleGroup);
  } catch (error) {
    console.error("Error creating vehicle group:", error);
    res.status(500).json({ error: "Internal server error" });
  }
});

// Update vehicle group
server.put("/vehicleGroups/:id", (req, res) => {
  try {
    const db = router.db;
    const vehicleGroups = db.get("vehicleGroups");
    const id = parseInt(req.params.id);
    const updatedData = req.body;

    if (!updatedData) {
      return res.status(400).json({ error: "Request body is required" });
    }

    // Find the vehicle group
    const groupIndex = vehicleGroups
      .value()
      .findIndex(group => group.id === id);

    if (groupIndex === -1) {
      return res.status(404).json({ error: "Vehicle group not found" });
    }

    // Update the vehicle group
    const updatedGroup = {
      ...vehicleGroups.value()[groupIndex],
      ...updatedData,
      id,
    };
    vehicleGroups.value()[groupIndex] = updatedGroup;
    vehicleGroups.write();

    res.json(updatedGroup);
  } catch (error) {
    console.error("Error updating vehicle group:", error);
    res.status(500).json({ error: "Internal server error" });
  }
});

// Custom route for vehicleGroups with pagination wrapper
server.get("/vehicleGroups", (req, res) => {
  const db = router.db;
  const vehicleGroups = db.get("vehicleGroups").value();

  // Get query parameters
  const page = parseInt(req.query.page) || 1;
  const limit = parseInt(req.query.limit) || 10;
  const sortBy = req.query.sortBy || "id";
  const sortOrder = req.query.sortOrder || "asc";
  const search = req.query.search || "";

  // Filter by search if provided
  let filteredGroups = vehicleGroups;
  if (search) {
    const searchLower = search.toLowerCase();
    filteredGroups = vehicleGroups.filter(
      group =>
        group.Name.toLowerCase().includes(searchLower) ||
        (group.Description &&
          group.Description.toLowerCase().includes(searchLower)),
    );
  }

  // Filter by isActive if provided (not implemented yet in the backend)
  if (typeof req.query.isActive !== "undefined") {
    const isActive = req.query.isActive === "true";
    filteredGroups = filteredGroups.filter(
      group => group.IsActive === isActive,
    );
  }

  // Sort vehicle groups
  filteredGroups.sort((a, b) => {
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
  const totalCount = filteredGroups.length;
  const totalPages = Math.ceil(totalCount / limit);
  const startIndex = (page - 1) * limit;
  const endIndex = startIndex + limit;
  const paginatedGroups = filteredGroups.slice(startIndex, endIndex);

  // Return response in expected format
  res.json({
    Items: paginatedGroups,
    TotalCount: totalCount,
    PageNumber: page,
    PageSize: limit,
    TotalPages: totalPages,
    HasPreviousPage: page > 1,
    HasNextPage: page < totalPages,
  });
});

// Get single vehicle group
server.get("/vehicleGroups/:id", (req, res) => {
  const db = router.db;
  const group = db
    .get("vehicleGroups")
    .find({ id: parseInt(req.params.id) })
    .value();

  if (group) {
    res.json(group);
  } else {
    res.status(404).json({ error: "Vehicle group not found" });
  }
});

// Custom route for technicians with pagination wrapper
server.get("/technicians", (req, res) => {
  const db = router.db;
  const technicians = db.get("technicians").value();

  // Get query parameters
  const page = parseInt(req.query.page) || 1;
  const limit = parseInt(req.query.limit) || 10;
  const sortByParam = req.query.sortBy
    ? req.query.sortBy.toLowerCase()
    : "firstname";
  const sortOrder = req.query.sortOrder || "asc";
  const search = req.query.search || "";

  // Map sortBy param to actual field name in db
  const sortByMap = {
    firstname: "FirstName",
    lastname: "LastName",
    hiredate: "HireDate",
    isactive: "IsActive",
    email: "Email",
  };
  const sortBy = sortByMap[sortByParam] || "FirstName";

  // Filter by search if provided
  let filteredTechs = technicians;
  if (search) {
    const searchLower = search.toLowerCase();
    filteredTechs = filteredTechs.filter(
      tech =>
        tech.FirstName.toLowerCase().includes(searchLower) ||
        tech.LastName.toLowerCase().includes(searchLower) ||
        tech.Email.toLowerCase().includes(searchLower),
    );
  }

  // Filter by isActive if provided (not implemented yet)
  if (typeof req.query.isActive !== "undefined") {
    const isActive = req.query.isActive === "true";
    filteredTechs = filteredTechs.filter(tech => tech.IsActive === isActive);
  }

  // Sort technicians
  filteredTechs.sort((a, b) => {
    let aValue = a[sortBy];
    let bValue = b[sortBy];

    if (aValue == null && bValue == null) return 0;
    if (aValue == null) return sortOrder === "asc" ? 1 : -1;
    if (bValue == null) return sortOrder === "asc" ? -1 : 1;

    // For HireDate, sort as date
    if (sortBy === "HireDate") {
      aValue = new Date(aValue);
      bValue = new Date(bValue);
      return sortOrder === "asc" ? aValue - bValue : bValue - aValue;
    }

    // For IsActive, sort as boolean
    if (sortBy === "IsActive") {
      return sortOrder === "asc"
        ? aValue === bValue
          ? 0
          : aValue
            ? -1
            : 1
        : aValue === bValue
          ? 0
          : aValue
            ? 1
            : -1;
    }

    // For string fields
    const aString = String(aValue).toLowerCase();
    const bString = String(bValue).toLowerCase();
    if (aString < bString) return sortOrder === "asc" ? -1 : 1;
    if (aString > bString) return sortOrder === "asc" ? 1 : -1;
    return 0;
  });

  // Calculate pagination
  const totalCount = filteredTechs.length;
  const totalPages = Math.ceil(totalCount / limit);
  const startIndex = (page - 1) * limit;
  const endIndex = startIndex + limit;
  const paginatedTechs = filteredTechs.slice(startIndex, endIndex);

  // Return response in expected format
  res.json({
    Items: paginatedTechs,
    TotalCount: totalCount,
    PageNumber: page,
    PageSize: limit,
    TotalPages: totalPages,
    HasPreviousPage: page > 1,
    HasNextPage: page < totalPages,
  });
});

// Get single technician
server.get("/technicians/:id", (req, res) => {
  const db = router.db;
  const tech = db.get("technicians").find({ id: req.params.id }).value();

  if (tech) {
    res.json(tech);
  } else {
    res.status(404).json({ error: "Technician not found" });
  }
});

// Create a new technician
server.post("/technicians", (req, res) => {
  const db = router.db;
  const technicians = db.get("technicians");
  const newTechnician = req.body;

  const generateUUID = () => {
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(
      /[xy]/g,
      function (c) {
        const r = (Math.random() * 16) | 0;
        const v = c === "x" ? r : (r & 0x3) | 0x8;
        return v.toString(16);
      },
    );
  };

  newTechnician.id = generateUUID();

  if (newTechnician.HireDate && !newTechnician.HireDate.includes("T")) {
    newTechnician.HireDate = new Date(newTechnician.HireDate).toISOString();
  }

  if (
    !newTechnician.FirstName ||
    !newTechnician.LastName ||
    !newTechnician.Email
  ) {
    return res
      .status(400)
      .json({ error: "FirstName, LastName, and Email are required." });
  }

  technicians.push(newTechnician).write();

  res.status(201).json(newTechnician);
});

server.post("/technicians/status/:id", (req, res) => {
  const db = router.db;
  const technicianId = req.params.id;

  const technician = db.get("technicians").find({ id: technicianId });

  if (technician.value()) {
    const currentStatus = technician.value().IsActive;
    const newStatus = !currentStatus;

    technician.assign({ IsActive: newStatus }).write();
    res.json({
      message: `Technician ${newStatus ? "activated" : "deactivated"} successfully`,
      technician: technician.value(),
    });
  } else {
    res.status(404).json({ error: "Technician not found" });
  }
});

// Custom route for inventoryItems with pagination wrapper
server.get("/inventoryItems", (req, res) => {
  const db = router.db;
  const inventoryItems = db.get("inventoryItems").value();

  // Get query parameters
  const page = parseInt(req.query.page) || 1;
  const limit = parseInt(req.query.limit) || 10;
  const sortByParam = req.query.sortBy
    ? req.query.sortBy.toLowerCase()
    : "itemname";
  const sortOrder = req.query.sortOrder || "asc";
  const search = req.query.search || "";

  // Map sortBy param to actual field name in db
  const sortByMap = {
    itemnumber: "ItemNumber",
    itemname: "ItemName",
    description: "Description",
    category: "Category",
    manufacturer: "Manufacturer",
    manufacturerpartnumber: "ManufacturerPartNumber",
    universalproductcode: "UniversalProductCode",
    unitcost: "UnitCost",
    unitcostmeasurementunit: "UnitCostMeasurementUnit",
    supplier: "Supplier",
    weightkg: "WeightKG",
    isactive: "IsActive",
  };
  const sortBy = sortByMap[sortByParam] || "ItemName";

  // Filter by search if provided
  let filteredItems = inventoryItems;
  if (search) {
    const searchLower = search.toLowerCase();
    filteredItems = filteredItems.filter(
      item =>
        (item.ItemNumber &&
          item.ItemNumber.toLowerCase().includes(searchLower)) ||
        (item.ItemName && item.ItemName.toLowerCase().includes(searchLower)) ||
        (item.Description &&
          item.Description.toLowerCase().includes(searchLower)) ||
        (item.Manufacturer &&
          item.Manufacturer.toLowerCase().includes(searchLower)) ||
        (item.Supplier && item.Supplier.toLowerCase().includes(searchLower)),
    );
  }

  // Filter by isActive if provided (not implemented yet)
  if (typeof req.query.isActive !== "undefined") {
    const isActive = req.query.isActive === "true";
    filteredItems = filteredItems.filter(item => item.IsActive === isActive);
  }

  // Sort inventory items
  filteredItems.sort((a, b) => {
    let aValue = a[sortBy];
    let bValue = b[sortBy];

    if (aValue == null && bValue == null) return 0;
    if (aValue == null) return sortOrder === "asc" ? 1 : -1;
    if (bValue == null) return sortOrder === "asc" ? -1 : 1;

    // For number fields
    if (typeof aValue === "number" && typeof bValue === "number") {
      return sortOrder === "asc" ? aValue - bValue : bValue - aValue;
    }

    // For boolean
    if (typeof aValue === "boolean" && typeof bValue === "boolean") {
      return sortOrder === "asc"
        ? aValue === bValue
          ? 0
          : aValue
            ? -1
            : 1
        : aValue === bValue
          ? 0
          : aValue
            ? 1
            : -1;
    }

    // For string fields
    const aString = String(aValue).toLowerCase();
    const bString = String(bValue).toLowerCase();
    if (aString < bString) return sortOrder === "asc" ? -1 : 1;
    if (aString > bString) return sortOrder === "asc" ? 1 : -1;
    return 0;
  });

  // Calculate pagination
  const totalCount = filteredItems.length;
  const totalPages = Math.ceil(totalCount / limit);
  const startIndex = (page - 1) * limit;
  const endIndex = startIndex + limit;
  const paginatedItems = filteredItems.slice(startIndex, endIndex);

  // Return response in expected format
  res.json({
    Items: paginatedItems,
    TotalCount: totalCount,
    PageNumber: page,
    PageSize: limit,
    TotalPages: totalPages,
    HasPreviousPage: page > 1,
    HasNextPage: page < totalPages,
  });
});

// Get single inventory item
server.get("/inventoryItems/:id", (req, res) => {
  const db = router.db;
  const item = db
    .get("inventoryItems")
    .find({ id: parseInt(req.params.id) })
    .value();

  if (item) {
    res.json(item);
  } else {
    res.status(404).json({ error: "Inventory item not found" });
  }
});

// Custom route for serviceTasks with pagination wrapper
server.get("/serviceTasks", (req, res) => {
  const db = router.db;
  const serviceTasks = db.get("serviceTasks").value();

  // Get query parameters
  const page = parseInt(req.query.page) || 1;
  const limit = parseInt(req.query.limit) || 10;
  const sortByParam = req.query.sortBy ? req.query.sortBy.toLowerCase() : "id";
  const sortOrder = req.query.sortOrder || "asc";
  const search = req.query.search || "";

  // Filter by search if provided
  let filteredTasks = serviceTasks;
  if (search) {
    const searchLower = search.toLowerCase();
    filteredTasks = filteredTasks.filter(
      task =>
        (task.Name && task.Name.toLowerCase().includes(searchLower)) ||
        (task.Description &&
          task.Description.toLowerCase().includes(searchLower)),
    );
  }

  // Filter by isActive if provided
  if (typeof req.query.isActive !== "undefined") {
    const isActive = req.query.isActive === "true";
    filteredTasks = filteredTasks.filter(task => task.IsActive === isActive);
  }

  // Sort service tasks
  const sortByMap = {
    id: "id",
    name: "Name",
    description: "Description",
    estimatedlabourhours: "EstimatedLabourHours",
    estimatedcost: "EstimatedCost",
    category: "Category",
    isactive: "IsActive",
  };
  const sortBy = sortByMap[sortByParam] || "id";

  filteredTasks.sort((a, b) => {
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
  const totalCount = filteredTasks.length;
  const totalPages = Math.ceil(totalCount / limit);
  const startIndex = (page - 1) * limit;
  const endIndex = startIndex + limit;
  const paginatedTasks = filteredTasks.slice(startIndex, endIndex);

  // Return response in your expected format
  res.json({
    Items: paginatedTasks,
    TotalCount: totalCount,
    PageNumber: page,
    PageSize: limit,
    TotalPages: totalPages,
    HasPreviousPage: page > 1,
    HasNextPage: page < totalPages,
  });
});

// Get single service task
server.get("/serviceTasks/:id", (req, res) => {
  const db = router.db;
  const task = db
    .get("serviceTasks")
    .find({ id: parseInt(req.params.id) })
    .value();

  if (task) {
    res.json(task);
  } else {
    res.status(404).json({ error: "Service Task not found" });
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
  console.log(`POST /vehicles - Create new vehicle`);
  console.log(`PUT /vehicles/:id - Update vehicle`);
  console.log(`POST /vehicles/deactivate/:id - Archive vehicle`);

  // Issues
  console.log(`GET /issues - Get paginated issues`);
  console.log(`GET /issues?page=1&limit=5 - Get issues with pagination`);
  console.log(`GET /issues?search=issue - Search issues`);
  console.log(`GET /issues/:id - Get single issue`);
  console.log(`PUT /issues/:id - Update issue`);
  console.log(`POST /issues - Create new issue`);

  // Vehicle Groups
  console.log(`GET /vehicleGroups - Get paginated vehicle groups`);
  console.log(
    `GET /vehicleGroups?page=1&limit=5 - Get vehicle groups with pagination`,
  );
  console.log(`GET /vehicleGroups?search=group - Search vehicle groups`);
  console.log(`GET /vehicleGroups/:id - Get single vehicle group`);

  // Technicians
  console.log(`GET /technicians - Get paginated technicians`);
  console.log(
    `GET /technicians?page=1&limit=5 - Get technicians with pagination`,
  );
  console.log(`GET /technicians?search=tech - Search technicians`);
  console.log(
    `GET /technicians?sortBy=firstname - Sort technicians by firstname`,
  );
  console.log(`GET /technicians/:id - Get single technician`);
  console.log(`POST /technicians - Create new technician`);
  console.log(`POST /technicians/deactivate/:id - Deactivate technician`);

  // Inventory Items
  console.log(`GET /inventoryItems - Get paginated inventory items`);
  console.log(
    `GET /inventoryItems?page=1&limit=5 - Get inventory items with pagination`,
  );
  console.log(`GET /inventoryItems?search=oil - Search inventory items`);
  console.log(`GET /inventoryItems/:id - Get single inventory item`);

  // Service Tasks
  console.log(`GET /serviceTasks - Get paginated service tasks`);
  console.log(
    `GET /serviceTasks?page=1&limit=5 - Get service tasks with pagination`,
  );
  console.log(`GET /serviceTasks?search=oil - Search service tasks`);
  console.log(`GET /serviceTasks/:id - Get single service task`);
});

module.exports = server;
