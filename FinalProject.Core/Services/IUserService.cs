using FinalProject.Core.Entities;
using FinalProject.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Core.Services
{
    public interface IUserService
    {
        List<User> GetAll();
        User GetById(int id);
        User PostUser(User value);
        User PutUser(string d, User value);
        User DeleteUser(int id);
        IEnumerable<Folder> GetFoldersByTeacherId(int teacherId);
        IEnumerable<User> GetUsersFoldersByFolderId(int folderId);
        IEnumerable<Folder> GetPurchasedCoursesByUserId(int userId);
        Task<bool> PurchaseCourseAsync(int userId, int folderId);
    }
}
