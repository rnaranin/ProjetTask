using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.Interfaces;
using TaskManagerApi.Models;

namespace TaskManagerApi.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly TaskDbContext _dbContext;

        public TaskItemRepository(TaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _dbContext.Tasks.ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _dbContext.Tasks.FindAsync(id);
        }

        public async Task AddAsync(TaskItem taskItem)
        {
            await _dbContext.Tasks.AddAsync(taskItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem taskItem)
        {
            _dbContext.Tasks.Update(taskItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem taskItem)
        {
            _dbContext.Tasks.Remove(taskItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}