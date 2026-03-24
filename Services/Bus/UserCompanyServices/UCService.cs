using AppGroupCar.Entities.Bus;
using AppGroupCar.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Data;
using Microsoft.EntityFrameworkCore;
using AppGroupCar.DTO.ResponseDTO.Bus.CUResponse;
using AppGroupCar.DTO.RequestDTO.Bus.UCRequest;

namespace AppGroupCar.Services.Bus.UserCompanyServices
{
    public class UCService : IUCService
    {
            private readonly IBaseRepository<CompanyUser> _companyRepository;
            private readonly IMapper _mapper;
            private readonly UserManager<ApplicationUser>_userManager;
            protected readonly ApplicationDbContext _context;


            public UCService(
                IBaseRepository<CompanyUser> companyRepository,
                IMapper mapper,
                UserManager<ApplicationUser> userManager,
                ApplicationDbContext context)
            {
                _companyRepository = companyRepository;
                _mapper = mapper;
                _userManager = userManager;
                _context = context;
            }

        


    }
}
