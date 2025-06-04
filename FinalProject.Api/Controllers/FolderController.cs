using AutoMapper;
using FinalProject.Api.Models;
using FinalProject.Core.DTOs;
using FinalProject.Core.Entities;
using FinalProject.Core.Services;
using Google.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;

        public FolderController(IFolderService folderService, IMapper mapper, DataContext dataContext)
        {
            _folderService = folderService;
            _mapper = mapper;
            _dataContext = dataContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<FolderDTO>> Get()
        {
            var folders = _folderService.GetAllFolders();
            var folderDTO = _mapper.Map<IEnumerable<FolderDTO>>(folders);
            return Ok(new
            {
                success = true,
                data = folderDTO,
                message = "תיקיות נטענו בהצלחה"
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var folder = await _dataContext.Folders.FirstOrDefaultAsync(f => f.FolderId == id);
            if (folder == null)
                return NotFound();

            var folderDTO = _mapper.Map<FolderDTO>(folder);
            return Ok(folderDTO);
        }

        [HttpPost("add-folder")]
        public async Task<IActionResult> Post([FromBody] FolderModel value)
        {
            if (value == null)
                return BadRequest("Folder data is required.");

            var folder = new Folder
            {
                CategoryId = value.CategoryId,
                CourseId = value.CourseId,
                TeacherId = value.TeacherId,
                TeacherName = value.TeacherName,
                Title = value.Title,
                description = value.description,
                numberOfLessons = value.numberOfLessons
            };

            _dataContext.Folders.Add(folder);
            await _dataContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = folder.FolderId }, folder);
        }

        [HttpPost("add-lesson")]
        public async Task<IActionResult> Post([FromBody] LessonModel value)
        {
            if (value == null)
                return BadRequest("Lesson data is required.");

            var lesson = new Folder
            {
                CategoryId = value.CategoryId,
                CourseId = value.CourseId,
                LessonId = value.LessonId,
                TeacherId = value.TeacherId,
                TeacherName = value.TeacherName,
                Title = value.Title,
                description = value.description
            };

            _dataContext.Folders.Add(lesson);
            await _dataContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = lesson.FolderId }, lesson);
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory([FromBody] FolderModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                return BadRequest("Category name is required.");

            var exists = await _dataContext.Folders.AnyAsync(c => c.Title == model.Title);
            if (exists)
                return Conflict("A category with this name already exists.");

            var category = new Folder { Title = model.Title };
            _dataContext.Folders.Add(category);
            await _dataContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Category added successfully",
                categoryId = category.CategoryId,
                categoryName = category.Title
            });
        }

        [HttpPost("{userId}/purchase/{folderId}")]
        public async Task<IActionResult> PurchaseFolder(Guid userId, Guid folderId)
        {
            var folder = await _context.Folders.FindAsync(folderId);
            var user = await _context.Users.FindAsync(userId);

            if (folder == null || user == null)
                return NotFound("Folder or user not found.");

            var alreadyPurchased = await _context.FolderUsers
                .AnyAsync(fu => fu.UserId == userId && fu.FolderId == folderId);

            if (alreadyPurchased)
                return Conflict("Folder already purchased.");

            _context.FolderUsers.Add(new FolderUser
            {
                UserId = userId,
                FolderId = folderId,
                PurchaseDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return Ok("Purchase successful.");
        }

    }
}
