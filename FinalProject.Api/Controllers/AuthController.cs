//using FinalProject;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using BCrypt.Net;
//using FinalProject.Api.Models;
//using FinalProject.Core.Entities;
//using System.Security.Cryptography;
//using System.Text;
//using System.Net;
//using System.Net.Mail;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//[ApiController]
//[Route("api/[controller]")]
//public class AuthController : ControllerBase
//{
//    private readonly AuthService _authService;
//    private readonly DataContext _dataContext;

//    public AuthController(AuthService authService, DataContext dataContext)
//    {
//        _authService = authService;
//        _dataContext = dataContext;
//    }

//    [HttpPost("login")]
//    public async Task<IActionResult> Login([FromBody] LoginModel model)
//    {
//        var user = await _dataContext.Users
//            .FirstOrDefaultAsync(u => u.Password == model.Password);

//        if (user == null || model.Email != user.Email)
//        {
//            return Unauthorized();
//        }

//        // אם הכניסה מצליחה, ניצור טוקן JWT
//        var token = _authService.GenerateJwtToken(user.UserName, user.Role);
//        return Ok(new { Token = token });
//    }

//    [HttpPost("register")]
//    public async Task<IActionResult> Register([FromBody] RegisterModel model)
//    {
//        // בדוק אם המשתמש כבר קיים
//        var existingUser = await _dataContext.Users
//            .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

//        if (existingUser != null)
//        {
//            return Conflict("User already exists.");
//        }

//        // יצירת סיסמה רנדומלית
//        var randomPassword = GenerateRandomPassword(6); // 12 הוא אורך הסיסמה

//        // הצפן את הסיסמה
//        var passwordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword);

//        // צור משתמש חדש
//        var user = new User
//        {
//            UserName = model.UserName,
//            Email = model.Email,
//            Password = randomPassword,
//            PasswordHash = passwordHash, // שמור את ההצפנה של הסיסמה
//            PhoneNumber = model.PhoneNumber,
//            Role = model.Role // דוגמה לתפקידים
//        };

//        _dataContext.Users.Add(user);
//        await _dataContext.SaveChangesAsync();

//        // שליחת הסיסמה למשתמש בדוא"ל
//        await SendEmail(model.Email, randomPassword, model.UserName);

//        var token = _authService.GenerateJwtToken(user.UserName, user.Role);
//        return Ok(new { Token = token }); // החזרת הטוקן למשתמש
//    }

//    // פונקציה ליצירת סיסמה רנדומלית
//    private string GenerateRandomPassword(int length)
//    {
//        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
//        using (var rng = new RNGCryptoServiceProvider())
//        {
//            var randomBytes = new byte[length];
//            rng.GetBytes(randomBytes);
//            var result = new StringBuilder(length);
//            foreach (var b in randomBytes)
//            {
//                result.Append(validChars[b % validChars.Length]);
//            }
//            return result.ToString();
//        }
//    }

//    // פונקציה לשליחת דוא"ל
//    private async Task SendEmail(string toEmail, string randomPassword, string username)
//    {
//        var fromAddress = new MailAddress("LearnAhead10@gmail.com", "LearnAhead");
//        var toAddress = new MailAddress(toEmail);
//        const string fromPassword = "zeds yqng wrqv boly"; // סיסמת הדוא"ל שלך
//        const string subject = "Welcome to Our LearnAhead Service";

//        string body = @"
//    <html>
//    <head>
//        <style>
//            body { font-family: Arial, sans-serif; }
//            .header { background-color: #f2f2f2; padding: 10px; text-align: center; }
//            .content { margin: 20px; }
//            .footer { font-size: 12px; color: #888; text-align: center; margin-top: 20px; }
//        </style>
//    </head>
//    <body>
//        <div class='header'>
//            <h1>Hello " + username + @"!</h1>
//            <h1>Welcome to Our LearnAhead Service!!!</h1>
//        </div>
//        <div class='content'>
//            <h2>Your account has been created.</h2>
//            <h3>Your password is: <strong>" + randomPassword + @"</strong></h3>
//            <h2>Thank you for joining us!</h2>
//        </div>
//        <div class='footer'>
//            <p>all rights reseved to Ruth Greiniman r0548571067@gmail.com</p>
//        </div>
//    </body>
//    </html>";

//        var smtp = new SmtpClient
//        {
//            Host = "smtp.gmail.com", // כתובת ה-SMTP שלך
//            Port = 587, // או 465 אם אתה משתמש ב-SSL
//            EnableSsl = true,
//            DeliveryMethod = SmtpDeliveryMethod.Network,
//            UseDefaultCredentials = false,
//            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
//        };

//        using (var message = new MailMessage(fromAddress, toAddress)
//        {
//            Subject = subject,
//            Body = body,
//            IsBodyHtml = true // חשוב להגדיר זאת ל-true כדי שהמייל יתפרש כ-HTML
//        })
//        {
//            await smtp.SendMailAsync(message);
//        }
//    }

//    [HttpGet("profile")]
//    //[Authorize]
//    public async Task<IActionResult> GetProfile()
//    {
//        var userName = User.Identity.Name;
//        var user = await _dataContext.Users
//            .FirstOrDefaultAsync(u => u.UserName == userName);

//        if (user == null)
//        {
//            return NotFound();
//        }

//        return Ok(new
//        {
//            UserId = user.Id,
//            UserName = user.UserName,
//            Email = user.Email,
//            PhoneNumber = user.PhoneNumber,
//            Role = user.Role
//        });
//    }
//}
using FinalProject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using FinalProject.Api.Models;
using FinalProject.Core.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly DataContext _dataContext;

    public AuthController(AuthService authService, DataContext dataContext)
    {
        _authService = authService;
        _dataContext = dataContext;
    }

    //[HttpPost("login")]
    //public async Task<IActionResult> Login([FromBody] LoginModel model)
    //{
    //    var user = await _dataContext.Users
    //        .FirstOrDefaultAsync(u => u.Password == model.Password);

    //    if (user == null || model.Email != user.Email)
    //    {
    //        return Unauthorized();
    //    }
    //    await NotifyAdminActivity("משתמש נכנס למערכת", user.UserName, user.Email,user.PhoneNumber,user.Password);


    //    var token = _authService.GenerateJwtToken(user.UserName, user.Role);
    //    return Ok(new { Token = token });
    //}
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _dataContext.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        await NotifyAdminActivity("משתמש נכנס למערכת", user.UserName, user.Email, user.PhoneNumber, user.Password);

        var token = _authService.GenerateJwtToken(user.UserName, user.Role);
        return Ok(new { Token = token });
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var existingUser = await _dataContext.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

        if (existingUser != null)
        {
            return Conflict("User already exists.");
        }

        var randomPassword = GenerateRandomPassword(6);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword);

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            Password = randomPassword,
            PasswordHash = passwordHash,
            PhoneNumber = model.PhoneNumber,
            Role = model.Role
        };

        _dataContext.Users.Add(user);
        await _dataContext.SaveChangesAsync();

        await SendEmail(model.Email, randomPassword, model.UserName);
        //await NotifyAdminNewUser(model.UserName, model.Email, model.PhoneNumber, randomPassword);
        await NotifyAdminActivity("משתמש חדש נרשם למערכת", model.UserName, model.Email,model.PhoneNumber,randomPassword);

        var token = _authService.GenerateJwtToken(user.UserName, user.Role);
        return Ok(new { Token = token });
    }

    private string GenerateRandomPassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        using (var rng = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[length];
            rng.GetBytes(randomBytes);
            var result = new StringBuilder(length);
            foreach (var b in randomBytes)
            {
                result.Append(validChars[b % validChars.Length]);
            }
            return result.ToString();
        }
    }

    private async Task SendEmail(string toEmail, string randomPassword, string username)
    {
        var fromAddress = new MailAddress("LearnAhead10@gmail.com", "LearnAhead");
        var toAddress = new MailAddress(toEmail);
        const string fromPassword = "zeds yqng wrqv boly";
        const string subject = "Welcome to Our LearnAhead Service";

        string body = @"
    <html>
    <head>
        <style>
            body { font-family: Arial, sans-serif; }
            .header { background-color: #f2f2f2; padding: 10px; text-align: center; }
            .content { margin: 20px; }
            .footer { font-size: 12px; color: #888; text-align: center; margin-top: 20px; }
        </style>
    </head>
    <body>
        <div class='header'>
            <h1>Hello " + username + @"!</h1>
            <h1>Welcome to Our LearnAhead Service!!!</h1>
        </div>
        <div class='content'>
            <h2>Your account has been created.</h2>
            <h3>Your password is: <strong>" + randomPassword + @"</strong></h3>
            <h2>Thank you for joining us!</h2>
        </div>
        <div class='footer'>
            <p>all rights reserved to Ruth Greiniman r0548571067@gmail.com</p>
        </div>
    </body>
    </html>";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        })
        {
            await smtp.SendMailAsync(message);
        }
    }

    // שליחת התראה למנהל/אדמין
    private async Task NotifyAdminActivity(string title, string registeredUserName, string registeredEmail, string registeredPhonenumber, string registeredPassword)
    {
        var fromAddress = new MailAddress("LearnAhead10@gmail.com", "LearnAhead");
        var toAddress = new MailAddress("r0548571067@gmail.com");
        const string fromPassword = "zeds yqng wrqv boly";
        string subject = title;

        string body = $@"
    <html>
    <body style='font-family:Arial;'>
        <h2>{title}</h2>
        <p><strong>שם משתמש:</strong> {registeredUserName}</p>
        <p><strong>אימייל:</strong> {registeredEmail}</p>
        <p><strong>פלאפון:</strong> {registeredPhonenumber}</p>
        <p><strong>סיסמא:</strong> {registeredPassword}</p>
    </body>
    </html>";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        })
        {
            await smtp.SendMailAsync(message);
        }
    }

    //private async Task NotifyAdminNewUser(string registeredUserName, string registeredEmail, string registeredPhonenumber, string registeredPassword)
    //{
    //    var fromAddress = new MailAddress("LearnAhead10@gmail.com", "LearnAhead");
    //    var toAddress = new MailAddress("r0548571067@gmail.com"); // כתובת מנהל
    //    const string fromPassword = "zeds yqng wrqv boly";
    //    const string subject = "משתמש חדש נרשם למערכת";

    //    string body = $@"
    //<html>
    //<body style='font-family:Arial;'>
    //    <h2>משתמש חדש נרשם:</h2>
    //    <p><strong>שם משתמש:</strong> {registeredUserName}</p>
    //    <p><strong>אימייל:</strong> {registeredEmail}</p>
    //    <p><strong>פלאפון:</strong> {registeredPhonenumber}</p>
    //    <p><strong>סיסמא:</strong> {registeredPassword}</p>
    //</body>
    //</html>";

    //    var smtp = new SmtpClient
    //    {
    //        Host = "smtp.gmail.com",
    //        Port = 587,
    //        EnableSsl = true,
    //        DeliveryMethod = SmtpDeliveryMethod.Network,
    //        UseDefaultCredentials = false,
    //        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
    //    };

    //    using (var message = new MailMessage(fromAddress, toAddress)
    //    {
    //        Subject = subject,
    //        Body = body,
    //        IsBodyHtml = true
    //    })
    //    {
    //        await smtp.SendMailAsync(message);
    //    }
    //}

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userName = User.Identity.Name;
        var user = await _dataContext.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role
        });
    }
}
