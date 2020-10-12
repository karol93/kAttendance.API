using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using kAttendance.Infrastructure.DTO;
using kAttendance.Models.Group;
using kAttendance.Models.Person;
using Newtonsoft.Json;
using Xunit;

namespace kAttendance.EndToEndTests.Controllers
{
   [Trait("Controller", "People")]
   public class PeopleControllerTests : BaseControllerTests
   {
      [Fact]
      public async Task GetPeopleForGroup_ShouldReturnOkStatusWithEmptyList_WhenNoPeopleForSpecifiedGroupInDatabase()
      {
         var response = await Client.GetAsync("api/groups/1/people");
         var responseString = await response.Content.ReadAsStringAsync();
         var people = JsonConvert.DeserializeObject<IEnumerable<PersonDto>>(responseString);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
         people.Count().ShouldBeEquivalentTo(0);
      }

      [Fact]
      public async Task GetPeopleForGroup_ShouldReturnOkStatusNotEmptyList_WhenPeopleExistsSpecifiedGroupInDatabase()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var person = await CreatePersonAndReturnDto(group.Id, "Jan Kowalski", 2000);

         var response = await Client.GetAsync($"api/groups/{group.Id}/people");
         var responseString = await response.Content.ReadAsStringAsync();
         var people = JsonConvert.DeserializeObject<IEnumerable<PersonDto>>(responseString);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
         people.Any(p => p.Id == group.Id && p.FullName == person.FullName).Should().BeTrue();
      }

      [Fact]
      public async Task GetPersonForGroup_ShouldReturnNotFoundStatus_WhenPersonWithGivenIdNotExistsInDatabase()
      {
         var response = await Client.GetAsync("api/groups/1/people/1");
         var responseString = await response.Content.ReadAsStringAsync();
         var person = JsonConvert.DeserializeObject<PersonDto>(responseString);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NotFound);
         person.Should().BeNull();
      }

      [Fact]
      public async Task GetPersonForGroup_ShouldReturnOkStatusWithPerson_WhenPersonExsistsWithGivenId()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var newPerson = await CreatePersonAndReturnDto(group.Id, "Jan Kowalski", 2000);

         var response = await Client.GetAsync($"api/groups/{group.Id}/people/{newPerson.Id}");
         var responseString = await response.Content.ReadAsStringAsync();
         var person = JsonConvert.DeserializeObject<PersonDto>(responseString);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
         person.Id.ShouldBeEquivalentTo(newPerson.Id);
      }

      [Fact]
      public async Task CreatePersonForGroup_ShouldReturnBadRequest_WhenModelIsInvalid()
      {
         var response = await CreatePersonAndReturnResponse(1, "", -1);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);
      }

      [Fact]
      public async Task CreatePersonForGroup_ShouldReturnCreatedStatusAndPersonShouldExistsInDatabase_WhenModelIsValid()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var newPerson = await CreatePersonAndReturnDto(group.Id,"Jan Kowalski", 2000);

         var response = await Client.GetAsync($"api/groups/{group.Id}/people/{newPerson.Id}");
         var responseString = await response.Content.ReadAsStringAsync();
         var person = JsonConvert.DeserializeObject<PersonDto>(responseString);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
         person.Id.ShouldBeEquivalentTo(newPerson.Id);
         person.FullName.ShouldBeEquivalentTo(newPerson.FullName);
         person.Year.ShouldBeEquivalentTo(newPerson.Year);
         person.GroupId.ShouldBeEquivalentTo(newPerson.GroupId);
      }

      [Fact]
      public async Task UpdatePersonForGroup_ShouldReturnBadRequest_WhenModelIsInvalid()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var person = await CreatePersonAndReturnDto(group.Id, "Jan Kowalski", 2000);
         var model = new SavePersonModel()
         {
            FullName = "New name",
            Year = 0
         };
         var payload = GetPayload(model);

         var response = await Client.PutAsync($"api/groups/{group.Id}/people/{person.Id}", payload);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);
      }

      [Fact]
      public async Task UpdatePersonForGroup_ShouldReturnNoContentStatusAndUpdatedPersonShouldHasNewValue_WhenModelIsValid()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var person = await CreatePersonAndReturnDto(group.Id, "Jan Kowalski", 2000);
         var model = new SavePersonModel()
         {
            FullName = "New name",
            Year = 2001
         };
         var payload = GetPayload(model);

         var response = await Client.PutAsync($"api/groups/{group.Id}/people/{person.Id}", payload);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NoContent);

         var getUpdatedPersonResponse = await Client.GetAsync($"api/groups/{group.Id}/people/{person.Id}");
         var responseString = await getUpdatedPersonResponse.Content.ReadAsStringAsync();
         var updatedPerson = JsonConvert.DeserializeObject<PersonDto>(responseString);

         getUpdatedPersonResponse.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
         updatedPerson.FullName.ShouldBeEquivalentTo(model.FullName);
         updatedPerson.Year.ShouldBeEquivalentTo(model.Year);
      }

      [Fact]
      public async Task Delete_ShouldReturnBadRequest_WhenNoPersonWithGivenId()
      {
         var groupId = 12;
         var personId = 12;

         var result = await Client.DeleteAsync($"api/groups/{groupId}/people/{personId}");
         result.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.BadRequest);
      }

      [Fact]
      public async Task Delete_ShouldReturnNoContentAndGroupShoulNotExistsInDatabase_WhenGroupExistsWithGivenId()
      {
         var group = await CreateGroupAndReturnDto("Group 1");
         var person = await CreatePersonAndReturnDto(group.Id, "Jan Kowalski", 2000);

         var result = await Client.DeleteAsync($"api/groups/{group.Id}/people/{person.Id}");
         result.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NoContent);

         var notExistsPersonResponse = await Client.GetAsync("api/groups/1/people/1");
         notExistsPersonResponse.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NotFound);
      }
   }
}
