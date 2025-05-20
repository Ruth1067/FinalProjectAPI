using AutoMapper;
using FinalProject;
using FinalProject.Api.Models;
using FinalProject.Core.DTOs;
using FinalProject.Core.Entities;
using FinalProject.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


[Route("api/[controller]")]
[ApiController]
//[Authorize]
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

    // GET: api/<TurnController>
    [HttpGet]
    //[Authorize]
    public ActionResult<IEnumerable<Folder>> Get()
    {
        var folder = _folderService.GetAllFolders();
        var folderDTO = _mapper.Map<IEnumerable<FolderDTO>>(folder);
        return Ok(folderDTO);
    }

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
            TeacherId=value.TeacherId,
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
    //public async Task<IActionResult> AddFolder([FromBody] FolderModel folder)
    //{
    //    //public ActionResult<Folder> AddFolder([FromBody] FolderModel folder)
    //    {
    //        if (folder == null)
    //        {
    //            return BadRequest("Folder data is required.");
    //        }

    //        // Map FolderModel to Folder before passing it to the service
    //        var folderEntity = _mapper.Map<Folder>(folder);

    //        var createdFolder = _folderService.PostFolder(folderEntity);
    //        return Ok(CreatedAtAction(nameof(Get), new { id = createdFolder.CourseId }, createdFolder));
    //    }

    //}
}

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//[ApiController]
//[Route("api/[controller]")]
//public class FilesController : ControllerBase
//{
//    [HttpGet("admin-only")]
//    [Authorize(Policy = "AdminOnly")] // רק Admin יכול לגשת
//    public IActionResult AdminOnly()
//    {
//        return Ok("This is accessible only by Admins.");
//    }

//    [HttpGet("editor-or-admin")]
//    [Authorize(Policy = "EditorOrAdmin")] // Editor או Admin יכולים לגשת
//    public IActionResult EditorOrAdmin()
//    {
//        return Ok("This is accessible by Editors and Admins.");
//    }

//    [HttpGet("viewer-only")]
//    [Authorize(Policy = "ViewerOnly")] // רק Viewer יכול לגשת
//    public IActionResult ViewerOnly()
//    {
//        return Ok("This is accessible only by Viewers.");
//    }

//    [HttpGet("authenticated-only")]
//    [Authorize] // כל משתמש מאומת יכול לגשת
//    public IActionResult AuthenticatedOnly()
//    {
//        return Ok("This is accessible by any authenticated user.");
//    }
//}