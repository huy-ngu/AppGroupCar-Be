# Tai lieu nhanh cho developer moi (May 13, 2026)

Muc tieu: giup dev moi hieu nhanh he thong va quy uoc code, de lam viec dong nhat va nhanh hon.

## 1. Tong quan project

- Muc dich: he thong API cho dat xe/van chuyen/xe buyt, gom cac chuc nang auth, quan ly nguoi dung, xe, tuyen, dat cho, don hang, v.v.
- Module chinh:
  - Auth & User: dang ky/dang nhap/phan quyen
  - Market: offer, booking, shipment
  - Bus: route, showtime, booking, seat
  - Common: location, vehicle, role

## 2. Cau truc thu muc

- Controllers/: HTTP layer. Nhan request, check auth, goi service, tra ApiResponse
- Services/: business logic. Xu ly nghiep vu, validate, thao tac data
- Repositories/: data access. Base repository cho CRUD, paging
- DTO/: request/response/paging param
- Entities/: model DB, navigation property
- AutoMapper/: mapping profile Entity <-> DTO
- Data/: DbContext, mapping bang va quan he
- Middleware/: xu ly loi tap trung
- Helpers/: extension, helper claim
- Exceptions/: exception nghiep vu

## 3. Naming convention

- Class/Interface/Enum: PascalCase (vi du: UserService, IAuthService, OfferStatus)
- Method: PascalCase (vi du: GetByIdAsync)
- Local variable/parameter: camelCase (vi du: userId)
- Private field: \_camelCase (vi du: \_userService)
- DTO: ket thuc bang Dto hoac DTO (vi du: UserDto, CreateOfferRequestDTO)
- DB table: hien tai su dung tieng Anh, PascalCase va so nhieu (vi du: BusRoutes, ShipmentOffers)
- Enum value: PascalCase (vi du: Active, Inactive)

## 4. Quy tac viet Model / Entity

- Co the co navigation property neu can Include/Query lien quan
- Quan he dinh nghia bang Fluent API trong DbContext (uu tien)
- Data annotation chi dung cho co ban (Key, DatabaseGenerated)
- Soft delete: co DeleteAt trong BaseEntity

Quan he thuong gap:

- 1-n: Company -> BusRoute
- n-n: neu can, tao bang join ro rang

## 5. Quy tac Service / Business logic

- Controller khong viet logic nghiep vu
- Service lam:
  - validate input, quyen, rang buoc
  - thao tac DbContext/Repository
  - map Entity -> DTO
  - nem AppException khi loi nghiep vu

## 6. Quy tac Database (EF Core)

- DbContext dat trong Data/ (ApplicationDbContext)
- Fluent API dat trong OnModelCreating
- Dung migration khi thay doi schema
- Mapping table/name/index/constraint tap trung o DbContext

## 7. Vi du chuan

Entity mau:

```csharp
public class Vehicle : BaseEntity
{
    public Guid DriverId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public int SeatCapacity { get; set; }

    // Navigation (neu can truy van kem)
    public ApplicationUser Driver { get; set; } = null!;
}
```

Service mau:

```csharp
public class VehicleService : IVehicleService
{
    private readonly IBaseRepository<Vehicle> _repo;
    private readonly IMapper _mapper;

    public VehicleService(IBaseRepository<Vehicle> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<VehicleDto> GetByIdAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null || entity.DeleteAt != null)
            throw new AppException(404, "Khong tim thay xe");

        return _mapper.Map<VehicleDto>(entity);
    }
}
```

## 8. Quy tac chung

- Clean code: ham ngan gon, ro rang, dung ten bien co nghia
- Khong duplicate logic: dung helper/service chung
- Comment chi khi can lam ro ly do hoac logic phuc tap
- Uu tien DateTime.UtcNow trong logic server
- Tra ve ApiResponse dong nhat cho FE
