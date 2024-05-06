using DemoCleanArchitecture.Domain.Entities;
using DemoCleanArchitecture.Domain.Interfaces.IServices;
using DemoCleanArchitecture.Domain.Interfaces;
using DemoCleanArchitecture.Application.Exceptions;

namespace DemoCleanArchitecture.Application.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ITodoItemRepository _todoItemRepository;

        public TodoItemService(ITodoItemRepository todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        public async Task<ToDoItem> CreateAsync(ToDoItem item)
        {
            if (item == null)
            {
                throw new DataNotFoundException("item not found");
            }
            var existingTodoItemByName = await _todoItemRepository
                        .GetByTodoItemname(item.Name ?? "");
            if (existingTodoItemByName != null)
            {
                throw new BadRequestException("TodoItem name is exist!!");
            }

            await _todoItemRepository.CreateAsync(item);
            return item;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _todoItemRepository.GetByIdAsync(id);
            if (item == null)
            {
                throw new Exception("not found item with id : " + id);
            }
            await _todoItemRepository.DeleteAsync(id);
        }

        public async Task<(List<ToDoItem> items, int totalCount)> GetAllAsync(int pageIndex, int pageSize)
        {
            return await _todoItemRepository.GetAllAsync(pageIndex, pageSize);
        }

        public async Task<List<ToDoItem>> GetAllByUserIdAsync(int userId)
        {
            var listTodoItem = await _todoItemRepository.GetAllByUserIdAsync(userId);
            if (listTodoItem == null)
            {
                throw new DataNotFoundException("not found");
            }
            return listTodoItem;
        }

        public async Task<ToDoItem> GetByIdAsync(int id)
        {
            return await _todoItemRepository.GetByIdAsync(id);
        }

        public async Task<ToDoItem> UpdateAsync(int id, ToDoItem item)
        {
            if (item is null)
            {
                throw new DataNotFoundException("not found");
            }
            ToDoItem existingItem = await _todoItemRepository.GetByIdAsync(id);
            if (item == null)
            {
                throw new ArgumentException("not found item with id : " + id);
            }
            ToDoItem newToDoItem = new ToDoItem
            {
                Name = item.Name,
                IsComplete = item.IsComplete,
            };
            await _todoItemRepository.UpdateAsync(id, newToDoItem);
            return newToDoItem;
        }
    }
}
