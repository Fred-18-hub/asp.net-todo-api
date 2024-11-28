using ToDoList.Api.Models;
using ToDoList.Api.Models.DTO;

namespace ToDoList.Api.Repositories.Db
{
	public class TaskItemDbRepository : ITaskItemDbRepository
	{
		private readonly DataContext _context;

		public TaskItemDbRepository(DataContext context)
		{
			_context = context;
		}

		public async Task<List<TaskItem>> GetAllTasks()
		{
			return await _context.TaskItems.ToListAsync();
		}

		public async Task<TaskItem?> GetTaskById(int id)
		{
			return await _context.TaskItems.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<TaskItem>> GetTaskByDate(DateTime date)
		{
			return await _context.TaskItems.Where(d => d.IntendedDateToComplete ==  date).ToListAsync();
		}

		public async Task AddTask(TaskItem task)
		{
			_context.TaskItems.Add(task);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> UpdateTask(int id, TaskItemDto updateTaskDto)
		{
			var itemToUpdate = await GetTaskById(id);

			if (itemToUpdate == null)
			{
				return false;
			}

			else
			{
				if (!string.IsNullOrEmpty(updateTaskDto.Title))
				{
					itemToUpdate.Title = updateTaskDto.Title;
				}

				if (updateTaskDto.IntendedDateToComplete != null)
				{
					itemToUpdate.IntendedDateToComplete = updateTaskDto.IntendedDateToComplete;
				}

				await _context.SaveChangesAsync();
				return true;
			}
		}

		public async Task MarkTaskAsCompleted(TaskItem task)
		{
			task.IsCompleted = true;

			if (task.DateCompleted == null)
			{
				task.DateCompleted = DateTime.UtcNow;
			}
			
			_context.TaskItems.Update(task);
			await _context.SaveChangesAsync();
		}

		public async Task UnmarkTaskAsCompleted(TaskItem task)
		{
			task.IsCompleted = false;
			task.DateCompleted = null;

			_context.TaskItems.Update(task);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteTask(TaskItem task)
		{
			_context.TaskItems.Remove(task);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAllTasks()
		{
			var allItems = await _context.TaskItems.ToListAsync();

			_context.TaskItems.RemoveRange(allItems);
			await _context.SaveChangesAsync();
		}
	}
}
