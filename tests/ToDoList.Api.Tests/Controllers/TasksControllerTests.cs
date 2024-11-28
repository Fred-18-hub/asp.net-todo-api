using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ToDoList.Api.Controllers;
using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;
using ToDoList.Api.Repositories.Db;

namespace ToDoList.Api.Tests.Controllers
{
	public class TasksControllerTests
	{
		private readonly IFixture _fixture;
        private readonly ITaskItemDbRepository _taskItemDbRepository;
        
        private readonly TasksController _sut;

        public TasksControllerTests()
        {
            _fixture = new Fixture();
            _taskItemDbRepository = Substitute.For<ITaskItemDbRepository>();

            _sut = new TasksController(_taskItemDbRepository);
        }

        [Fact]
        public async Task GetAllTasks_ShouldReturnOkResponse_WhenToDoListIsEmpty()
        {
            // Arrange
            List<TaskItem> allItems = [];
            _taskItemDbRepository.GetAllTasks().Returns(allItems);

            // Act
            var result = await _sut.GetAllTasks();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeOfType<string>();
        }

		[Fact]
		public async Task GetAllTasks_ShouldReturnOkResponse_WhenToDoListIsNotEmpty()
		{
			// Arrange
            var all = _fixture.Create<List<TaskItem>>();
			_taskItemDbRepository.GetAllTasks().Returns(all);

			// Act
			var result = await _sut.GetAllTasks();

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.As<OkObjectResult>().Value.Should().BeOfType(all.GetType());
		}

		[Fact]
		public async Task GetById_ShouldReturnBadRequest_WhenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.GetById(id);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.As<BadRequestObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task GetById_ShouldReturnNotFound_WhenTaskNotFound()
		{
			// Arrange
			var id = _fixture.Create<int>();
			TaskItem? item = null;
			_taskItemDbRepository.GetTaskById(id).Returns(item);

			// Act
			var result = await _sut.GetById(id);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
			result.As<NotFoundObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task GetById_ShouldReturnOkResponse_WhenTaskIsFound()
		{
			// Arrange
			var id = _fixture.Create<int>();
			var item = _fixture.Create<TaskItem>();
			_taskItemDbRepository.GetTaskById(id).Returns(item);

			// Act
			var result = await _sut.GetById(id);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.As<OkObjectResult>().Value.Should().BeOfType(item.GetType());
		}

		[Fact]
		public async Task GetByDate_ShouldReturnBadRequest_WhenDateIsNull()
		{
			// Arrange
			DateTime? date = null;

			// Act
			var result = await _sut.GetByDate(date);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.As<BadRequestObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task GetByDate_ShouldReturnNotFound_WhenTaskNotFound()
		{
			// Arrange
			var date = _fixture.Create<DateTime>();
			List<TaskItem> items = [];
			_taskItemDbRepository.GetTaskByDate(date).Returns(items);

			// Act
			var result = await _sut.GetByDate(date);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
			result.As<NotFoundObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task GetByDate_ShouldReturnOkResponse_WhenTasksFound()
		{
			// Arrange
			var date = _fixture.Create<DateTime>();
			var items = _fixture.Create<List<TaskItem>>();
			_taskItemDbRepository.GetTaskByDate(date).Returns(items);

			// Act
			var result = await _sut.GetByDate(date);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.As<OkObjectResult>().Value.Should().BeOfType(items.GetType());
		}

		[Fact]
		public async Task AddTask_ShouldReturnBadRequest_WhenTaskTitleIsNullOrEmpty()
		{
			// Arrange
			var task = new TaskItemDto
			{
				Title = string.Empty,
				IntendedDateToComplete = _fixture.Create<DateTime>(),
			};

			// Act
			var result = await _sut.AddTask(task);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.As<BadRequestObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task AddTask_ShouldReturnCreatedResponse_WhenTaskIsAdded()
		{
			// Arrange
			var task = _fixture.Create<TaskItemDto>();

			// Act
			var result = await _sut.AddTask(task);

			// Assert
			result.Should().BeOfType<CreatedAtRouteResult>();
			result.As<CreatedAtRouteResult>().Value.Should().BeOfType<TaskItem>();
		}

		[Fact]
		public async Task UpdateTask_ShouldReturnBadRequest_WhenIdIsNull()
		{
			// Arrange
			int? id = null;
			var taskUpdate = _fixture.Create<TaskItemDto>();

			// Act
			var result = await _sut.UpdateTask(id, taskUpdate);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.As<BadRequestObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task UpdateTask_ShouldReturnNoContent_WhenTaskIsUpdated()
		{
			// Arrange
			var id = _fixture.Create<int>();
			var taskUpdate = _fixture.Create<TaskItemDto>();
			_taskItemDbRepository.UpdateTask(Arg.Any<int>(), Arg.Any<TaskItemDto>()).Returns(true);

			// Act
			var result = await _sut.UpdateTask(id, taskUpdate);

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

		[Fact]
		public async Task UpdateTask_ShouldReturnNotFound_WhenTaskToUpdateIsNotFound()
		{
			// Arrange
			var id = _fixture.Create<int>();
			var taskUpdate = _fixture.Create<TaskItemDto>();
			_taskItemDbRepository.UpdateTask(Arg.Any<int>(), Arg.Any<TaskItemDto>()).Returns(false);

			// Act
			var result = await _sut.UpdateTask(id, taskUpdate);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
			result.As<NotFoundObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task MarkAsCompleted_ShouldReturnBadRequest_WhenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.MarkAsCompleted(id);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.As<BadRequestObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task MarkAsCompleted_ShouldReturnNotFound_WhenTaskNotFound()
		{
			// Arrange
			var id = _fixture.Create<int>();
			TaskItem? item = null;
			_taskItemDbRepository.GetTaskById(id).Returns(item);

			// Act
			var result = await _sut.MarkAsCompleted(id);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
			result.As<NotFoundObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task MarkAsCompleted_ShouldReturnOkResponse_WhenTaskIsMarkedAsCompleted()
		{
			// Arrange
			var id = _fixture.Create<int>();
			var item = _fixture.Create<TaskItem>();
			_taskItemDbRepository.GetTaskById(id).Returns(item);

			// Act
			var result = await _sut.MarkAsCompleted(id);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.As<OkObjectResult>().Value.Should().BeOfType(item.GetType());
		}

		[Fact]
		public async Task UnmarkAsCompleted_ShouldReturnBadRequest_WhenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.UnmarkAsCompleted(id);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.As<BadRequestObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task UnmarkAsCompleted_ShouldReturnNotFound_WhenTaskIsUnmarkedAsCompleted()
		{
			// Arrange
			var id = _fixture.Create<int>();
			TaskItem? item = null;
			_taskItemDbRepository.GetTaskById(id).Returns(item);

			// Act
			var result = await _sut.UnmarkAsCompleted(id);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
			result.As<NotFoundObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task UnmarkAsCompleted_ShouldReturnOkResponse_WhenTaskIsFound()
		{
			// Arrange
			var id = _fixture.Create<int>();
			var item = _fixture.Create<TaskItem>();
			_taskItemDbRepository.GetTaskById(id).Returns(item);

			// Act
			var result = await _sut.UnmarkAsCompleted(id);

			// Assert
			result.Should().BeOfType<OkObjectResult>();
			result.As<OkObjectResult>().Value.Should().BeOfType(item.GetType());
		}

		[Fact]
		public async Task DeleteTask_ShouldReturnBadRequest_WhenIdIsNull()
		{
			// Arrange
			int? id = null;

			// Act
			var result = await _sut.DeleteTask(id);

			// Assert
			result.Should().BeOfType<BadRequestObjectResult>();
			result.As<BadRequestObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task DeleteTask_ShouldReturnNotFound_WhenTaskNotFound()
		{
			// Arrange
			var id = _fixture.Create<int>();
			TaskItem? item = null;
			_taskItemDbRepository.GetTaskById(id).Returns(item);

			// Act
			var result = await _sut.DeleteTask(id);

			// Assert
			result.Should().BeOfType<NotFoundObjectResult>();
			result.As<NotFoundObjectResult>().Value.Should().BeOfType<string>();
		}

		[Fact]
		public async Task DeleteTask_ShouldReturnNoContent_WhenTaskIsDeleted()
		{
			// Arrange
			var id = _fixture.Create<int>();
			var item = _fixture.Create<TaskItem>();
			_taskItemDbRepository.GetTaskById(id).Returns(item);

			// Act
			var result = await _sut.DeleteTask(id);

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}
		
		[Fact]
		public async Task DeleteAllTasks_ShouldReturnNoContent()
		{
			// Arrange

			// Act
			var result = await _sut.DeleteAllTasks();

			// Assert
			result.Should().BeOfType<NoContentResult>();
		}

	}
}
