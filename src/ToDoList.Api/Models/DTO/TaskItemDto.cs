using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Api.Models.DTO
{
	public class TaskItemDto
	{
		public string? Title { get; set; }

		[Column(TypeName = "date")]
		public DateTime? IntendedDateToComplete { get; set; }
	}
}
