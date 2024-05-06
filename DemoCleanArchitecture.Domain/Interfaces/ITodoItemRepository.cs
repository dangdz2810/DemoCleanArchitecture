using DemoCleanArchitecture.Domain.Entities;

namespace DemoCleanArchitecture.Domain.Interfaces
{
    public interface ITodoItemRepository
    {
        Task<ToDoItem> GetByIdAsync(int id);
        Task<(List<ToDoItem> items, int totalCount)> GetAllAsync(int pageIndex, int pageSize);
        Task<ToDoItem?> GetByTodoItemname(string name);
        Task<ToDoItem> CreateAsync(ToDoItem item);
        Task<ToDoItem> UpdateAsync(int id, ToDoItem item);
        Task DeleteAsync(int id);
        Task<List<ToDoItem>> GetAllByUserIdAsync(int userId);
    }
}
