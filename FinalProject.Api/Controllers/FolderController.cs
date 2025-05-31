using AutoMapper;
using FinalProject;
using FinalProject.Api.Models;
using FinalProject.Core.DTOs;
using FinalProject.Core.Entities;
using FinalProject.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
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
    //[Authorize]
    public ActionResult<IEnumerable<Folder>> Get()
    {
        var folder = _folderService.GetAllFolders();
        var folderDTO = _mapper.Map<IEnumerable<FolderDTO>>(folder);
        return Ok(folderDTO);
    }

    //[Authorize(Policy = "TeacherOrAdmin")]
    [HttpPost("add-folder")]
    public async Task<IActionResult> Post([FromBody] FolderModel value)
    {
        if (value == null)
        {
            return BadRequest("Folder data is required.");
        }

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

    //[Authorize(Policy = "TeacherOrAdmin")]
    [HttpPost("add-lesson")]
    public async Task<IActionResult> Post([FromBody] LessonModel value)
    {
        if (value == null)
        {
            return BadRequest("Lesson data is required.");
        }

        var lesson = new Folder
        {
            CategoryId = value.CategoryId,
            CourseId = value.CourseId,
            LessonId = value.LessonId,
            TeacherId = value.TeacherId,
            TeacherName = value.TeacherName,
            Title = value.Title,
            description = value.description,
        };

        _dataContext.Folders.Add(lesson);
        await _dataContext.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = lesson.FolderId }, lesson);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("add-category")]
    public async Task<IActionResult> AddCategory([FromBody] FolderModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            return BadRequest("Category name is required.");
        }

        var exists = await _dataContext.Folders
            .AnyAsync(c => c.Title == model.Title);

        if (exists)
        {
            return Conflict("A category with this name already exists.");
        }

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


    [HttpGet("{id}")]
    //[Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var folder = await _dataContext.Folders
            .FirstOrDefaultAsync(f => f.FolderId == id);

        if (folder == null)
        {
            return NotFound();
        }

        var folderDTO = _mapper.Map<FolderDTO>(folder);
        return Ok(folderDTO);
    }

}