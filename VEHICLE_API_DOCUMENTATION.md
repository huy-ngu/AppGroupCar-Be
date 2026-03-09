# Vehicle API Documentation

## Overview
API for managing vehicles for drivers. All endpoints require JWT authentication except Search endpoint. Driver ID is automatically extracted from the token and cannot be manually provided by the client.

### Base URL
```
/api/vehicle
```

### Authentication
- Most endpoints require header: `Authorization: Bearer <token>`
- Search endpoint allows anonymous access

---

## Endpoints

### 1. Create New Vehicle
**POST** `/api/vehicle/create`

**Headers:**
```
Authorization: Bearer <token>
Content-Type: multipart/form-data
```

**Request Body (Form Data):**
```
- PlateNumber (string, required): Vehicle plate number. Max 20 characters
- Brand (string, required): Vehicle brand. Max 50 characters
- SeatCapacity (integer, required): Number of seats. 1-50
- VehicleType (enum, required): Vehicle type. Values: 1 (Car), 2 (Truck)
- UrlImage (file, required): Vehicle image
- RegistrationDocumentUrl (file, required): Registration inspection document
```

**Example Response (200):**
```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "driverId": "660e8400-e29b-41d4-a716-446655440001",
    "plateNumber": "ABC123",
    "brand": "Toyota",
    "seatCapacity": 4,
    "vehicleType": 1,
    "status": 1,
    "urlImage": "https://cloudinary.com/image1.jpg",
    "registrationDocumentUrl": "https://cloudinary.com/image2.jpg",
    "createdAt": "2024-01-15T10:30:00Z",
    "lastUpdateAt": null
  },
  "message": "Vehicle created successfully"
}
```

---

### 2. Get All Vehicles of Current Driver
**GET** `/api/vehicle/my-vehicles`

**Headers:**
```
Authorization: Bearer <token>
```

**Example Response (200):**
```json
{
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "driverId": "660e8400-e29b-41d4-a716-446655440001",
      "plateNumber": "ABC123",
      "brand": "Toyota",
      "seatCapacity": 4,
      "vehicleType": 1,
      "status": 1,
      "urlImage": "https://cloudinary.com/image1.jpg",
      "registrationDocumentUrl": "https://cloudinary.com/image2.jpg",
      "createdAt": "2024-01-15T10:30:00Z",
      "lastUpdateAt": null
    }
  ],
  "message": "Vehicles retrieved successfully"
}
```

---

### 3. Get Vehicle Details by ID
**GET** `/api/vehicle/{id}`

**Headers:**
```
Authorization: Bearer <token>
```

**Path Parameters:**
- `id` (UUID): Vehicle ID

**Example Response (200):**
```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "driverId": "660e8400-e29b-41d4-a716-446655440001",
    "plateNumber": "ABC123",
    "brand": "Toyota",
    "seatCapacity": 4,
    "vehicleType": 1,
    "status": 1,
    "urlImage": "https://cloudinary.com/image1.jpg",
    "registrationDocumentUrl": "https://cloudinary.com/image2.jpg",
    "createdAt": "2024-01-15T10:30:00Z",
    "lastUpdateAt": null
  },
  "message": "Vehicle retrieved successfully"
}
```

---

### 4. Update Vehicle Information
**PUT** `/api/vehicle/{id}`

**Headers:**
```
Authorization: Bearer <token>
Content-Type: multipart/form-data
```

**Path Parameters:**
- `id` (UUID): Vehicle ID to update

**Request Body (Form Data - all optional):**
```
- PlateNumber (string, optional): Vehicle plate number. Max 20 characters
- Brand (string, optional): Vehicle brand. Max 50 characters
- SeatCapacity (integer, optional): Number of seats. 1-50
- VehicleType (enum, optional): Vehicle type. Values: 1 (Car), 2 (Truck)
- Status (enum, optional): Vehicle status. Values: 1 (Active), 2 (Inactive), 3 (Maintenance)
- UrlImage (file, optional): New vehicle image
- RegistrationDocumentUrl (file, optional): New registration inspection document
```

**Example Response (200):**
```json
{
  "data": null,
  "message": "Vehicle updated successfully"
}
```

---

### 5. Delete Vehicle (Soft Delete)
**DELETE** `/api/vehicle/{id}`

**Headers:**
```
Authorization: Bearer <token>
```

**Path Parameters:**
- `id` (UUID): Vehicle ID to delete

**Example Response (200):**
```json
{
  "data": null,
  "message": "Vehicle deleted successfully"
}
```

---

### 6. Search and Filter Vehicles with Pagination
**GET** `/api/vehicle/search`

**Headers:**
```
Authorization: Bearer <token> (Optional - allows anonymous access)
```

**Query Parameters:**
```
- driverName (string, optional): Search by driver name (partial match, case-sensitive)
- brand (string, optional): Search by vehicle brand (partial match, case-sensitive)
- seatCapacity (integer, optional): Filter by exact seat capacity (1-50)
- vehicleType (integer, optional): Filter by vehicle type. Values: 1 (Car), 2 (Truck)
- status (integer, optional): Filter by status. Values: 1 (Active), 2 (Inactive), 3 (Maintenance)
- pageNumber (integer, optional): Page number. Default: 1
- pageSize (integer, optional): Items per page. Default: 10. Max: 50
```

**Example Requests:**
```
GET /api/vehicle/search?pageNumber=1&pageSize=10
GET /api/vehicle/search?driverName=John&pageNumber=1
GET /api/vehicle/search?brand=Toyota&vehicleType=1&pageNumber=1
GET /api/vehicle/search?status=1&seatCapacity=4&pageNumber=1&pageSize=20
GET /api/vehicle/search?driverName=John&brand=Honda&seatCapacity=7&vehicleType=1&status=1&pageNumber=1&pageSize=10
```

**Example Response (200):**
```json
{
  "data": {
    "items": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440000",
        "driverId": "660e8400-e29b-41d4-a716-446655440001",
        "plateNumber": "ABC123",
        "brand": "Toyota",
        "seatCapacity": 4,
        "vehicleType": 1,
        "status": 1,
        "urlImage": "https://cloudinary.com/image1.jpg",
        "registrationDocumentUrl": "https://cloudinary.com/image2.jpg",
        "createdAt": "2024-01-15T10:30:00Z",
        "lastUpdateAt": null
      },
      {
        "id": "660e8400-e29b-41d4-a716-446655440002",
        "driverId": "660e8400-e29b-41d4-a716-446655440001",
        "plateNumber": "XYZ789",
        "brand": "Toyota",
        "seatCapacity": 7,
        "vehicleType": 1,
        "status": 1,
        "urlImage": "https://cloudinary.com/image3.jpg",
        "registrationDocumentUrl": "https://cloudinary.com/image4.jpg",
        "createdAt": "2024-01-14T09:20:00Z",
        "lastUpdateAt": null
      }
    ],
    "totalCount": 2,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  },
  "message": "Vehicles retrieved successfully"
}
```

---

## Enums

### VehicleType
- `1` = Car
- `2` = Truck

### VehicleStatus
- `1` = Active
- `2` = Inactive
- `3` = Maintenance

---

## Error Responses

### 400 Bad Request
```json
{
  "message": "Plate number already exists"
}
```

### 403 Forbidden
```json
{
  "message": "You don't have permission to update this vehicle"
}
```

### 404 Not Found
```json
{
  "message": "Vehicle not found"
}
```

### 401 Unauthorized (for protected endpoints)
```json
{
  "message": "Unauthorized"
}
```

---

## Features

? **Create Vehicle**: Automatically uploads 2 images (vehicle + registration document) to Cloudinary  
? **Get by Driver**: Only retrieves non-soft-deleted vehicles owned by current driver  
? **Update**: Validates ownership, supports image updates  
? **Soft Delete**: Marks DeleteAt timestamp instead of hard delete from database  
? **Authorization**: Driver ID automatically extracted from JWT token  
? **Search & Filter**: Advanced search with pagination support  
? **Validation**: Prevents duplicate plate numbers, verifies driver existence  

---

## Search and Filter Logic

### Search Parameters (Partial Match - Case Sensitive)
- **driverName**: Searches driver full name (contains comparison)
- **brand**: Searches vehicle brand (contains comparison)

### Filter Parameters (Exact Match)
- **seatCapacity**: Exact match for number of seats
- **vehicleType**: 1 (Car) or 2 (Truck)
- **status**: 1 (Active), 2 (Inactive), or 3 (Maintenance)

### Pagination
- **pageNumber**: Default 1, must be >= 1
- **pageSize**: Default 10, maximum 50

### Ordering
- Results are ordered by CreatedAt in descending order (newest first)

### Response Includes
- **items**: Array of vehicles matching criteria
- **totalCount**: Total number of matching vehicles (across all pages)
- **pageNumber**: Current page number
- **pageSize**: Items per page
- **totalPages**: Total number of pages available
- **hasPreviousPage**: Boolean indicating if previous page exists
- **hasNextPage**: Boolean indicating if next page exists

---

## Filtering Examples

### Get all active vehicles
```
GET /api/vehicle/search?status=1
```

### Get all cars with 4 seats
```
GET /api/vehicle/search?vehicleType=1&seatCapacity=4
```

### Search for vehicles by driver name and filter by brand
```
GET /api/vehicle/search?driverName=John&brand=Toyota
```

### Get page 2 with 20 items per page
```
GET /api/vehicle/search?pageNumber=2&pageSize=20
```

### Complex filter: active Toyota cars with 4 seats, page 1
```
GET /api/vehicle/search?brand=Toyota&vehicleType=1&seatCapacity=4&status=1&pageNumber=1&pageSize=10
```

---

## Implementation Notes

- All images are uploaded to Cloudinary and URLs are stored in database
- Driver ID is extracted from token claim `ClaimTypes.NameIdentifier`
- Deletion is soft delete only - adds DeleteAt timestamp without removing record
- Update only modifies fields that are provided (optional fields)
- Ownership verification performed before update/delete operations
- Search endpoint allows public access for vehicle discovery
- Only non-soft-deleted vehicles are returned in all queries
- Pagination limits: maximum page size is 50 items per page
- Search filters are combined with AND logic (all specified filters must match)
