using AtonTestTask.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AtonTestTask.Services;
using System.Security.Claims;
using System.Text.Json;
using AtonTestTask.Models;

namespace AtonTestTask.Controllers
{
    /// <summary>
    /// Контроллер пользователя
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        /// <summary>
        /// Создаёт нового пользователя
        /// </summary>
        /// <param name="userLogin">Логин пользователя от которого совершается запрос</param>
        /// <param name="userPassword">Пароль пользователя от которого совершается запрос</param>
        /// <param name="login">Логин нового пользователя</param>
        /// <param name="password">Пароль нового пользователя</param>
        /// <param name="name">Имя нового пользователя</param>
        /// <param name="gender">Пол нового пользователя</param>
        /// <param name="birthday">День рождение нового пользователя</param>
        /// <param name="isAdmin">Является ли пользователь админом</param>
        /// <response code="200">Пользователь создан</response>
        /// <response code="400">
        /// Не удалось авторизоваться, пользователь не админ или пользователь с таким логином уже существует
        /// </response>
        [Route("createuser")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateUser(string userLogin, string userPassword, string login, string password,
    string name, int gender, DateTime? birthday, bool isAdmin)
        {
            User user = new User
            {
                Guid = System.Guid.NewGuid(),
                Login = login,
                Password = password,
                Name = name,
                Gender = gender,
                Birthday = birthday.GetValueOrDefault().ToUniversalTime(),
                Admin = isAdmin,
                CreatedOn = DateTime.UtcNow
            };

            var result = UserService.Add(userLogin, userPassword, user).Result;
            if (result)
                return Ok("Пользователь успешно создан");
            else
                return BadRequest("Не удалось авторизоваться или данный логин уже занят");
        }

        /// <summary>
        /// Обновляет информацию о пользователе
        /// </summary>
        /// <param name="userLogin">Логин пользователя от которого совершается запрос</param>
        /// <param name="userPassword">Пароль пользователя от которого совершается запрос</param>
        /// <param name="editUserLogin">Логин пользователя которого необходимо изменить</param>
        /// <param name="editUserName"></param>
        /// <param name="editUserGender"></param>
        /// <param name="editUserBirthday"></param>
        /// <response code="200">Данные изменены</response>
        /// <response code="400">
        /// Не удалось автроизоваться или найти пользователя для изменения
        /// </response>
        [Route("updateuserinfo")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateUserInfo(string userLogin, string userPassword, string editUserLogin, string? editUserName,
            int? editUserGender, DateTime? editUserBirthday)
        {
            var result = UserService.UpdateUserInfo(userLogin, userPassword,
                editUserLogin, editUserName, editUserGender, editUserBirthday).Result;
            if (result)
                return Ok("Данные пользователя успешно изменены");
            else
                return BadRequest("Не удалось авторизоваться или пользователь для изменения не найден");

        }

        /// <summary>
        /// Обновление пароля пользователя
        /// </summary>
        /// <param name="userLogin">Логин пользователя от которого совершается запрос</param>
        /// <param name="userPassword">Пароль пользователя от которого совершается запрос</param>
        /// <param name="editUserLogin">Логин пользователя которому необходим изменить пароль</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <response code="200">Пароль успешно изменён</response>
        /// <response code="400">Не удалось автроизоваться или пользователь не найден</response>
        [Route("updatepassword")]
        [HttpPut]
        public IActionResult UpdateUserPassword(string userLogin, string userPassword, string editUserLogin, string newPassword)
        {
            var result = UserService.UpdateUserPassword(userLogin, userPassword, editUserLogin, newPassword).Result;
            if (result)
                return Ok("Пароль пользователя изменён");
            else
                return BadRequest("Не удалось авторизоваться или пользователь для изменения не найден");
        }

        /// <summary>
        /// Изменение логин пользователя
        /// </summary>
        /// <param name="userLogin">Логин пользователя от которого совершается запрос</param>
        /// <param name="userPassword">Пароль пользователя от которого совершается запрос</param>
        /// <param name="editUserLogin">Логин пользователя которому необходимо изменить логин</param>
        /// <param name="newLogin">Новый логин</param>
        /// <response code="200">Логин успешно изменён</response>
        /// <response code="400">Не удалось автроизоваться или пользователь не найден</response>
        [Route("updatelogin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateLogin(string userLogin, string userPassword, string editUserLogin, string newLogin)
        {
            var result = UserService.UpdateUserLogin(userLogin, userPassword, editUserLogin, newLogin).Result;
            if (result)
                return Ok("Логин пользователя изменён");
            else
                return BadRequest("Не удалось авторизоваться или пользователь для изменения не найден");
        }

        /// <summary>
        /// Получить список пользователей
        /// </summary>
        /// <param name="userLogin">Логин пользователя от которого совершается запрос</param>
        /// <param name="userPassword">Пароль пользователя от которого совершается запрос</param>
        /// <response code="200">Возвращяется список пользователей</response>
        /// <response code="400">
        /// Пользователь не администратор, не удалось автроизоваться или список пользователей пуст
        /// </response>
        [Route("getallusers")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllUsers(string userLogin, string userPassword)
        {
            var users = UserService.GetAllUsers(userLogin, userPassword).Result;
            if (users != null)
                return Ok(JsonSerializer.Serialize<List<User>>(users));
            else
                return BadRequest("Не удалось авторизоваться или список пользователей пуст");
        }

        /// <summary>
        /// Возвращаяет пользователя с соответствующим логином
        /// </summary>
        /// <param name="userLogin">Логин пользователя от которого совершается запрос</param>
        /// <param name="userPassword">Пароль пользователя от которого совершается запрос</param>
        /// <param name="searchUserLogin">Логин для поиска</param>
        /// <response code="200">Возвращаяет пользователя</response>
        /// <response code="400">
        /// Пользователь не администратор, не удалось автроизоваться или список пользователей пуст
        /// </response>
        [Route("getuser")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetUser(string userLogin, string userPassword, string searchUserLogin)
        {
            var user = UserService.GetUser(userLogin, userPassword, searchUserLogin).Result;
            if (user != null)
                return Ok(JsonSerializer.Serialize<RowsUser>(user));
            else
                return BadRequest("Не удалось авторизоваться или найти пользователя");
        }

        /// <summary>
        /// Возвращяет пользователя по логину и паролю
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <response code="200">Возвращяет пользователя</response>
        /// <response code="400">
        /// Не удалось автроизоваться или пользователь не найден
        /// </response>
        [Route("getyourself")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetYourself(string login, string password)
        {
            var yourself = UserService.GetYourself(login, password).Result;
            if (yourself != null)
                return Ok(JsonSerializer.Serialize<User>(yourself));
            else
                return BadRequest("Пользователь не найден или заблокирован");
        }

        /// <summary>
        /// Возвращяет список пользователей старше определённого возраста
        /// </summary>
        /// <param name="userLogin">Логин от кого совершается запрос</param>
        /// <param name="userPassword">Пароль от кого соврешается запрос</param>
        /// <param name="age"></param>
        /// <response code="200">Возвращяет список пользователей</response>
        /// <response code="400">
        /// Пользователь не администратор, не удалось автроизоваться или список пользователей пуст
        /// </response>
        [Route("getusersolderthan")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetUsersOlderThan(string userLogin, string userPassword, int age)
        {
            var user = UserService.GetUsersOlderThan(userLogin, userPassword, age).Result;
            if (user != null)
                return Ok(JsonSerializer.Serialize<List<User>>(user));
            else
                return BadRequest("Не удалось авторизоваться или пользователь пуст");
        }

        /// <summary>
        /// Полностью удалить пользователя
        /// </summary>
        /// <param name="userLogin">Логин от кого совершается запрос</param>
        /// <param name="userPassword">Пароль от кого соврешается запрос</param>
        /// <param name="login">Логин пользователя которого необходимо удалить</param>
        /// <response code="200">Пользователь удалён</response>
        /// <response code="400">
        /// Пользователь не администратор, не удалось автроизоваться или список пользователей пуст
        /// </response>
        [Route("harddelete")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult HardDelete(string userLogin, string userPassword, string login)
        {
            var result = UserService.HardDelete(userLogin, userPassword, login).Result;
            if (result)
                return Ok("Пользователь полностью удалён");
            else
                return BadRequest("Не удалось автроизоваться или пользователь не найден");
        }

        /// <summary>
        /// "Мягко" удаляет пользователя
        /// </summary>
        /// <param name="userLogin">Логин от кого совершается запрос</param>
        /// <param name="userPassword">Пароль от кого соврешается запрос</param>
        /// <param name="login">Логин пользователя которого необходимо удалить</param>
        /// <response code="200">Пользователь удалён</response>
        /// <response code="400">
        /// Пользователь не администратор, не удалось автроизоваться или список пользователей пуст
        /// </response>
        [Route("softdelete")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SoftDelete(string userLogin, string userPassword, string login)
        {
            var result = UserService.SoftDelete(userLogin, userPassword, login).Result;
            if (result)
                return Ok("Пользователь удалён");
            else
                return BadRequest("Не удалось авторизоваться или пользователь не найден");
        }

        /// <summary>
        /// Восстанволение пользователя
        /// </summary>
        /// <param name="userLogin">Логин от кого совершается запрос</param>
        /// <param name="userPassword">Пароль от кого соврешается запрос</param>
        /// <param name="login">Логин пользователя которого необходимо восстановить</param>
        /// <response code="200">Пользователь восстановлен</response>
        /// <response code="400">
        /// Пользователь не администратор, не удалось автроизоваться или список пользователей пуст
        /// </response>
        [Route("recoveruser")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult RecoverUser(string userLogin, string userPassword, string login)
        {
            var result = UserService.RecoverUser(userLogin, userPassword, login).Result;
            if (result)
                return Ok("Пользователь восстановлен");
            else
                return BadRequest("Не удалось автроизоваться или пользователь не найден");
        }
    }
}
