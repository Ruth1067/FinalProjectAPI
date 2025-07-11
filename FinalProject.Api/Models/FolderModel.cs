﻿namespace FinalProject.Api.Models
{
    public class FolderModel
    {
        public bool? IsPurchased { get; set; }
        public int? CategoryId { get; set; }
        public int? CourseId { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string Title { get; set; }
        public string? description { get; set; }
        public int? numberOfLessons { get; set; }
        public int? price { get; set; }
    }
}
