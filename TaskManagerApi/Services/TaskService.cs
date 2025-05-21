using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TaskManagerApi.DTOs;
using TaskManagerApi.Interfaces;
using TaskManagerApi.Models;

namespace TaskManagerApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IMapper _mapper;

        public TaskService(ITaskItemRepository taskItemRepository, IMapper mapper)
        {
            _taskItemRepository = taskItemRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskItemDto>> GetTasksAsync(
            bool? isDone,
            string? search,
            string? sortBy,
            bool desc = false)
        {
            // Récupère d'abord la liste des tâches depuis le repository
            var tasks = await _taskItemRepository.GetAllAsync();

            // Transforme ensuite la liste en IQueryable pour pouvoir appliquer les filtres et tris
            var query = tasks.AsQueryable();

            if (isDone.HasValue)
            {
                query = query.Where(task => task.IsDone == isDone);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(task => task.Title.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "title" => desc ? query.OrderByDescending(task => task.Title) : query.OrderBy(task => task.Title),
                    "duedate" => desc ? query.OrderByDescending(task => task.DueDate) : query.OrderBy(task => task.DueDate),
                    _ => query
                };
            }

            // Retourner les DTOs après mapping
            return _mapper.Map<IEnumerable<TaskItemDto>>(query);
        }


        public async Task<TaskItemDto?> GetTaskByIdAsync(int id)
        {
            var task = await _taskItemRepository.GetByIdAsync(id);

            if (task == null)
            {
                return null;
            }

            return _mapper.Map<TaskItemDto>(task);
        }

        public async Task<TaskItemDto?> CreateTaskAsync(TaskItemCreateDto dto)
        {
            var task = _mapper.Map<TaskItem>(dto); // Mapper le DTO en entité TaskItem

            // Appeler le repository pour ajouter la tâche à la base de données
            await _taskItemRepository.AddAsync(task);
            await _taskItemRepository.SaveAsync();  

            // Mapper l'entité TaskItem en TaskItemDto et retourner le DTO
            return _mapper.Map<TaskItemDto>(task);
        }

        public async Task<TaskItemDto?> UpdateTaskAsync(int id, TaskItemUpdateDto dto)
        {
            // Récupérer la tâche de la base de données
            var task = await _taskItemRepository.GetByIdAsync(id);

            if (task == null)
            {
                return null; 
            }

            // Mapper les données du DTO vers l'entité TaskItem
            _mapper.Map(dto, task);

            // Mettre à jour la tâche dans la base de données
            await _taskItemRepository.SaveAsync();

            // Retourner le DTO mis à jour
            return _mapper.Map<TaskItemDto>(task);
        }


        public async Task<bool?> DeleteTaskAsync(int id)
        {
            // Récupérer la tâche de la base de données
            var task = await _taskItemRepository.GetByIdAsync(id);

            if (task == null)
            {
                return null;  
            }

            // Vérifier si la tâche est terminée
            if (task.IsDone)
            {
                return false; 
            }

            // Supprimer la tâche via le repository
            await _taskItemRepository.DeleteAsync(task);
            await _taskItemRepository.SaveAsync();  // Sauvegarder les changements dans la base de données

            return true;  // Retourner true si la suppression a réussi
        }

        // /// <summary>
        // /// Savoir si une tâche est en retard
        // /// </summary>
        // /// <param name="task"></param>
        // /// <returns></returns>
        // public bool IsLate(TaskItem task)
        // {
        //     return !task.IsDone && task.DueDate < DateTime.Now;
        // }

        
    }
}