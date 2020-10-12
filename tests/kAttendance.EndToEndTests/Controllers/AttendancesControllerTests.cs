using FluentAssertions;
using kAttendance.Infrastructure.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace kAttendance.EndToEndTests.Controllers
{
   [Trait("Controller", "Attendances")]
   public class AttendancesControllerTests : BaseControllerTests
   {
      [Fact]
      public async Task GetAttendancesForGroup_ShouldReturnOkStatusWithEmptyArray_WhenNoAttendancesWithGivenGroupId()
      {
         var groupId = 12;

         var response = await Client.GetAsync($"api/groups/{groupId}/attendances");
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);

         var responseString = await response.Content.ReadAsStringAsync();
         var attendances = JsonConvert.DeserializeObject<IEnumerable<AttendanceDto>>(responseString);

         attendances.Count().ShouldBeEquivalentTo(0);
      }

      [Fact]
      public async Task GetAttendancesForGroup_ShouldReturnOkStatusWithNotEmptyArray_WhenAttendancesWithGivenGroupIdExists()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var person = await CreatePersonAndReturnDto(group.Id,"Jan Kowalski",1990);
         var attendance = await CreateAttendanceAndReturnDto(group.Id, DateTime.Now, new List<int>() { person.Id });

         var response = await Client.GetAsync($"api/groups/{group.Id}/attendances");
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);

         var responseString = await response.Content.ReadAsStringAsync();
         var attendances = JsonConvert.DeserializeObject<IEnumerable<AttendanceDto>>(responseString);

         attendances.Any(p => p.Date == attendance.Date).Should().BeTrue();
      }

      [Fact]
      public async Task GetAttendancesForPerson_ShouldReturnOkStatusWithEmptyArray_WhenNoAttednancesWithPersonIdGiven()
      {
         var personId = 12;

         var response = await Client.GetAsync($"api/people/{personId}/attendances");
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);

         var responseString = await response.Content.ReadAsStringAsync();
         var attendances = JsonConvert.DeserializeObject<IEnumerable<AttendanceDto>>(responseString);

         attendances.Count().ShouldBeEquivalentTo(0);
      }

      [Fact]
      public async Task GetAttendancesForPerson_ShouldReturnOkStatusWithNotEmptyArray_WhenAttednancesWithPersonIdExists()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var person = await CreatePersonAndReturnDto(group.Id, "Jan Kowalski", 1990);
         var attendance = await CreateAttendanceAndReturnDto(group.Id, DateTime.Now, new List<int>() { person.Id });

         var response = await Client.GetAsync($"api/people/{person.Id}/attendances");
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);

         var responseString = await response.Content.ReadAsStringAsync();
         var attendances = JsonConvert.DeserializeObject<IEnumerable<AttendanceDto>>(responseString);

         attendances.Any(p => p.Date == attendance.Date).Should().BeTrue();
      }

      [Fact]
      public async Task GetAttendanceForGroup_ShouldReturnNotFoundStatus_WhenAttendanceWithGivenGroupIdNotExists()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var person = await CreatePersonAndReturnDto(group.Id, "Jan Kowalski", 1990);
         var newAttendance = await CreateAttendanceAndReturnDto(group.Id, DateTime.Now, new List<int>() { person.Id });

         var response = await Client.GetAsync($"api/groups/3/attendances/{newAttendance.Date:yyyy-MM-dd}");
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NotFound);
      }

      [Fact]
      public async Task GetAttendanceForGroup_ShouldReturnNotFoundStatus_WhenAttendanceWithGivenDateNotExists()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var person = await CreatePersonAndReturnDto(group.Id, "Jan Kowalski", 1990);
         await CreateAttendanceAndReturnDto(group.Id, DateTime.Now, new List<int>() { person.Id });

         var response = await Client.GetAsync($"api/groups/{group.Id}/attendances/{DateTime.Now.AddDays(-1):yyyy-MM-dd}");
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NotFound);
      }

      [Fact]
      public async Task GetAttendanceForGroup_ShouldReturnOkStatus_WhenAttendanceWithGivenGroupIdAndDateExists()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var person = await CreatePersonAndReturnDto(group.Id, "Jan Kowalski", 1990);
         var newAttendance = await CreateAttendanceAndReturnDto(group.Id, DateTime.Now, new List<int>() { person.Id });

         var response = await Client.GetAsync($"api/groups/{group.Id}/attendances/{newAttendance.Date:yyyy-MM-dd}");
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);

         var responseString = await response.Content.ReadAsStringAsync();
         var attendance = JsonConvert.DeserializeObject<AttendanceDto>(responseString);
         attendance.Date.ShouldBeEquivalentTo(newAttendance.Date);
      }

      [Fact]
      public async Task CreateAttendanceForGroup_ShouldReturnBadRequestWithMessage_WhenGroupNotExists()
      {
         var groupId = 12;

         var response = await CreateAttendanceAndReturnResponse(groupId, DateTime.Now, new List<int>() {1, 2});
         var responseString = await response.Content.ReadAsStringAsync();

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);
         responseString.Should().Contain($"Group {groupId} not found.");
      }

      [Fact]
      public async Task CreateAttendanceForGroup_ShouldReturnBadRequest_WhenRequestIsInvalid()
      {
         var response = await CreateAttendanceAndReturnResponse(12, DateTime.Now, null);
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);
      }

      // create - should throw exception when person not exsists in given group

      // create - should return ok

      // update

      // delete
   }
}
