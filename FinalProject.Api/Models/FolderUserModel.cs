using FinalProject.Core.Entities;

namespace FinalProject.Api.Models
{
    public class FolderUserModel
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int FolderId { get; set; }
        public Folder Folder { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    }
}
