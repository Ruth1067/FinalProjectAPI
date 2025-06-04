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
        private readonly DataContext _context;

        public UserService(IUserRepository userRepository, DataContext context)
        {
            _userRepository = userRepository;
            _context = context; 
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