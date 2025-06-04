﻿//using FinalProject.Core.Entities;
//using FinalProject.Core.Repositories;
//using FinalProject.Core.Services;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

////namespace FinalProject.Service
////{
////    public class UserService
////    {
////        using System.Collections.Generic;

//namespace FinalProject.Core.Services
//{
//    public class UserService : IUserService
//    {
//        private readonly IUserRepository _userRepository;

//        public UserService(IUserRepository userRepository)
//        {
//            _userRepository = userRepository;
//        }

//        public User GetById(int id)
//        {
//            return _userRepository.GetById(id);
//        }

//        public IEnumerable<User> GetAll()
//        {
//            return _userRepository.GetAll();
//        }

//        public void Add(User user)
//        {
//            _userRepository.Add(user);
//        }

//        public void Update(User user)
//        {
//            _userRepository.Update(user);
//        }

//        public void Delete(int id)
//        {
//            _userRepository.Delete(id);
//        }
//    }
//}

//using AutoMapper;
//using FinalProject.Core.DTOs;
//using FinalProject.Core.Entities;
//using FinalProject.Core.Repositories;
//using FinalProject.Core.Services;
//using System;
//using System.Collections.Generic;

////using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Collections.Generic;

////using LinqToDB;


//namespace FinalProject
//{
//    public class UserService : IUserService
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly DataContext _context;


//        //public UserService(IUserRepository userRepository)
//        //{
//        //    _userRepository = userRepository;
//        //}
//        public UserService(IUserRepository userRepository, DataContext context)
//        {
//            _userRepository = userRepository;
//            _context = context;
//        }


//        public List<User> GetAll()
//        {
//            return _userRepository.GetAll().ToList(); // שינוי מ-GetList ל-GetAll
//        }

//        public User GetById(int id)
//        {
//            return _userRepository.GetById(id);
//        }

//        public User PostUser(User value)
//        {
//            _userRepository.Add(value);
//            return value; // מחזיר את המשתמש שנוסף
//        }

//        public User PutUser(string d, User value)
//        {
//            _userRepository.Update(value);
//            return value; // מחזיר את המשתמש המעודכן
//        }

//        public User DeleteUser(int id)
//        {
//            var user = _userRepository.GetById(id);
//            if (user != null)
//            {
//                _userRepository.Delete(id);
//                return user; // מחזיר את המשתמש שנמחק
//            }
//            return null; // מחזיר null אם המשתמש לא נמצא
//        }
//        public IEnumerable<Folder> GetFoldersByTeacherId(int teacherId)
//        {
//            return _userRepository.GetFolderByTeacherId(teacherId); // הנחה שיש לנו Repository עבור קורסים
//        }
//        public IEnumerable<User> GetUsersFoldersByFolderId(int folderId)
//        {
//            return _userRepository.GetUsersFolderByFolderId(folderId); // הנחה שיש לנו Repository עבור קורסים
//        }
//        public IEnumerable<Folder> GetPurchasedCoursesByUserId(int userId)
//        {
//            return _context.Folders
//                .Include(f => f.Users)
//                .Where(f => f.CourseId != null && f.Users.Any(u => u.UserId == userId))
//                .ToList();
//        }

//    }

using AutoMapper;
using FinalProject.Core.DTOs;
using FinalProject.Core.Entities;
using FinalProject.Core.Repositories;
using FinalProject.Core.Services;
using FinalProject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FinalProject
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly DataContext _context; // Added missing _context field  

        public UserService(IUserRepository userRepository, DataContext context)
        {
            _userRepository = userRepository;
            _context = context; // Initialize _context  
        }

        public List<User> GetAll()
        {
            return _userRepository.GetAll().ToList();
        }

        public User GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public User PostUser(User value)
        {
            _userRepository.Add(value);
            return value;
        }

        public User PutUser(string d, User value)
        {
            _userRepository.Update(value);
            return value;
        }

        public User DeleteUser(int id)
        {
            var user = _userRepository.GetById(id);
            if (user != null)
            {
                _userRepository.Delete(id);
                return user;
            }
            return null;
        }

        public IEnumerable<Folder> GetFoldersByTeacherId(int teacherId)
        {
            return _userRepository.GetFolderByTeacherId(teacherId);
        }

        public IEnumerable<User> GetUsersFoldersByFolderId(int folderId)
        {
            return _userRepository.GetUsersFolderByFolderId(folderId);
        }

        public IEnumerable<Folder> GetPurchasedCoursesByUserId(int userId)
        {

            return _context.Folders
                .Where(f =>
                    f.CourseId != null &&
                    f.LessonId == null &&
                    f.Users.Any(u => u.Id == userId))
                .ToList();

            //return _userRepository.GetPurchasedCoursesByUserId(userId);

        }
        public async Task<bool> PurchaseCourseAsync(int userId, int folderId)
        {
            var exists = await _context.FolderUsers
                .AnyAsync(fu => fu.UserId == userId && fu.FolderId == folderId);

            if (exists)
                return false;

            var folderUser = new FolderUser
            {
                UserId = userId,
                FolderId = folderId,
                PurchaseDate = DateTime.UtcNow
            };

            _context.FolderUsers.Add(folderUser);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}

    //public class UserService : IUserService
    //{
    //    //private readonly IUserRepository _userRepository;
    //    //private readonly IMapper _mapper;
    //    //private readonly DataContext _context;

    //    //public UserService(IUserRepository userRepository, IMapper mapper, DataContext context)
    //    //{
    //    //    _userRepository = userRepository;
    //    //    _mapper = mapper;
    //    //    _context = context;
    //    //}

    //    //public async Task<List<User>> GetAllAsync()
    //    //{
    //    //    return await Task.FromResult(_userRepository.GetList().ToList());
    //    //}

    //    //public async Task<User> GetByIdAsync(int id)
    //    //{
    //    //    return await Task.FromResult(_userRepository.GetUser(id));
    //    //}

    //    //public async Task<User> PostUserAsync(User value) // Changed return type to Task<User>
    //    //{
    //    //    _userRepository.PostUser(value); // Correct method name
    //    //    await _context.SaveChangesAsync(); // Ensure you save the changes
    //    //    return value; // Return User instead of UserDTO
    //    //}

    //    //public async Task PutUserAsync(string id, User value) // Changed return type to Task
    //    //{
    //    //    _userRepository.PutUser(id, value);
    //    //    await Task.CompletedTask; // Ensure the method is async
    //    //}

    //    //public async Task<bool> DeleteUserAsync(int id)
    //    //{
    //    //    _userRepository.DeleteUser(id);
    //    //    return await Task.FromResult(true);
    //    //}

    //    private readonly IUserRepository _userRepository;

    //    public UserService(IUserRepository userRepository)
    //    {
    //        _userRepository = userRepository;
    //    }

    //    public List<User> GetAll()
    //    {
    //        return _userRepository.GetAll().ToList(); // שינוי מ-GetList ל-GetAll
    //    }

    //    public User GetById(int id)
    //    {
    //        return _userRepository.GetById(id);
    //    }

    //    public User PostUser(User value)
    //    {
    //        _userRepository.Add(value);
    //        return value; // מחזיר את המשתמש שנוסף
    //    }

    //    public User PutUser(string d, User value)
    //    {
    //        _userRepository.Update(value);
    //        return value; // מחזיר את המשתמש המעודכן
    //    }

    //    public User DeleteUser(int id)
    //    {
    //        _userRepository.Delete(id);
    //        return new User();
    //    }
    //}
//}
