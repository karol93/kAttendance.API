using AutoMapper;
using kAttendance.Infrastructure.EntityFramework;
using kAttendance.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace kAttendance.UnitTests.Services
{
   public abstract class BaseServiceTests
   {
      protected readonly ApplicationDbContext _context;
      protected readonly IMapper _mapper;
      protected BaseServiceTests()
      {
         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("DataSource=:memory:").Options;
         _context = new ApplicationDbContext(options);
         _context.Database.OpenConnection();
         _context.Database.EnsureCreated();

         var mockMapper = new MapperConfiguration(cfg =>
         {
            cfg.AddProfile(new AutoMapperProfile());
         });
         _mapper = mockMapper.CreateMapper();
      }
   }
}