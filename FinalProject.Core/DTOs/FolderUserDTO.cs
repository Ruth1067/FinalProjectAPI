using FinalProject.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Core.DTOs
{
    class FolderUserDTO
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int FolderId { get; set; }
        public Folder Folder { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    }
}
