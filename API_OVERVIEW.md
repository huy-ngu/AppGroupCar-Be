# API OVERVIEW - AppGroupCar Backend

Cap nhat: 2026-05-14
Muc tieu: tai lieu de lam viec hang ngay, giup khong can doc lai toan bo project khi phat trien API moi.

## 1) Tech stack va startup snapshot

- Runtime: ASP.NET Core Web API (.NET 8)
- ORM/DB: EF Core + SQL Server + NetTopologySuite
- Auth: ASP.NET Identity + JWT Bearer + Google login
- Mapping: AutoMapper
- Background job: Hangfire (MemoryStorage)
- Logging: Serilog
- Media upload: Cloudinary
- Mail: MailKit
- API doc: Swagger

File goc can nho:

- `Program.cs`: DI, middleware pipeline, auth, CORS, swagger, hangfire
- `Data/ApplicationDbContext.cs`: DbSet, relation, constraints, query filters
- `Middleware/ExceptionMiddleware.cs`: format loi JSON toan he thong

## 2) Runtime config can biet

- CORS policy `AllowReactApp` hien chi mo cho `http://localhost:5173`.
- JWT key/expire khai bao trong `appsettings.json` (`Jwt`).
- Claim role su dung key `Roles` (khong phai role mac dinh).
- Hangfire dang dung MemoryStorage (phu hop local/dev, khong phu hop production)
- App se seed role va admin mac dinh luc startup:
  - Roles: `Admin`, `User`, `Driver`, `Company`, `ADMIN_COMPANY`, `STAFF_COMPANY`
  - Admin default: `admin@example.com` / `Admin@123`

## 3) Convention backend dang su dung

### 3.1 Response

- Wrapper chung: `ApiResponse<T>`
- Mac dinh: `Code=200`, `Message="Success"`, `Data=...`

### 3.2 Error

- Nghi?p vu nem `AppException(statusCode, message, details?)`
- `ExceptionMiddleware` convert thanh `ApiResponse<object>` + status code HTTP

### 3.3 Paging

- Input: `PagingParameters` (`PageNumber`, `PageSize`, `Search`)
- Output: `PagedResult<T>` (`Items`, `TotalCount`, `TotalPages`, `HasNextPage`, ...)
- Max page size global: 50

### 3.4 Soft delete

- `BaseEntity` co `DeleteAt`; xoa mem set `DeleteAt = UtcNow`
- Nhieu entity co `HasQueryFilter(DeleteAt == null)` trong DbContext

### 3.5 Claim helpers

- `User.GetUserId()` doc `ClaimTypes.NameIdentifier`
- `User.GetCompanyId()` doc claim `companyId`
- `User.GetFullName()` doc claim `fullname`

## 4) Kien truc code va luong xu ly

Cau truc layer:

- `Controllers`: HTTP layer, authz, bind request, goi service
- `Services`: business logic, validate, ownership checks, transaction
- `Repositories`: base CRUD/paging va auth repo rieng
- `Entities`: model DB + enum status
- `DTO`: request/response/filter/paging
- `AutoMapper`: profile map entity <-> dto

Request flow:

1. Controller nhan request (`FromBody` / `FromForm` / `FromQuery`)
2. Service xu ly nghiep vu + validate
3. Repo/DbContext truy van va luu
4. Tra `ApiResponse<T>`
5. Loi di qua `ExceptionMiddleware`

## 5) Domain map (du de code API moi)

### 5.1 Auth/User domain

Core entities:

- `ApplicationUser` (IdentityUser<Guid> + profile fields)
- `RefreshToken` (rotation + revoke)

Main behavior:

- Register tao user + role `User`
- Login/Google login tao access token + refresh token
- Refresh token co `Used`/`Invalidated` de chan replay
- Forgot password su dung cache 15 phut + enqueue mail qua Hangfire

### 5.2 Location/Search domain

Core entities:

- `Location` co `ParentLocationId`, `LocationType`, `Coordinate`

Main behavior:

- Location search tra ve hierarchy (parent chain)
- `TripSearchService` hop nhat ket qua tu:
  - Bus showtime
  - Offer shared ride
  - Offer truck
- Co scoring theo muc do match diem don/tra (target/sibling/parent)

### 5.3 Market domain

Core relation:

- `Driver` (1-1 voi user)
- `Vehicle` -> thuoc `Driver`
- `Offer` -> thuoc `Driver` + `Vehicle`
- `OfferRoutePoint` -> lo trinh theo `Sequence`
- `SharedRideOfferDetail` (1-1 Offer)
- `CargoOfferDetail` (1-1 Offer)
- `Booking` -> `BookingItem` (shared ride)
- `ShipmentRequest` -> `ShipmentOffer` -> `Shipment`

Main business rules da co:

- Offer moi phai co >= 2 route points
- Driver khong duoc co >1 `OfferStatus.Active` cung luc
- Booking shared ride tru seat vao `AvailableSeats`
- Shipment offer accept se tao `Shipment` status `Created`

### 5.4 Bus domain

Core relation:

- `Company` -> `CompanyUser`
- `Company` -> `CompanyVehicle` -> `VehicleSeatLayout`
- `Company` -> `BusRoute` -> `BusRoutePoint`
- `BusShowtime` thuoc route + vehicle + company
- Tao showtime se auto sinh `Seat` tu `LayoutJson`
- `BusBooking` + `BusBookingSeat` + `SeatStatusHistory`

Main business rules da co:

- Trung showtime theo `(CompanyVehicleId, DepartureDate, DepartureTime)` bi chan
- Booking bus lock seat trong transaction + ghi history
- Seat status co log doi trang thai (`SeatStatusHistory`)

### 5.5 Wallet domain

- Da co schema + constraint trong DB (`Wallet`, `Deposit`, `Withdraw`, `WalletTransaction`, `Payment`)
- Hien chua co controller/service API wallet hoan chinh

## 6) Status/enum cheat sheet

Market:

- `DriverVerificationStatus`: Pending, Active, Inactive, Rejected
- `VehicleStatus`: Pending, Active, Inactive, Maintenance, Rejected
- `OfferStatus`: Active, Paused, Closed, Complete
- `RideRequestStatus`: Open, Matched, Cancelled
- `BookingStatus`: Pending, Confirmed, Cancelled
- `PaymentStatus`: Unpaid, Paid, Refunded
- `ShipmentRequestStatus`: Open, Cancelled, Expired
- `ShipmentOfferStatus`: Pending, Accepted, Rejected
- `ShipmentStatus`: Created, InTransit, Delivered, Cancelled, Return

Bus:

- `CompanyStatus`: Pending, Approved, Rejected, Suspended
- `BusShowtimeStatus`: Scheduled, Active, Delayed, Cancelled, Hidden
- `SeatStatus`: Available, Booked, Disabled
- `BusBookingStatus`: Cash, Paid, Cancelled, Expired

Wallet:

- `WalletStatus`, `DepositStatus`, `WithdrawStatus`, `WalletTransactionStatus`, `PaymentRecordStatus`

## 7) Endpoint inventory (group theo controller)

Ghi chu:

- Prefix `[controller]` => ten controller bo chu `Controller` (vi du `UserController` => `/User`)
- Nhieu route dang legacy (chu hoa/thuong va ten path chua dong nhat)

### 7.1 Core controllers

`AuthController` (`/Auth`)

- `POST /Auth/register` (form)
- `POST /Auth/login`
- `POST /Auth/google/callback`
- `POST /Auth/refresh-token`
- `POST /Auth/logout` (`[Authorize]`)
- `POST /Auth/logout_AllDevices` (`[Authorize]`)
- `POST /Auth/forgot-password`
- `POST /Auth/reset-password`

`UserController` (`/User`)

- `GET /User` (`Admin`)
- `GET /User/{id}` (`Admin,User`)
- `GET /User/me` (`Admin,User,Driver,ADMIN_COMPANY`)
- `POST /User` (`Admin`)
- `PUT /User/{id}` (`Admin,User`)
- `DELETE /User/{id}` (`Admin,User`)
- `POST /User/{id}/restore` (`Admin,User`)
- `POST /User/{id}/change-password` (`Admin,User`)

`RoleController` (`/Role`)

- `GET /Role`
- `POST /Role`
- `DELETE /Role/Id`

`LocationController` (`/Location`)

- `GET /Location/id?id=...`
- `GET /Location` (search)
- `GET /Location/all`
- `POST /Location/Create`

`VehicleController` (`/Vehicle`, class `[Authorize]`)

- `POST /Vehicle/create`
- `GET /Vehicle/my-vehicles`
- `GET /Vehicle/{id}`
- `PUT /Vehicle/{id}`
- `DELETE /Vehicle/{id}`
- `GET /Vehicle` (`[AllowAnonymous]`, search/filter)
- `PATCH /Vehicle/approval/{id}`

`SearchController` (`/api/search`)

- `GET /api/search/trips`

### 7.2 Market controllers

`RideRequestController` (`/RideRequest`)

- `POST /RideRequest/Create`
- `GET /RideRequest/GetAll`
- `GET /RideRequest/GetById?id=...`
- `POST /RideRequest/FilterByNameAsync`
- `POST /RideRequest/FilterByTimeAndLocation`
- `PATCH /RideRequest/id`
- `DELETE /RideRequest?rideRequestId=...`

`MarketDriverController` (`/MarketDriver`)

- `GET /MarketDriver`
- `GET /MarketDriver/{id}`
- `POST /MarketDriver`
- `PATCH /MarketDriver/update`
- `PATCH /MarketDriver/approval` (`Admin`)

`OfferController` (`/Offer`)

- `GET /Offer/shared-ride`
- `GET /Offer/shipment`
- `GET /Offer/me` (`[Authorize]`)
- `GET /Offer/{id}`
- `POST /Offer/shared-ride` (`[Authorize]`)
- `POST /Offer/shipment` (`[Authorize]`)
- `PATCH /Offer/{offerId}` (`[Authorize]`)
- `DELETE /Offer/{id}` (`[Authorize]`)

`OfferRoutePointController` (`/OfferRoutePoint`)

- `GET /OfferRoutePoint`
- `POST /OfferRoutePoint`
- `PUT /OfferRoutePoint/{id}`
- `DELETE /OfferRoutePoint/{id}`

`BookingController` (`/Booking`)

- `GET /Booking`
- `GET /Booking/me` (`[Authorize]`)
- `GET /Booking/id?id=...`
- `POST /Booking`
- `PATCH /Booking`

`BookingItemController` (`/BookingItem`)

- `POST /BookingItem/Create`
- `GET /BookingItem/GetById?id=...`
- `GET /BookingItem/GetAll`
- `PUT /BookingItem/Update`
- `DELETE /BookingItem/Delete?id=...`

`SharedRideOfferDetailController` (`/SharedRideOfferDetail`)

- `GET /SharedRideOfferDetail/get-all`
- `GET /SharedRideOfferDetail/{id}`
- `GET /SharedRideOfferDetail/by-offer/{offerId}`
- `POST /SharedRideOfferDetail/create`
- `PUT /SharedRideOfferDetail/{id}`
- `DELETE /SharedRideOfferDetail/{id}`

`ShipmentRequestController` (`/api/ShipmentRequest`)

- `POST /api/ShipmentRequest/filter`
- `POST /api/ShipmentRequest`
- `GET /api/ShipmentRequest/id?id=...`
- `PATCH /api/ShipmentRequest`

`ShipmentOfferController` (`/api/ShipmentOffer`)

- `POST /api/ShipmentOffer/request` (`Driver`)
- `POST /api/ShipmentOffer/offer` (`[Authorize]`)
- `POST /api/ShipmentOffer/{id}/accept` (`[Authorize]`)
- `POST /api/ShipmentOffer/{id}/driver-accept` (`Driver`)
- `GET /api/ShipmentOffer`
- `GET /api/ShipmentOffer/shipmentRequestId?shipmentRequestId=...` (`[Authorize]`)

`ShipmentController` (`/api/Shipment`)

- `GET /api/Shipment/{id}` (`[Authorize]`)
- `GET /api/Shipment/me` (`[Authorize]`)
- `PATCH /api/Shipment` (`[Authorize]`)
- `PATCH /api/Shipment/compelete` (`Driver`)
- `PATCH /api/Shipment/return` (`Driver`)

### 7.3 Bus controllers

`CompanyController` (`/companies`, `Admin`)

- `POST /companies`
- `GET /companies/{id}`
- `GET /companies`
- `PUT /companies/{id}`
- `DELETE /companies/{id}`
- `GET /companies/pending`
- `PATCH /companies/{id}/status`

`CompanyUserController` (`/company-users`, `ADMIN_COMPANY`)

- `POST /company-users`
- `POST /company-users/leave`
- `GET /company-users`
- `PATCH /company-users/company/{companyId}/users/{userId}/active`

`CompanyVehicleController` (`/company-vehicles`, `ADMIN_COMPANY`)

- `POST /company-vehicles`
- `GET /company-vehicles/{id}`
- `GET /company-vehicles`
- `PUT /company-vehicles/{id}`
- `DELETE /company-vehicles/{id}`

`SeatLayoutController` (`/seat-layouts`)

- `POST /seat-layouts`
- `GET /seat-layouts`
- `GET /seat-layouts/{id}`

`BusRouteController` (`/bus-routes`, `ADMIN_COMPANY`)

- `POST /bus-routes`
- `GET /bus-routes`
- `GET /bus-routes/{id}`
- `PUT /bus-routes/{id}`
- `DELETE /bus-routes/{id}`

`BusShowtimeController` (`/bus-showtimes`, `ADMIN_COMPANY`)

- `POST /bus-showtimes`
- `GET /bus-showtimes`
- `GET /bus-showtimes/{id}`
- `PUT /bus-showtimes/{id}`
- `DELETE /bus-showtimes/{id}`
- `PATCH /bus-showtimes/{id}/status`

`ShowtimeSeatController` (`/showtimes`)

- `GET /showtimes/{id}/seats`

`SeatStatusHistoryController` (`/seat-status-histories`, `STAFF_COMPANY`)

- `PATCH /seat-status-histories/status`
- `GET /seat-status-histories/seat/{seatId}`
- `GET /seat-status-histories/showtime/{showtimeId}`

`BusBookingController` (`/bus-bookings`, `[Authorize]`)

- `POST /bus-bookings`
- `GET /bus-bookings/me`
- `GET /bus-bookings/{id}`

Bus booking payload now includes `PickupLocationId` and `DropoffLocationId`, and the service validates both locations against the selected bus route.

## 8) Ownership va role matrix ngan gon

- `Admin`: quan ly user/role/company approval
- `User`: dat cho, shipment, xem/sua thong tin ca nhan
- `Driver`: tao/quan ly vehicle + offer + shipment offer
- `ADMIN_COMPANY`: quan ly route/showtime/vehicle/staff trong company
- `STAFF_COMPANY`: doi status seat + xem history seat

Ownership rule quan trong trong service:

- Vehicle update/delete: phai la chu xe (`DriverId`)
- Offer update/delete: phai la owner driver
- Company resources (route/showtime/vehicle/user): phu thuoc `companyId` trong token
- Shipment detail/update: user phai la driver hoac customer cua request

## 9) DB/constraint can nho khi mo rong API

Trong `ApplicationDbContext` da co nhieu rang buoc quan trong:

- `CompanyVehicle.PlateNumber` unique (soft-delete aware)
- `BusShowtime` unique `(CompanyVehicleId, DepartureDate, DepartureTime)`
- `Seat` unique `(ShowtimeId, SeatNumber)`
- `BusBookingSeat.SeatId` unique de tranh double booking
- Nhom Wallet co check constraints so du/so tien > 0

Khi them API moi lien quan cac bang tren, can ton trong rang buoc DB de tranh 409/500.

## 10) Development playbook (them API moi nhanh)

### 10.1 Them endpoint CRUD/business moi

1. Tao DTO request/response trong `DTO/RequestDTO/...` va `DTO/ResponseDTO/...`
2. Them method vao interface service (`I*Service`) va implement tai service
3. Validate ownership/role tai service (khong dat business logic o controller)
4. Controller chi bind input + goi service + tra `ApiResponse`
5. Neu can map entity/dto, them AutoMapper profile
6. Neu thay doi schema:
   - update `Entity`
   - update `ApplicationDbContext` (DbSet/Fluent API)
   - tao migration moi

### 10.2 Checklist truoc khi merge

- Co su dung `AppException` cho loi nghiep vu
- Co soft-delete awareness (`DeleteAt == null`) khi query list/detail
- Co paging cho API list
- Co auth/authorize dung role
- Co validate ownership (user/company)
- Co test thu endpoint tren Swagger/Postman

## 11) Known inconsistencies / tech debt can de y

Nhung diem nay khong chan app chay, nhung de gay loi khi FE tich hop hoac mo rong nghiep vu:

1. Route naming chua dong nhat:

- Tr?n `api/...` va non-`api`
- Tr?n `Create/GetById/id` va REST style `{id}`
- Co typo path: `compelete`, `Appoval`

2. Auth coverage chua dong nhat:

- Co endpoint su dung `User.GetUserId()` nhung khong gan `[Authorize]`

3. Mot so logic co dau hieu sai hoac can review gap:

- `OfferController.Update` nhan `offerId` tren route nhung service dung `request.Id`
- `ShipmentOfferService.DriverAcceptAsync` dang load `Offer` bang `driverId`
- `ShipmentOfferService.CreateFromOfferAsync` load `ShipmentRequest` bang `offerId` thay vi `requestId`
- `ShipmentOfferService.GetByRequestId` dang filter theo `ShipmentOffer.Id` thay vi `RequestId`
- `SharedRideOfferDetailService.GetByOfferId` dang lay ban ghi dau tien, chua loc theo `offerId`
- `ShipmentRequestServices.DeleteAsync` dieu kien null-check co the NRE (`data == null && data.DeleteAt != null`)

4. Security/deploy note:

- `appsettings.json` dang chua thong tin secret (JWT key, Cloudinary, Google, email). Can dua ve secret manager/env var truoc khi deploy.

## 12) File map de doc nhanh theo use-case

Neu can sua API theo nghiep vu, uu tien doc theo thu tu:

1. Controller target
2. Interface service + service implementation
3. DTO request/response
4. Entity lien quan + `ApplicationDbContext` mapping
5. AutoMapper profile (neu co)

Map nhanh:

- Auth: `Controllers/AuthController.cs` + `Services/Auth/*`
- User/Role: `Controllers/UserController.cs`, `Controllers/RoleController.cs`, `Services/Auth/UserServices`, `Services/Auth/RoleServices`
- Market Offer/Booking: `Controllers/Market/*`, `Services/Market/*`, `Entities/Market/*`
- Bus operation: `Controllers/Bus/*`, `Services/Bus/*`, `Entities/Bus/*`
- Search: `Controllers/SearchController.cs`, `Services/Search/TripSearchService.cs`

---

Tai lieu nay uu tien cho speed dev, khong thay the Swagger chi tiet payload.
Neu can bo sung tiep, nen them 2 phan:

- OpenAPI contract chi tiet cho tung endpoint (request/response sample)
- Test matrix cho role va ownership (Admin/User/Driver/ADMIN_COMPANY/STAFF_COMPANY)
