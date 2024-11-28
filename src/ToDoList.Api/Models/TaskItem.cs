using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Api.Models
{
	public class TaskItem
	{
        public int Id { get; set; }
        public string Title { get; set; } 

        [Column(TypeName = "date")]
        public DateTime? IntendedDateToComplete { get; set; } = DateTime.Today.ToUniversalTime();
        public bool IsCompleted { get; set; } = false;
		public DateTime? DateCompleted { get; set; } = null;
    }
}
