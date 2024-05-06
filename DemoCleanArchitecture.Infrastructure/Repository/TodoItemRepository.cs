using DemoCleanArchitecture.Domain.Entities;
using DemoCleanArchitecture.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using DemoCleanArchitecture.Domain;

namespace DemoCleanArchitecture.Infrastructure.Repository
{
    public class TodoItemRepository : ITodoItemRepository

    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TodoItemRepository(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitofWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitofWork = unitofWork;
        }

        public async Task<ToDoItem> CreateAsync(ToDoItem item)
        {
            // Lấy id của người dùng từ HttpContext.Items
            var userId = (int)_httpContextAccessor.HttpContext.Items["UserId"];

            // Gán userId cho trường userId của ToDoItem
            item.UserId = userId;

            await _unitofWork.Repository<ToDoItem>().CreateAsync(item);
            await _unitofWork.Complete();

            return item;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _unitofWork.Repository<ToDoItem>().GetByIdAsync(id);
            await _unitofWork.Repository<ToDoItem>().DeleteAsync(item);
            await _unitofWork.Complete();
        }

        public async Task<(List<ToDoItem> items, int totalCount)> GetAllAsync(int pageIndex, int pageSize)
        {
            var allItems = await _unitofWork.Repository<ToDoItem>().GetAllAsync();

            var totalCount = allItems.Count;
            var items = allItems.Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            return (items, totalCount);
        }

        public async Task<List<ToDoItem>> GetAllByUserIdAsync(int userId)
        {
            var TodoItemByUser = await _unitofWork.Repository<ToDoItem>().GetAll(filter: td => td.UserId == userId);
            return (List<ToDoItem>)TodoItemByUser;
        }

        public async Task<ToDoItem> GetByIdAsync(int id)
        {
            return await _unitofWork.Repository<ToDoItem>().GetByIdAsync(id);
        }

        public async Task<ToDoItem?> GetByTodoItemname(string name)
        {
            return await _unitofWork.Repository<ToDoItem>().Get(filter: td => td.Name == name);
        }

        public async Task<ToDoItem> UpdateAsync(int id, ToDoItem item)
        {
            ToDoItem existingItem = await _unitofWork.Repository<ToDoItem>().GetByIdAsync(id);
            if (existingItem == null)
            {
                throw new ArgumentException("not found item with id : " + id);
            }

            existingItem.Name = item.Name;
            existingItem.IsComplete = item.IsComplete;
            await _unitofWork.Repository<ToDoItem>().UpdateAsync(existingItem);
            await _unitofWork.Complete();
            return existingItem;
        }
    }
}
