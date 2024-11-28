using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;
using ToDoList.Api.Repositories.Db;

namespace ToDoList.Api.Controllers
{
	[ApiVersion("1.0", Deprecated = true)]      // Comment if convention is used in Program.cs
	[ApiVersion("2.0")]                         // Comment if convention is used in Program.cs
	[Route("api/[controller]")]                 // [Route("api/v{version:apiversion}/[controller]")] - Used for URL based versioning
	[ApiController]
	public class TasksController : ControllerBase
	{
		private readonly ITaskItemDbRepository _taskItemDbRepository;

		public TasksController(ITaskItemDbRepository taskItemDbRepository)
		{
			_taskItemDbRepository = taskItemDbRepository;
		}

		/// <summary>
		/// Gets all tasks on to-do list
		/// </summary>
		/// <response code="200">Successfully get all the tasks on the to-do list</response>
		/// <returns></returns>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TaskItem>))]
		[MapToApiVersion("2.0")]
		public async Task<IActionResult> GetAllTasks()
		{
			var allItems = await _taskItemDbRepository.GetAllTasks();

			if (allItems.Count == 0)
			{
				return Ok("To-do list is empty!");
			}

			return Ok(allItems);
		}

		/// <summary>
		/// Gets a task from the to-do list by its id
		/// </summary>
		/// <param name="id">id of the task to get</param>
		/// <response code="200">Successfully gets the task</response>
		/// <response code="404">Could not find the task</response>
		/// <returns></returns>
		[HttpGet("GetById/{id}", Name = "GetById")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskItem))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		//[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		public async Task<IActionResult> GetById(int? id)
		{
			if (id == null)
			{
				return BadRequest("Task id is required");
			}

			var item = await _taskItemDbRepository.GetTaskById((int)id);

			return item != null ? Ok(item) : NotFound($"Task item with id {id} not found!");
		}

		/// <summary>
		/// Gets a list of tasks from the to-do list by the completion date
		/// </summary>
		/// <param name="date">completion date of the tasks to get</param>
		/// <response code="200">Successfully gets the tasks</response>
		/// <response code="404">Could not find any task with the specified completion date</response>
		/// <returns></returns>
		[HttpGet("GetByDate")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TaskItem>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		//[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		public async Task<IActionResult> GetByDate([FromQuery] DateTime? date)
		{
			if (date is null)
			{
				return BadRequest("date is required to proceed");
			}

			var itemsOnSpecifiedDate = await _taskItemDbRepository.GetTaskByDate((DateTime)date);

			if (itemsOnSpecifiedDate.Count == 0)
			{
				return NotFound($"No task item(s) exist for the specified date {date}");
			}

			return Ok(itemsOnSpecifiedDate);
		}

		/// <summary>
		/// Adds a new task to the to-do list
		/// </summary>
		/// <param name="createTaskDto">**Title and completion date of the task to be added**</param>
		/// <response code="201">Successfully adds the task</response>
		/// <response code="400">When the title is empty or null</response>
		/// <returns></returns>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TaskItem))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		public async Task<IActionResult> AddTask([FromBody] TaskItemDto createTaskDto)
		{
			if (string.IsNullOrEmpty(createTaskDto.Title))
			{
				return BadRequest("Task title cannot be null or empty");
			}

			var task = new TaskItem
			{
				Title = createTaskDto.Title,
				IntendedDateToComplete = (createTaskDto.IntendedDateToComplete != null ? createTaskDto.IntendedDateToComplete : DateTime.UtcNow)
			};

			await _taskItemDbRepository.AddTask(task);

			return CreatedAtRoute("GetById", new { id = task.Id }, task);
		}

		/// <summary>
		/// Updates an existing task; its title or completion date or both
		/// </summary>
		/// <param name="id">id of the task to be updated</param>
		/// <param name="updateTaskDto">**New title and completion date for the task to be updated**</param>
		/// <response code="204">Successfully updates the task</response>
		/// <response code="404">Could not find the task</response>
		/// <returns></returns>
		[HttpPatch("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		//[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		public async Task<IActionResult> UpdateTask(int? id, [FromBody] TaskItemDto updateTaskDto)
		{
			if (id == null)
			{
				return BadRequest("Task id is required");
			}

			var updated = await _taskItemDbRepository.UpdateTask((int)id, updateTaskDto);

			if (updated)
			{
				return NoContent();
			}

			return NotFound($"Task item with id {id} not found!");
		}

		/// <summary>
		/// Marks a task as completed by its id
		/// </summary>
		/// <param name="id">id of the task to be marked as completed</param>
		/// <response code="200">Successfully marks the task as completed</response>
		/// <response code="404">Could not find the task</response>
		/// <returns></returns>
		[HttpPatch("MarkAsCompleted/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskItem))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		//[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		public async Task<IActionResult> MarkAsCompleted(int? id)
		{
			if (id == null)
			{
				return BadRequest("Task id is required");
			}

			var itemToMarkCompleted = await _taskItemDbRepository.GetTaskById((int)id);

			if (itemToMarkCompleted == null)
			{
				return NotFound($"Task item with id {id} not found!");
			}

			await _taskItemDbRepository.MarkTaskAsCompleted(itemToMarkCompleted);

			return Ok(itemToMarkCompleted);
		}

		/// <summary>
		/// Unmarks a task as completed by its id
		/// </summary>
		/// <param name="id">id of the task to be unmarked as completed</param>
		/// <response code="200">Successfully unmarks the task as completed</response>
		/// <response code="404">Could not find the task</response>
		/// <returns></returns>
		[HttpPatch("UnmarkAsCompleted/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskItem))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		//[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		public async Task<IActionResult> UnmarkAsCompleted(int? id)
		{
			if (id == null)
			{
				return BadRequest("Task id is required");
			}

			var itemToUnmarkCompleted = await _taskItemDbRepository.GetTaskById((int)id);

			if (itemToUnmarkCompleted == null)
			{
				return NotFound($"Task item with id {id} not found!");
			}

			await _taskItemDbRepository.UnmarkTaskAsCompleted(itemToUnmarkCompleted);

			return Ok(itemToUnmarkCompleted);
		}

		/// <summary>
		/// Deletes a task by its id
		/// </summary>
		/// <param name="id">id of the task to be deleted</param>
		/// <response code="204">Successfully deletes the task</response>
		/// <response code="404">Could not find the task</response>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
		//[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
		public async Task<IActionResult> DeleteTask(int? id)
		{
			if (id is null)
			{
				return BadRequest("Task id is required");
			}

			var itemToDelete = await _taskItemDbRepository.GetTaskById((int)id);

			if (itemToDelete == null)
			{
				return NotFound($"Task item with id {id} not found!");
			}

			await _taskItemDbRepository.DeleteTask(itemToDelete);

			return NoContent();
		}

		/// <summary>
		/// Deletes all the tasks on the to-do list
		/// </summary>
		/// <response code="204">Successfully delete all the tasks on the to-do list</response>
		/// <returns></returns>
		[HttpDelete("DeleteAllTasks")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> DeleteAllTasks()
		{
			await _taskItemDbRepository.DeleteAllTasks();

			return NoContent();
		}

	}
}
