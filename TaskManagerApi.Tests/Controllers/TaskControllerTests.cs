using Moq;
using TaskManagerApi.Interfaces;
using TaskManagerApi.DTOs;     // Idem pour les DTOs
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Controllers;

namespace TaskManagerApi.Tests
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _mockService;
        private readonly TasksController _controller;

        public TaskControllerTests()
        {
            _mockService = new Mock<ITaskService>();
            _controller = new TasksController(_mockService.Object);
        }

        #region GetTasks

        [Fact]
        public async Task GetTasks_ReturnsOkResult_WithTasks()
        {
            // Arrange
            var tasks = new List<TaskItemDto>
        {
            new TaskItemDto { Title = "Task 1", DueDate = DateTime.Now.AddDays(1), IsDone = false },
            new TaskItemDto { Title = "Task 2", DueDate = DateTime.Now.AddDays(2), IsDone = true }
        };

            _mockService.Setup(service => service.GetTasksAsync(It.IsAny<bool?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>()))
                        .ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetTasks(null, null, null, false);

            // Assert           
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TaskItemDto>>>(result); // Vérifie que c'est un ActionResult
            var okResult = actionResult.Result as OkObjectResult; // Vérifie que c'est un OkObjectResult

            Assert.NotNull(okResult); // Vérifie que le OkObjectResult est non nul
            var returnValue = Assert.IsType<List<TaskItemDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetTasks_ReturnsStatusCode500_WhenExceptionThrown()
        {
            // Arrange
            _mockService.Setup(service => service.GetTasksAsync(It.IsAny<bool?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool>()))
                        .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetTasks(null, null, null, false);

            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<TaskItemDto>>>(result); // Vérifie que le résultat est un ActionResult
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result); // Vérifie qu'il s'agit d'un ObjectResult à l'intérieur de l'ActionResult

            // Vérifie le code de statut et le message d'erreur
            Assert.Equal(500, objectResult.StatusCode); // Vérifie que le code de statut est 500
            Assert.Equal("Erreur serveur : Test exception", objectResult.Value); // Vérifie le message d'erreur dans le corps de la réponse
        }

        #endregion

        #region GetTask(int id)

        [Fact]
        public async Task GetTasks_Id_ReturnOkResult_WithTasks()
        {
            // Arrange
            int taskId = 1;
            var tasks = new TaskItemDto { Id = taskId, Title = "Task 1", DueDate = DateTime.Now.AddDays(1), IsDone = false };

            _mockService.Setup(service => service.GetTaskByIdAsync(taskId))
                        .ReturnsAsync(tasks);
            //Act
            var result = await _controller.GetTask(taskId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult);
            var returnValue = Assert.IsType<TaskItemDto>(okResult.Value);
            Assert.Equal(taskId, returnValue.Id);
            Assert.Equal("Task 1", returnValue.Title);
        }

        [Fact]
        public async Task GetTaks_Id_Notfound()
        {
            // Arrange
            int taskId = 1;

            _mockService.Setup(service => service.GetTaskByIdAsync(taskId))
               .ReturnsAsync((TaskItemDto)null);
            // Act
            var result = await _controller.GetTask(taskId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);

        }

        #endregion

        #region CreateTask

        [Fact]
        public async Task CreateTask_ReturnsCreatedAtAction_WithCreatedTask()
        {
            // Arrange
            var createDto = new TaskItemCreateDto
            {
                Title = "Nouvelle tâche",
                DueDate = DateTime.Now.AddDays(2)
            };

            var createdTask = new TaskItemDto
            {
                Id = 1,
                Title = createDto.Title,
                DueDate = (DateTime)createDto.DueDate,
                IsDone = false
            };

            _mockService.Setup(service => service.CreateTaskAsync(createDto))
                        .ReturnsAsync(createdTask);

            // Act
            var result = await _controller.CreateTask(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<TaskItemDto>(createdAtActionResult.Value);

            Assert.Equal(createdTask.Id, returnValue.Id);
            Assert.Equal(createdTask.Title, returnValue.Title);
            Assert.Equal(createdTask.DueDate, returnValue.DueDate);
        }

        [Fact]
        public async Task CreateTask_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var createDto = new TaskItemCreateDto
            {
                Title = "Tâche invalide",
                DueDate = DateTime.Now.AddDays(2)
            };

            _mockService.Setup(service => service.CreateTaskAsync(createDto))
                        .ReturnsAsync((TaskItemDto?)null);

            // Act
            var result = await _controller.CreateTask(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Erreur lors de la création de la tâche", badRequestResult.Value);
        }

        #endregion

        #region UpdateTask

        [Fact]
        public async Task UpdateTask_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            int taskId = 1;
            var updateDto = new TaskItemUpdateDto
            {
                Title = "Tâche mise à jour",
                DueDate = DateTime.Now.AddDays(3),
                IsDone = true
            };

            var updatedTask = new TaskItemDto
            {
                Id = taskId,
                Title = updateDto.Title,
                DueDate = (DateTime)updateDto.DueDate,
                IsDone = updateDto.IsDone
            };

            _mockService.Setup(service => service.UpdateTaskAsync(taskId, updateDto))
                        .ReturnsAsync(updatedTask);

            // Act
            var result = await _controller.UpdateTask(taskId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            int taskId = 42; // id inexistant
            var updateDto = new TaskItemUpdateDto
            {
                Title = "Titre inexistant",
                DueDate = DateTime.Now.AddDays(2),
                IsDone = false
            };

            _mockService.Setup(service => service.UpdateTaskAsync(taskId, updateDto))
                        .ReturnsAsync((TaskItemDto?)null);

            // Act
            var result = await _controller.UpdateTask(taskId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


        #endregion

        #region DeleteTask

        [Fact]
        public async Task DeleteTask_ReturnsNoContent_WhenTaskIsDeletedSuccessfully()
        {
            // Arrange
            int taskId = 1;

            _mockService.Setup(service => service.DeleteTaskAsync(taskId))
                        .ReturnsAsync(true); // Tâche supprimée avec succès

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            int taskId = 42;

            _mockService.Setup(service => service.DeleteTaskAsync(taskId))
                        .ReturnsAsync((bool?)null); // Tâche non trouvée

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        
        #endregion

    }
}