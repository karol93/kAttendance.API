using AutoMapper;
using kAttendance.Core.Domain;
using kAttendance.Infrastructure.DTO;
using kAttendance.Infrastructure.EntityFramework;
using kAttendance.Infrastructure.Exceptions;
using kAttendance.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace kAttendance.Services
{
   public class GroupService : IGroupService
   {
      private readonly ApplicationDbContext _context;
      private readonly IMapper _mapper;

      public GroupService(ApplicationDbContext context, IMapper mapper)
      {
         _context = context;
         _mapper = mapper;
      }

      public GroupDto FindById(int id)
      {
         var group = _context.Groups.Find(id);
         return _mapper.Map<Group, GroupDto>(group);
      }

      public IEnumerable<GroupDto> GetAll()
      {
         var groups = _context.Groups.Include(p=>p.People);
         return _mapper.Map<IEnumerable<Group>, IEnumerable<GroupDto>>(groups);
      }

      public GroupDto Add(string name)
      {
         var group = new Group()
         {
            Name = name
         };

         _context.Groups.Add(group);
         _context.SaveChanges();
         return _mapper.Map<Group, GroupDto>(group);
      }

      public void Update(int id, string name)
      {
         var group = _context.Groups.Find(id);
         if (group == null)
            throw new ServiceException("Nie odnaleziono wskazanej grupy.");
         group.Name = name;
         _context.SaveChanges();
      }

      public void Delete(int id)
      {
         var group = _context.Groups.Find(id);
         if (group == null)
            throw new ServiceException("Nie odnaleziono wskazanej grupy.");
         _context.Groups.Remove(group);
         _context.SaveChanges();
      }
   }
}
