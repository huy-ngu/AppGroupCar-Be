# Vehicle API Documentation

## Overview
API for managing vehicles for drivers. All endpoints require JWT authentication. Driver ID is automatically extracted from the token and cannot be manually provided by the client.

### Base URL
```
/api/vehicle
```

### Authentication
- All endpoints require header: `Authorization: Bearer <token>`

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

---

## Features

? **Create Vehicle**: Automatically uploads 2 images (vehicle + registration document) to Cloudinary  
? **Get by Driver**: Only retrieves non-soft-deleted vehicles  
? **Update**: Validates ownership, supports image updates  
? **Soft Delete**: Marks DeleteAt timestamp instead of hard delete from database  
? **Authorization**: Driver ID automatically extracted from JWT token  
? **Validation**: Prevents duplicate plate numbers, verifies driver existence  

---

## Implementation Notes

- All images are uploaded to Cloudinary and URLs are stored in database
- Driver ID is extracted from token claim `ClaimTypes.NameIdentifier`
- Deletion is soft delete only - adds DeleteAt timestamp without removing record
- Update only modifies fields that are provided (optional fields)
- Ownership verification performed before update/delete operations
- Validation errors include detailed error messages
