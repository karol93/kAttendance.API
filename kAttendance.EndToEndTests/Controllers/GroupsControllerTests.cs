using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using kAttendance.Infrastructure.DTO;
using kAttendance.Models.Group;
using Newtonsoft.Json;
using Xunit;

namespace kAttendance.EndToEndTests.Controllers
{
   [Trait("Controller","Group")]
   public class GroupsControllerTests : BaseControllerTests
   {
      [Fact]
      public async Task GetAll_ShouldReturnOkStatusWithEmptyList_WhenNoGroupsInDatabase()
      {
         var response = await Client.GetAsync("api/groups");
         var responseString = await response.Content.ReadAsStringAsync();
         var groups = JsonConvert.DeserializeObject<IEnumerable<GroupDto>>(responseString);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
         groups.Count().ShouldBeEquivalentTo(0);
      }

      [Fact]
      public async Task GetAll_ShouldReturnOkStatusNotEmptyList_WhenAnyGroupExistsInDatabase()
      {
         var group = await CreateGroupAndReturnDto("Group 1");

         var response = await Client.GetAsync("api/groups");
         var responseString = await response.Content.ReadAsStringAsync();
         var groups = JsonConvert.DeserializeObject<IEnumerable<GroupDto>>(responseString);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
         groups.Any(p => p.Id == group.Id && p.Name == group.Name).Should().BeTrue();
      }

      [Fact]
      public async Task GetById_ShouldReturnNotFoundStatus_WhenNoGroupWithGivenId()
      {
         var groupId = 12;

         var response = await Client.GetAsync($"api/groups/{groupId}");

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NotFound);
      }

      [Fact]
      public async Task GetById_ShouldReturnOkStatusWithGroup_WhenGroupExistsWithGivenId()
      {
         var newGroup = await CreateGroupAndReturnDto("Group 1");

         var getGroupResponse = await Client.GetAsync($"api/groups/{newGroup.Id}");
         getGroupResponse.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);

         var group = await GetGroupDtoFromResponse(getGroupResponse);
         group.Id.ShouldBeEquivalentTo(newGroup.Id);
         group.Name.ShouldBeEquivalentTo(newGroup.Name);
      }

      [Fact]
      public async Task CreateGroup_ShouldReturnBadRequestStatus_WhenRequestIsInvalid()
      {
         var response = await CreateGroupAndReturnResponse("");

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);
      }

      [Fact]
      public async Task CreateGroup_ShouldReturnCreatedStatusAndGroupShouldExisitsInDatabase_WhenRequestIsValid()
      {
         var newGroup = await CreateGroupAndReturnDto("Group 1");

         var getGroupResponse = await Client.GetAsync($"api/groups/{newGroup.Id}");
         getGroupResponse.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);

         var group = await GetGroupDtoFromResponse(getGroupResponse);
         group.Id.ShouldBeEquivalentTo(newGroup.Id);
         group.Name.ShouldBeEquivalentTo(newGroup.Name);
      }

      [Fact]
      public async Task DeleteGroup_ShouldReturnBadRequest_WhenNoGroupWithGivenId()
      {
         var groupId = 12;

         var result = await Client.DeleteAsync($"api/groups/{groupId}");
         result.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);
      }

      [Fact]
      public async Task DeleteGroup_ShouldReturnNoContentAndGroupShoulNotExistsInDatabase_WhenGroupExistsWithGivenId()
      {
         var group = await CreateGroupAndReturnDto("Group 1");

         var result = await Client.DeleteAsync($"api/groups/{group.Id}");
         result.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NoContent);

         var getGroupResponse = await Client.GetAsync($"api/groups/{group.Id}");
         getGroupResponse.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NotFound);
      }

      [Fact]
      public async Task UpdateGroup_ShouldReturnBadRequestStatus_WhenRequestIsInvalid()
      {
         var groupId = 12;
         var model = new SaveGroupModel();
         var payload = GetPayload(model);

         var response = await Client.PutAsync($"api/groups/{groupId}", payload);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);
      }

      [Fact]
      public async Task UpdateGroup_ShouldReturnBadRequestStatus_WhenUpdatedGroupNotExists()
      {
         var groupId = 12;
         var model = new SaveGroupModel() {Name = "New name"};
         var payload = GetPayload(model);

         var response = await Client.PutAsync($"api/groups/{groupId}", payload);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);
      }

      [Fact]
      public async Task UpdateGroup_ShouldReturnNoContentStatusAndUpdatedGroupShouldBeHaveNewName_WhenRequestIsValidAndGroupExistsInDatabase()
      {
         var newGroup = await CreateGroupAndReturnDto("Group 1");

         var model = new SaveGroupModel() { Name = "New name" };
         var payload = GetPayload(model);

         var response = await Client.PutAsync($"api/groups/{newGroup.Id}", payload);
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NoContent);

         var getUpdatedGroupResponse = await Client.GetAsync($"api/groups/{newGroup.Id}");
         getUpdatedGroupResponse.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);

         var updatedGroup = await GetGroupDtoFromResponse(getUpdatedGroupResponse);
         updatedGroup.Name.ShouldBeEquivalentTo(model.Name);
         updatedGroup.Name.Should().NotBe(newGroup.Name);
         updatedGroup.Id.ShouldBeEquivalentTo(newGroup.Id);
      }

      private async Task<GroupDto> GetGroupDtoFromResponse(HttpResponseMessage response)
      {
         var responseString = await response.Content.ReadAsStringAsync();
         return JsonConvert.DeserializeObject<GroupDto>(responseString);
      }
   }
}
