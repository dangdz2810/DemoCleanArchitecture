using DemoCleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCleanArchitecture.Domain.Interfaces.IServices
{
    public interface ITodoItemService
    {
        Task<ToDoItem> GetByIdAsync(int id);
        Task<(List<ToDoItem> items, int totalCount)> GetAllAsync(int pageIndex, int pageSize);
        Task<ToDoItem> CreateAsync(ToDoItem item);
        Task<ToDoItem> UpdateAsync(int id, ToDoItem item);
        Task<List<ToDoItem>> GetAllByUserIdAsync(int userId);
        Task DeleteAsync(int id);
    }
}
