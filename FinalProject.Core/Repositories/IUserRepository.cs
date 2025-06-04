using FinalProject.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Core.Repositories
{
    public interface IUserRepository
    {
        User GetById(int id);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(int id);
        IEnumerable<Folder> GetFolderByTeacherId(int teacherId);
        IEnumerable<User> GetUsersFolderByFolderId(int folderId);
        IEnumerable<Folder> GetPurchasedCoursesByUserId(int userId);
    }
}
