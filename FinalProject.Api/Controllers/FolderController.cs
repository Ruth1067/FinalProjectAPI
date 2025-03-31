using AutoMapper;
using FinalProject;
using FinalProject.Core.DTOs;
using FinalProject.Core.Entities;
using FinalProject.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FolderController : ControllerBase
{
    private readonly IFolderService _folderService;
    private readonly IMapper _mapper;

    public FolderController(IFolderService folderService, IMapper mapper)
    {
        _folderService = folderService;
        _mapper = mapper;
    }

    // GET: api/<TurnController>
    [HttpGet]
    public ActionResult<IEnumerable<Folder>> Get()
    {
        var folder = _folderService.GetAllFolders();
        var folderDTO = _mapper.Map<IEnumerable<FolderDTO>>(folder);
        return Ok(folderDTO);
    }



}