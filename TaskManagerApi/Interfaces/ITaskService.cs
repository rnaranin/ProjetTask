using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerApi.DTOs;

namespace TaskManagerApi.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItemDto>> GetTasksAsync(
            bool? isDone,
            string? search,
            string? sortBy,
            bool desc = false);

        Task<TaskItemDto?> GetTaskByIdAsync(int id);

        Task<TaskItemDto?> CreateTaskAsync(TaskItemCreateDto dto);

        Task<TaskItemDto?> UpdateTaskAsync(int id, TaskItemUpdateDto dto);

        Task<bool?> DeleteTaskAsync(int id);
    }
}