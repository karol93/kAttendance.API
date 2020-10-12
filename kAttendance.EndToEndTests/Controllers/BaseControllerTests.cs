using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using kAttendance.Infrastructure.DTO;
using kAttendance.Models.Attendance;
using kAttendance.Models.Group;
using kAttendance.Models.Person;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace kAttendance.EndToEndTests.Controllers
{
   public class BaseControllerTests
   {
      protected readonly TestServer Server;
      protected readonly HttpClient Client;

      protected BaseControllerTests()
      {
         Server = new TestServer(new WebHostBuilder()
            .UseStartup<TestStartup>());
         Client = Server.CreateClient();
      }

      protected static StringContent GetPayload(object data)
      {
         var json = JsonConvert.SerializeObject(data);

         return new StringContent(json, Encoding.UTF8, "application/json");
      }


      protected async Task<HttpResponseMessage> CreateGroupAndReturnResponse(string name)
      {
         return await CreateGroup(name);
      }

      protected async Task<GroupDto> CreateGroupAndReturnDto(string name)
      {
         var response = await CreateGroup(name);
         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Created);

         var groupDto = JsonConvert.DeserializeObject<GroupDto>(await response.Content.ReadAsStringAsync());
         groupDto.Should().NotBeNull();
         groupDto.Name.ShouldBeEquivalentTo(name);

         return groupDto;
      }


      private async Task<HttpResponseMessage> CreateGroup(string name)
      {
         var model = new SaveGroupModel()
         {
            Name = name
         };

         var payload = GetPayload(model);

         var response = await Client.PostAsync($"api/groups", payload);
         return response;
      }

      protected async Task<AttendanceDto> CreateAttendanceAndReturnDto(int groupId, DateTime date, List<int> peopleIds)
      {
         var response = await CreateAttendance(groupId, date, peopleIds);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Created);

         var attendanceDto = JsonConvert.DeserializeObject<AttendanceDto>(await response.Content.ReadAsStringAsync());
         attendanceDto.Should().NotBeNull();
         attendanceDto.Date.ShouldBeEquivalentTo(date);
         return attendanceDto;
      }

      protected async Task<HttpResponseMessage> CreateAttendanceAndReturnResponse(int groupId, DateTime date,
         List<int> peopleIds)
      {
         return await CreateAttendance(groupId, date, peopleIds);
      }

      private async Task<HttpResponseMessage> CreateAttendance(int groupId, DateTime date, List<int> peopleIds)
      {
         var model = new SaveAttendanceModel()
         {
            Date = date,
            PeopleIds = peopleIds
         };
         var payload = GetPayload(model);

         var response = await Client.PostAsync($"api/groups/{groupId}/attendances", payload);
         return response;
      }

      protected async Task<PersonDto> CreatePersonAndReturnDto(int groupId, string fullName, int year)
      {
         var response = await CreatePerson(groupId, fullName, year);

         response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Created);

         var personDto = JsonConvert.DeserializeObject<PersonDto>(await response.Content.ReadAsStringAsync());
         personDto.Should().NotBeNull();
         personDto.FullName.ShouldBeEquivalentTo(fullName);
         personDto.Year.ShouldBeEquivalentTo(year);
         personDto.GroupId.ShouldBeEquivalentTo(groupId);

         return personDto;
      }

      protected async Task<HttpResponseMessage> CreatePersonAndReturnResponse(int groupId, string fullName, int year)
      {
         return await CreatePerson(groupId, fullName, year);
      }

      private async Task<HttpResponseMessage> CreatePerson(int groupId, string fullName, int year)
      {
         var model = new SavePersonModel()
         {
            FullName =  fullName,
            Year = year,
         };
         var payload = GetPayload(model);

         var response = await Client.PostAsync($"api/groups/{groupId}/people", payload);
         return response;
      }
   }
}
