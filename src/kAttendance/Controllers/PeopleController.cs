using kAttendance.Infrastructure.Extensions;
using kAttendance.Infrastructure.Filters;
using kAttendance.Models.Person;
using kAttendance.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace kAttendance.Controllers
{
   [Route("api/groups/{groupId}/[controller]")]
   public class PeopleController : Controller
   {
      private readonly IPersonService _personService;

      public PeopleController(IPersonService personService) => _personService = personService;

      [HttpGet]
      public IActionResult GetPeopleForGroup(int groupId) => Ok(_personService.GetPersonByGroupId(groupId));

      [HttpGet("{id}", Name = "GetPersonForGroup")]
      public IActionResult GetPersonForGroup(int groupId, int id)
      {
         var person = _personService.FindByIdAndGroupId(id, groupId);
         if (person == null)
            return NotFound();
         return Ok(person);
      }

      [HttpPost()]
      [ServiceFilter(typeof(ModelValidationFilter))]
      public IActionResult CreatePersonForGroup(int groupId, [FromBody] SavePersonModel model)
      {
         var newPerson = _personService.CreatePerson(model.FullName, model.Year, groupId, model.Email, model.PhoneNumber);
         return CreatedAtRoute("GetPersonForGroup", new { groupId = groupId, id = newPerson.Id }, newPerson);
      }

      [HttpPut("{id}")]
      [ServiceFilter(typeof(ModelValidationFilter))]
      public IActionResult UpdatePersonForGroup(int id, [FromBody] SavePersonModel model)
      {
         if (!ModelState.IsValid)
            return BadRequest();

         _personService.Update(id, model.FullName, model.Year, model.Email, model.PhoneNumber);
         return NoContent();
      }

      [HttpDelete("{id}")]
      public IActionResult Delete(int id)
      {
         _personService.Delete(id);
         return NoContent();
      }

   }
}
