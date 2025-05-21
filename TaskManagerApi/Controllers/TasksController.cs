using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.DTOs;
using TaskManagerApi.Interfaces;
using TaskManagerApi.Models;
using TaskManagerApi.Services;

namespace TaskManagerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService service)
        {
            _taskService = service;
        }

        //GET: tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasks(
                [FromQuery] bool? isDone,      // filtre facultatif sur état terminé
                [FromQuery] string? search,    // recherche dans le titre
                [FromQuery] string? sortBy,    // champ à trier : "duedate" ou "title"
                [FromQuery] bool desc = false  // tri descendant si true
        )
        {
            try
            {
                var tasks = await _taskService.GetTasksAsync(isDone, search, sortBy, desc);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }

        }

        //GET: tasks/22
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetTask(int id)
        {
            var taskDto = await _taskService.GetTaskByIdAsync(id);

            if (taskDto == null)
            {
                return NotFound();
            }

            return Ok(taskDto);
        }

        //POST: tasks
        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTask(TaskItemCreateDto dto)
        {
            var taskDto = await _taskService.CreateTaskAsync(dto);

            // Si la création échoue ou retourne null, retourner un problème
            if (taskDto == null)
            {
                return BadRequest("Erreur lors de la création de la tâche");
            }

            return CreatedAtAction(nameof(GetTask), new { id = taskDto.Id }, taskDto);
        }

        //PUT : tasks/22
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItemUpdateDto dto)
        {
            var taskDto = await _taskService.UpdateTaskAsync(id, dto);

            // Si la mise à jour échoue, retourner NotFound
            if (taskDto == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        //DELETE : tasks/22
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);

            // Si la tâche n'existe pas ou ne peut pas être supprimée
            if (result == null)
            {
                return NotFound();  // La tâche n'a pas été trouvée
            }

            if (!result.Value)
            {
                return BadRequest("Impossible de supprimer une tâche terminée.");
            }

            return NoContent();
        }

    }
}