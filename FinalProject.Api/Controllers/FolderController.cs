using AutoMapper;
using FinalProject.Api.Models;
using FinalProject.Core.DTOs;
using FinalProject.Core.Entities;
using FinalProject.Core.Services;
using Google.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
                numberOfLessons = value.numberOfLessons,
                price = value.price
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



        [Authorize]
        [HttpPost("purchase/{folderId}")]
        public async Task<IActionResult> PurchaseFolder(int folderId)
        {
            var folder = await _dataContext.Folders.FindAsync(folderId);

            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int Id;

            int.TryParse(user, out Id);
            var student = await _dataContext.Users.FindAsync(Id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            if (folder == null || user == null)
                return NotFound("Folder or user not found.");

            var alreadyPurchased = await _dataContext.FolderUsers
                .AnyAsync(fu => fu.UserId == Id && fu.FolderId == folderId);

            if (alreadyPurchased)
                return Conflict("Folder already purchased.");

            _dataContext.FolderUsers.Add(new FolderUser
            {
                UserId = Id,
                FolderId = folderId,
                PurchaseDate = DateTime.UtcNow
            });

            await _dataContext.SaveChangesAsync();
            return Ok("Purchase successful.");
        }

        //[Authorize]
        //[HttpGet("check-purchase/{courseId}")]
        //public async Task<IActionResult> CheckPurchase(int courseId)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    int Id;
        //    int.TryParse(userId, out Id);

        //    // בדוק אם הקורס נרכש על ידי המשתמש
        //    var alreadyPurchased = await _dataContext.FolderUsers
        //        .AnyAsync(fu => fu.UserId == Id && fu.FolderId == courseId);
        //    if(alreadyPurchased)
        //       return Ok(true);
        //    return Ok(false);
        //}

        //[Authorize]
        //[HttpGet("check-purchase/{courseId}")]
        //public async Task<bool> CheckPurchase(int folderId)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    int Id;
        //    int.TryParse(userId, out Id);

        //    // בדוק אם הקורס נרכש על ידי המשתמש
        //    var alreadyPurchased = await _dataContext.FolderUsers
        //        .AnyAsync(fu => fu.UserId == Id && fu.FolderId == folderId);
        //    return alreadyPurchased;
        //}
        [HttpGet("check-purchase/{folderId}")]
        public async Task<bool> CheckPurchase(int folderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int Id;
            int.TryParse(userId, out Id);

            // בדוק אם הקורס נרכש על ידי המשתמש
            var alreadyPurchased = await _dataContext.FolderUsers
                .AnyAsync(fu => fu.UserId == Id && fu.FolderId == folderId);
            return alreadyPurchased;
        }

        //[HttpPost("{userId}/purchase/{folderId}")]
        //public async Task<IActionResult> PurchaseFolder(int userId, int folderId)
        //{
        //    var folder = await _dataContext.Folders.FindAsync(folderId);
        //    //var user = await _dataContext.Users.FindAsync(userId);
        //    var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (folder == null || user == null)
        //        return NotFound("Folder or user not found.");

        //    var alreadyPurchased = await _dataContext.FolderUsers
        //        .AnyAsync(fu => fu.UserId == userId && fu.FolderId == folderId);

        //    if (alreadyPurchased)
        //        return Conflict("Folder already purchased.");

        //    _dataContext.FolderUsers.Add(new FolderUser
        //    {
        //        UserId = userId,
        //        FolderId = folderId,
        //        PurchaseDate = DateTime.UtcNow
        //    });

        //    await _dataContext.SaveChangesAsync();
        //    return Ok("Purchase successful.");
        //}

        [HttpGet("{folderId}/students")]
        public async Task<IActionResult> GetStudentsByFolder(int folderId)
        {
            var folder = await _dataContext.Folders.FindAsync(folderId);
            if (folder == null)
                return NotFound("Folder not found.");

            var studentUsers = await _dataContext.FolderUsers
                .Where(fu => fu.FolderId == folderId)
                .Include(fu => fu.User) // הנחה שיש לנו קשר ניווט ל-User
                .Select(fu => new
                {
                    fu.User.Id,
                    fu.User.UserName,
                    fu.User.Email
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = studentUsers,
                message = "התלמידים נטענו בהצלחה"
            });
        }

        //[HttpPost("send-teacher-email")]
        //public async Task<IActionResult> SendEmailToTeacher(string fromEmail, string toEmail, string subject, string bodyHtml)
        //{
        //    try
        //    {
        //        var fromAddress = new MailAddress("LearnAhead10@gmail.com", "LearnAhead");
        //        var toAddress = new MailAddress(toEmail);
        //        var replyToAddress = new MailAddress(fromEmail);

        //        const string fromPassword = "zeds yqng wrqv boly"; // סיסמת המייל של האתר

        //        using var smtp = new SmtpClient
        //        {
        //            Host = "smtp.gmail.com",
        //            Port = 587,
        //            EnableSsl = true,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            UseDefaultCredentials = false,
        //            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //        };

        //        using var message = new MailMessage(fromAddress, toAddress)
        //        {
        //            Subject = subject,
        //            Body = bodyHtml,
        //            IsBodyHtml = true
        //        };

        //        message.ReplyToList.Add(replyToAddress);

        //        await smtp.SendMailAsync(message);

        //        return Ok("Email sent successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // כאן ניתן לרשום לוג שגיאה או טיפול שגיאה נוסף
        //        return StatusCode(500, "Failed to send email: " + ex.Message);
        //    }
        //}
        //[HttpPost("send-teacher-email")]
        //public async Task<IActionResult> SendEmailToTeacher(int folderId, string bodyHtml)
        //{
        //    try
        //    {
        //        // שלוף את המידע ממסד הנתונים
        //        var folder = await _dataContext.Folders.FindAsync(folderId);
        //        if (folder == null)
        //        {
        //            return NotFound("Folder not found.");
        //        }

        //        var user = await _dataContext.Users.FindAsync(folder.TeacherId);
        //        if (user == null)
        //        {
        //            return NotFound("Teacher not found.");
        //        }

        //        string fromEmail = "LearnAhead10@gmail.com"; // המייל הקבוע
        //        string toEmail = user.Email; // כתובת המייל של המורה
        //        string subject = folder.Title; // שם הקורס

        //        var fromAddress = new MailAddress(fromEmail, "LearnAhead");
        //        var replyToAddress = new MailAddress(fromEmail);

        //        const string fromPassword = "zeds yqng wrqv boly"; // סיסמת המייל של האתר

        //        using var smtp = new SmtpClient
        //        {
        //            Host = "smtp.gmail.com",
        //            Port = 587,
        //            EnableSsl = true,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            UseDefaultCredentials = false,
        //            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //        };

        //        using var message = new MailMessage(fromAddress, new MailAddress(toEmail))
        //        {
        //            Subject = subject,
        //            Body = bodyHtml,
        //            IsBodyHtml = true
        //        };

        //        message.ReplyToList.Add(replyToAddress);

        //        await smtp.SendMailAsync(message);

        //        return Ok("Email sent successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Failed to send email: " + ex.Message);
        //    }
        //}
        [Authorize]
        [HttpPost("send-teacher-email")]
        public async Task<IActionResult> SendEmailToTeacher(int folderId, string bodyHtml)
        {
            try
            {
                // שלוף את המידע ממסד הנתונים
                var folder = await _dataContext.Folders.FindAsync(folderId);
                if (folder == null)
                {
                    return NotFound("Folder not found.");
                }

                var user = await _dataContext.Users.FindAsync(folder.TeacherId);
                if (user == null)
                {
                    return NotFound("Teacher not found.");
                }

                // הנח שיש לך גישה למידע על התלמיד, לדוגמה מהקשר הנוכחי
                var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ////var studentIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //Console.WriteLine($"Student ID from JWT: {studentId}"); // הדפס את המזהה

                //var studentIdString = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value?.Trim();

                //if (string.IsNullOrEmpty(studentIdString))
                //{
                //    return BadRequest("Student ID is missing.");
                //}// או כל דרך אחרת לשלוף את מזהה התלמיד
                int Id;
                //if (!int.TryParse(studentIdString, out Id))
                //{
                //    return BadRequest("Invalid student ID.");
                //} 
                int.TryParse(studentId, out Id);
                var student = await _dataContext.Users.FindAsync(Id);

                if (student == null)
                {
                    return NotFound("Student not found.");
                }

                string fromEmail = "LearnAhead10@gmail.com"; // המייל הקבוע
                string toEmail = user.Email; // כתובת המייל של המורה
                string subject = folder.Title; // שם הקורס

                // הוסף את כתובת התלמיד לתוכן המייל
                bodyHtml += $"<br/><br/>כתובת מייל של התלמיד: {student.Email}";

                var fromAddress = new MailAddress(fromEmail, "LearnAhead");
                var replyToAddress = new MailAddress(fromEmail);

                const string fromPassword = "zeds yqng wrqv boly"; // סיסמת המייל של האתר

                using var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using var message = new MailMessage(fromAddress, new MailAddress(toEmail))
                {
                    Subject = subject,
                    Body = bodyHtml,
                    IsBodyHtml = true
                };

                message.ReplyToList.Add(replyToAddress);

                await smtp.SendMailAsync(message);

                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to send email: " + ex.Message);
            }
        }

        //[Authorize(Roles = "Teacher")]
        [HttpDelete("delete-course/{folderId}")]
        public async Task<IActionResult> DeleteCourse(int folderId)
        {
            var folder = await _dataContext.Folders.FindAsync(folderId);

            if (folder == null || folder.CourseId == null || folder.LessonId != null)
                return NotFound("Course not found.");

            _dataContext.Folders.Remove(folder);
            await _dataContext.SaveChangesAsync();

            return Ok("Course deleted successfully.");
        }

        //[Authorize(Roles = "Teacher")]
        [HttpPut("update-course/{folderId}")]
        public async Task<IActionResult> UpdateCourse(int folderId, [FromBody] FolderModel updated)
        {
            var folder = await _dataContext.Folders.FindAsync(folderId);

            if (folder == null || folder.CourseId == null || folder.LessonId != null)
                return NotFound("Course not found.");

            // רק שדות מותרים לעדכון:
            folder.Title = updated.Title ?? folder.Title;
            folder.description = updated.description ?? folder.description;
            folder.price = updated.price != 0 ? updated.price : folder.price;
            folder.numberOfLessons = updated.numberOfLessons != 0 ? updated.numberOfLessons : folder.numberOfLessons;

            await _dataContext.SaveChangesAsync();
            return Ok("Course updated successfully.");
        }

        //[Authorize(Roles = "Teacher")]
        [HttpDelete("delete-lesson/{folderId}")]
        public async Task<IActionResult> DeleteLesson(int folderId)
        {
            var folder = await _dataContext.Folders.FindAsync(folderId);

            if (folder == null || folder.LessonId == null)
                return NotFound("Lesson not found.");

            _dataContext.Folders.Remove(folder);
            await _dataContext.SaveChangesAsync();

            return Ok("Lesson deleted successfully.");
        }

        //[Authorize(Roles = "Teacher")]
        [HttpPut("update-lesson/{folderId}")]
        public async Task<IActionResult> UpdateLesson(int folderId, [FromBody] LessonModel updated)
        {
            var folder = await _dataContext.Folders.FindAsync(folderId);

            if (folder == null || folder.LessonId == null)
                return NotFound("Lesson not found.");

            folder.Title = updated.Title ?? folder.Title;
            folder.description = updated.description ?? folder.description;

            await _dataContext.SaveChangesAsync();
            return Ok("Lesson updated successfully.");
        }


    }
}
