using FluentAssertions;
using kAttendance.Services;
using kAttendance.Services.Interfaces;
using System.Linq;
using Xunit;

namespace kAttendance.UnitTests.Services
{
   [Trait("Service", "Group")]
   public class GroupServiceTests : BaseServiceTests
   {
      private readonly IGroupService _groupService;
      public GroupServiceTests()
      {
         _groupService = new GroupService(_context, _mapper);
      }

      [Fact]
      public void GetAll_ShouldReturnEmptyList_WhenNoData()
      {
         var groups = _groupService.GetAll();

         groups.Count().Should().Be(0);
      }

      [Fact]
      public void GetAll_ShouldReturnNotEmptyList_WhenDataExists()
      {
         _groupService.Add("Group 1");
         
         var groups = _groupService.GetAll().ToList();

         groups.Count().Should().BeGreaterThan(0);
      }

      [Fact]
      public void FindById_ShouldReturnNull_WhenGroupNotExists()
      {
         var group = _groupService.FindById(1);

         group.Should().BeNull();
      }

      [Fact]
      public void FindById_ShouldReturnGroupWithGivenId_WhenGroupExists()
      {
         var expectedGroup = _groupService.Add("Group 1");

         var group = _groupService.FindById(expectedGroup.Id);

         group.Should().NotBeNull();
         group.Id.Should().Be(expectedGroup.Id);
      }
   }
}
