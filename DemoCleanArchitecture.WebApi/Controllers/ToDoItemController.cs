using DemoCleanArchitecture.Domain.Entities;
using DemoCleanArchitecture.Domain.Interfaces.IServices;
using DemoCleanArchitecture.Infrastructure.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoCleanArchitecture.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemController : ControllerBase
    {
        private readonly ITodoItemService _todoItemService;

        public ToDoItemController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }
        [HttpGet]
        [JwtAuthorize("user", "admin")]
        public async Task<IActionResult> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _todoItemService.GetAllAsync(pageIndex, pageSize);
            if (result.items.Count == 0)
            {
                return NotFound();
            }
            var response = new
            {
                TotalCount = result.totalCount,
                Items = result.items
            };
            return Ok(response);
        }

        [HttpGet("todoItemByUser/{userId}")]
        public async Task<IActionResult> GetAllByUserId(int userId)
        {
            List<ToDoItem> listTodoItem = await _todoItemService.GetAllByUserIdAsync(userId);
            if (listTodoItem.Count() == 0)
            {
                return NotFound();
            }
            return Ok(listTodoItem);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var item = await _todoItemService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        [JwtAuthorize("user")]
        public async Task<IActionResult> CreateAsync(ToDoItem item)
        {
            var newItem = await _todoItemService.CreateAsync(item);
            return Ok(newItem);
        }

        [HttpPut("{id}")]
        [JwtAuthorize("user")]
        public async Task<IActionResult> UpdateAsync(int id, ToDoItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            try
            {
                await _todoItemService.UpdateAsync(id, item);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [JwtAuthorize("admin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _todoItemService.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return NoContent();
        }
    }
}
