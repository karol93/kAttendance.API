using kAttendance.Infrastructure.Filters;
using kAttendance.Models.Group;
using kAttendance.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace kAttendance.Controllers
{
   [Route("api/[controller]")]
   public class GroupsController : Controller
   {
      private readonly IGroupService _groupService;

      public GroupsController(IGroupService groupService) => _groupService = groupService;

      [HttpGet]
      public IActionResult GetAll() => Ok(_groupService.GetAll());

      [HttpGet("{id}", Name = "GetById")]
      public IActionResult GetById(int id)
      {
         var group = _groupService.FindById(id);
         if (group == null)
            return NotFound();
         return Ok(group);
      } 

      [HttpPost]
      [ServiceFilter(typeof(ModelValidationFilter))]
      public IActionResult CreateGroup([FromBody] SaveGroupModel model)
      {
         if (!ModelState.IsValid)
            return BadRequest();
         var newGroup = _groupService.Add(model.Name);
         return CreatedAtRoute("GetById", new {id = newGroup.Id}, newGroup);
      }

      [HttpDelete("{id}")]
      public IActionResult DeleteGroup(int id)
      {
         _groupService.Delete(id);
         return NoContent();
      }

      [HttpPut("{id}")]
      [ServiceFilter(typeof(ModelValidationFilter))]
      public IActionResult UpdateGroup(int id, [FromBody] SaveGroupModel model)
      {
         if (!ModelState.IsValid)
            return BadRequest();
         _groupService.Update(id, model.Name);
         return NoContent();
      }
   }
}
