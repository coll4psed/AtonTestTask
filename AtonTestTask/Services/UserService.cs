using AtonTestTask.Context;
using AtonTestTask.Entities;
using AtonTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace AtonTestTask.Services
{
    public static class UserService
    {
        public static async Task<bool> Add(string login, string password, User user)
        {
            using (var db = new AtonTestTaskContext())
            {
                var queryUser = await db.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
                if (queryUser != null && queryUser.Admin == true)
                {
                    var newUser = await db.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
                    if (newUser == null)
                    {
                        user.CreatedBy = queryUser.Name;
                        await db.Users.AddAsync(user);
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
        }

        public static async Task<bool> UpdateUserInfo(string userLogin, string userPassword, 
            string editUserLogin, string? name, int? gender, DateTime? birthday)
        {
            using (var db = new AtonTestTaskContext())
            {
                var queryUser = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin && u.Password == userPassword);
                if (queryUser != null)
                {
                    var editUser = await db.Users.FirstOrDefaultAsync(u => u.Login == editUserLogin && u.RevokedOn == null);
                    if (editUser != null && (queryUser.Admin == true || queryUser.Login == editUser.Login))
                    {
                        editUser.Name = String.IsNullOrEmpty(name) ? editUser.Name : name;
                        editUser.Gender = (int)(String.IsNullOrEmpty(gender.ToString()) ? editUser.Gender : gender);
                        editUser.Birthday = birthday.GetValueOrDefault().ToUniversalTime();
                        editUser.ModifiedOn = DateTime.UtcNow;
                        editUser.ModifiedBy = queryUser.Name;
                        db.Entry(editUser).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
        }
        
        public static async Task<bool> UpdateUserPassword(string userLogin, string userPassword, 
            string editUserLogin, string newPassword)
        {
            using (var db = new AtonTestTaskContext())
            {
                var queryUser = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin && u.Password == userPassword);
                if (queryUser != null)
                {
                    var editUser = await db.Users.FirstOrDefaultAsync(u => u.Login == editUserLogin && u.RevokedOn == null);
                    if (editUser != null && (queryUser.Admin == true || queryUser.Login == editUser.Login))
                    {
                        editUser.Password = newPassword;
                        db.Entry(editUser).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
        }

        public static async Task<bool> UpdateUserLogin(string userLogin, string userPassword,
            string editUserLogin, string newLogin)
        {
            using (var db = new AtonTestTaskContext())
            {
                var queryUser = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin && u.Password == userPassword);
                if (queryUser != null)
                {
                    var editUser = await db.Users.FirstOrDefaultAsync(u => u.Login == editUserLogin && u.RevokedOn == null);
                    if (editUser != null && (queryUser.Admin == true || queryUser.Login == editUser.Login))
                    {
                        try
                        {
                            editUser.Login = newLogin;
                            db.Entry(editUser).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                return false;
            }
        }

        public async static Task<List<User>> GetAllUsers(string login, string password)
        {
            using (var db = new AtonTestTaskContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
                if (user != null && user.Admin == true)
                {
                    return await db.Users
                        .Where(u => u.RevokedOn == null)
                        .OrderBy(u => u.CreatedOn)
                        .ToListAsync();
                }
                return null;
            }
        }

        public async static Task<RowsUser> GetUser(string userLogin, string userPassword, string searchUserLogin)
        {
            using (var db = new AtonTestTaskContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin && u.Password == userPassword);
                if (user != null && user.Admin == true)
                {
                    return await db.Users
                        .Where(u => u.Login == searchUserLogin)
                        .Select(u => new RowsUser
                        {
                            Name = u.Name,
                            Gender = u.Gender,
                            Birthday = u.Birthday,
                            RevokedOn = u.RevokedOn,
                            RevokedBy = u.RevokedBy
                        })
                        .FirstOrDefaultAsync();
                }
                return null;
            }
        }

        public async static Task<User> GetYourself(string login, string password)
        {
            using (var db = new AtonTestTaskContext())
            {
                var yourself = await db.Users.Where(u => u.RevokedOn == null).FirstOrDefaultAsync();
                if (yourself != null && yourself.RevokedOn == null)
                    return yourself;
                else
                    return null;
            }
        }

        public async static Task<List<User>> GetUsersOlderThan(string login, string password, int age)
        {
            DateTime now = DateTime.UtcNow;
            using (var db = new AtonTestTaskContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
                if (user != null && user.Admin == true)
                    return await db.Users
                        .Where(u => u.Birthday.Value.AddYears(age) < now)
                        .ToListAsync();
                else
                    return null;
            }
        }

        public async static Task<bool> HardDelete(string userLogin, string userPassword, string login)
        {
            using (var db = new AtonTestTaskContext())
            {
                var queryUser = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin && u.Password == userPassword);
                if (queryUser != null && queryUser.Admin == true)
                {
                    var deleteUser = await db.Users.FirstOrDefaultAsync(u => u.Login == login);
                    if (deleteUser != null)
                    {
                        db.Users.Remove(deleteUser);
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
        }

        public async static Task<bool> SoftDelete(string userLogin, string userPassword, string login)
        {
            using (var db = new AtonTestTaskContext())
            {
                var queryUser = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin && u.Password == userPassword);
                if (queryUser != null && queryUser.Admin == true)
                {
                    var deleteUser = await db.Users
                        .Where(u => u.RevokedOn == null)
                        .FirstOrDefaultAsync(u => u.Login == login);
                    if (deleteUser != null)
                    {
                        deleteUser.ModifiedOn = DateTime.UtcNow;
                        deleteUser.ModifiedBy = queryUser.Name;
                        deleteUser.RevokedOn = DateTime.UtcNow;
                        deleteUser.RevokedBy = queryUser.Name;
                        db.Entry(deleteUser).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
        }

        public async static Task<bool> RecoverUser(string userLogin, string userPassword, string login)
        {
            using (var db = new AtonTestTaskContext())
            {
                var queryUser = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin && u.Password == userPassword);
                if (queryUser != null && queryUser.Admin == true)
                {
                    var recoverUser = await db.Users
                        .Where(u => u.RevokedOn != null)
                        .FirstOrDefaultAsync();
                    if (recoverUser != null)
                    {
                        recoverUser.ModifiedOn = DateTime.UtcNow;
                        recoverUser.ModifiedBy = queryUser.Name;
                        recoverUser.RevokedOn = null;
                        recoverUser.RevokedBy = null;
                        db.Entry(recoverUser).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
