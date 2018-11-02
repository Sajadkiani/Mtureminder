namespace MtuReminder.Model
{
    public class TaskModel
    {
        public string TaskId { get; set; }
        public string TimeDone { get; set; }
        public string Content { get; set; }
        public string  CreatedOn { get; set; }
        public string Leftposition { get; set; }
        public string  TopPosition { get; set; }
        public string IsComplete { get; set; }
    }
}
