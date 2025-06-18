namespace FinalProject.Api.Models
{
    public class LessonModel
    {
        //public int? FolderId { get; set; }
        //public bool IsPurchased {  get; set; }
        public int? CategoryId { get; set; }
        public int? CourseId { get; set; }
        public int? TeacherId { get; set; }
        public int? LessonId { get; set; }
        public string? Title { get; set; }
        public string? TeacherName { get; set; }
        public string? description { get; set; }
    }
}
