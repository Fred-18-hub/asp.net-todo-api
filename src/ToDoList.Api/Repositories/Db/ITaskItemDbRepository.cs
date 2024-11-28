using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;

namespace ToDoList.Api.Repositories.Db
{
	public interface ITaskItemDbRepository
	{
		Task<List<TaskItem>> GetAllTasks();
		Task<TaskItem?> GetTaskById(int id);
		Task<List<TaskItem>> GetTaskByDate(DateTime date);
		Task AddTask(TaskItem task);
		Task<bool> UpdateTask(int id, TaskItemDto updateTaskDto);
		Task MarkTaskAsCompleted(TaskItem task);
		Task UnmarkTaskAsCompleted(TaskItem task);
		Task DeleteTask(TaskItem task);
		Task DeleteAllTasks();
	}
}
