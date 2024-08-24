using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Data.Entities;
using ASP.NET_Classwork.Migrations;
using ASP.NET_Classwork.Services.KDF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ASP.NET_Classwork.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IKdfService _kdfService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(DataContext dataContext, IKdfService kdfService, ILogger<AuthController> logger)
        {
            _dataContext = dataContext;
            _kdfService = kdfService;
            _logger = logger;
        }

        [HttpGet]
        public object DoGet(String input, String password)
        {
            if (String.IsNullOrEmpty(input) || String.IsNullOrEmpty(password))
            {
                return new
                {
                    status = "Error",
                    code = 400,
                    message = "Email/Name/Birthday and password must not be empty"
                };
            }

            var emailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            var nameRegex = new Regex(@"^\w{2,}(\s+\w{2,})*$");
            var birthdayRegex = new Regex(@"^(0[1-9]|[12][0-9]|3[01])[-./](0[1-9]|1[0-2])[-./](\d{4})$");

            var user = emailRegex.IsMatch(input) ? _dataContext.Users.FirstOrDefault(u => u.Email == input && u.DeleteDt == null) :
                nameRegex.IsMatch(input) ? _dataContext.Users.FirstOrDefault(u => u.Name == input && u.DeleteDt == null) :
                birthdayRegex.IsMatch(input) ? _dataContext.Users.FirstOrDefault(u => u.Birthdate == Convert.ToDateTime(input) && u.DeleteDt == null) :
                null;

            // Розшифрувати DK неможливо, тому повторюємо розрахунок DK з сіллю, що зберігається у користувача, та паролем, який був переданий
            if (user != null && _kdfService.DerivedKey(password, user.Salt) == user.Dk)
            {
                var activeToken = _dataContext.Tokens.FirstOrDefault(t => t.UserId == user.Id && t.ExpiresAt > DateTime.Now);

                if (activeToken != null)
                {
                    HttpContext.Session.SetString("token", activeToken.Id.ToString());
                    return new
                    {
                        status = "Ok",
                        code = 200,
                        message = activeToken.Id
                    };
                }
                else
                {
                    // Генеруємо новий токен
                    Token token = new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        ExpiresAt = DateTime.Now.AddHours(3)
                    };

                    _dataContext.Tokens.Add(token);
                    _dataContext.SaveChanges();

                    // Зберігаємо токен у сесії
                    HttpContext.Session.SetString("token", token.Id.ToString());
                    return new
                    {
                        status = "Ok",
                        code = 200,
                        message = token.Id // передаємо новий токен клієнту
                    };
                }
            }
            else
            {
                return new
                {
                    status = "Reject",
                    code = 401,
                    message = "Credentials rejected"
                };
            }
        }

        [HttpDelete]
        public object DoDelete()
        {
            HttpContext.Session.Remove("token");
            return "Ok";
        }

        [HttpPut]
        public async Task<object> DoPutAsync()
        {
            String body = await new StreamReader(Request.Body).ReadToEndAsync();

            _logger.LogWarning(body);

            JsonNode json = JsonSerializer.Deserialize<JsonNode>(body) ?? throw new Exception("JSON in body is invalid");

            String? email = json["email"]?.GetValue<String>();
            String? name = json["name"]?.GetValue<String>();
            String? birthdate = json["birthdate"]?.GetValue<String>();
            String? oldPassword = json["oldPassword"]?.GetValue<String>();
            String? newPassword = json["newPassword"]?.GetValue<String>();

            if (email == null &&  name == null && birthdate == null && oldPassword == null && newPassword == null)
            {
                return new {
                    status = "Error",
                    code = 400,
                    message = "No data" 
                };
            }

            Guid userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value);

            var user = _dataContext.Users.Find(userId);
            if (user == null)
            {
                return new
                {
                    status = "Error",
                    code = 403,
                    message = "Forbidden"
                };
            }

            if (email != null)
            {
                var emailRegex = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
                if (!emailRegex.IsMatch(email))
                {
                    return new
                    {
                        status = "Error",
                        code = 422,
                        message = "Email match no pattern"
                    };
                }
            }

            DateTime? birthDateTime = null;
            if (birthdate != null)
            {
                try
                {
                    birthDateTime = DateTime.Parse(birthdate);
                }
                catch 
                {
                    return new
                    {
                        status = "Error",
                        code = 422,
                        message = "Birthdate unparseable"
                    };
                }
            }

            if (oldPassword != null && newPassword != null)
            {
                if (_kdfService.DerivedKey(oldPassword, user.Salt) != user.Dk)
                {
                    return new
                    {
                        status = "Error",
                        code = 400,
                        message = "The old password does not match the new one"
                    };
                }

                var passwordRegex = new Regex(@"^(?=.*\d)(?=.*\D)(?=.*\W).+$");
                if (!passwordRegex.IsMatch(newPassword))
                {
                    return new
                    {
                        status = "Error",
                        code = 422,
                        message = "Password match no pattern"
                    };
                }
            }
            else if (oldPassword == null && newPassword == null) {}
            else
            {
                return new
                {
                    status = "Error",
                    code = 422,
                    message = "One of the passwords are empty"
                };
            }

            if (email != null)
            {
                user.Email = email;
            }
            if (name != null)
            {
                user.Name = name;
            }
            if (birthDateTime != null)
            {
                user.Birthdate = birthDateTime;
            }
            if (newPassword != null)
            {
                user.Dk = _kdfService.DerivedKey(newPassword, user.Salt);
            }

            _logger.LogInformation(json["name"]?.GetValue<String>());
            _logger.LogInformation(json["oldPassword"]?.GetValue<String>());
            _logger.LogInformation(json["newPassword"]?.GetValue<String>()); 

            _dataContext.SaveChanges();

            return new 
            { 
                status = "OK",
                code = 200,
                message = "Updated"
            };
        }

        public async Task<object> DoOther()
        {
            switch(Request.Method)
            {
                case "UNLINK":
                {
                    return await DoUnlink();
                }
                case "LINK":
                {
                    String body = await new StreamReader(Request.Body).ReadToEndAsync();
                    JsonNode json = JsonSerializer.Deserialize<JsonNode>(body) ?? throw new Exception("JSON in body is invalid");

                    String? email = json["email"]?.GetValue<String>();
                    String? password = json["password"]?.GetValue<String>();
                    String? regDate = json["regDate"]?.GetValue<String>();

                    return await DoLink(email, password, regDate);
                }
                default:
                {
                    return new
                    {
                        status = "Error",
                        code = 405,
                        message = "Method not allowed"
                    };
                }
            }
        }

        private async Task<object> DoLink(String email, String password, String regDate)
        {
            var users = _dataContext.Users.Where(u => u.Registered.Date.Equals(DateTime.Parse(regDate).Date));
            User? recoveredUser = null;
            foreach (var user in users)
            {
                if (_kdfService.DerivedKey(password, user.Salt) == user.Dk)
                {
                    recoveredUser = user;
                    break;
                }
            }

            if (recoveredUser == null)
            {
                return new
                {
                    status = "Error",
                    code = 404,
                    message = "User not found"
                };
            }

            recoveredUser.Email = email;
            recoveredUser.Name = "Anonymous";
            recoveredUser.DeleteDt = null;

            await _dataContext.SaveChangesAsync();

            return new
            {
                status = "OK",
                code = 200,
                message = "Recovered"
            };
        }

        private async Task<object> DoUnlink()
        {
            Guid userId;

            try
            {
                userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"DoUnlink Exception : {ex}");
                return new
                {
                    status = "Error",
                    code = 401,
                    message = "unAuthorized"
                };
            }

            var user = await _dataContext.Users.FindAsync(userId);

            if (user == null)
            {
                return new
                {
                    status = "Error",
                    code = 403,
                    message = "Forbidden"
                };
            }

            user.DeleteDt = DateTime.Now;
            user.Name = "";
            user.Email = "";
            user.Birthdate = null;
            if (user.Avatar != null)
            {
                String path = "./Uploads/User/";
                System.IO.File.Delete(path + user.Avatar);
                user.Avatar = null;
            }
            await _dataContext.SaveChangesAsync();
            this.DoDelete();

            return new
            {
                status = "OK",
                code = 200,
                message = $"Для відновлення введіть дату реєстрації ({user.Registered.ToString().Split(" ")[0]}) та свій пароль"
            };
        }
    }
}

// Контролери розрізняють MVC та API 
// MVC - різні адреси ведуть на різні дії (action)
//       /Home/Index -> Index()
//       /Home/Db    -> Db()
//
// API - різні методи запиту ведуть на різні дії 
//       GET  /api/auth  -> DoGet()
//       POST /api/auth  -> DoPost()
//       PUT  /api/auth  -> DoPut()

// Кодування - переведення символів однієї абетки у символи іншої абетки (на відміну від шифрування - без секретів)
// Розрізняють символьні та транспортні кодування
// Символьні - таблиці код-символ ASCII, UTF-8
// Транспортні - для усунення спец. символів, що вживаються у протоколах (?, &, ...)

// Токени авоторизації
// Токен - "жетон", "перепустка" - дані, що видаються як результат автентифікації і далі використовується для "підтвердження особи" - авторизації

// 