using HotelwebLisMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelwebLisMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        private readonly HotelWebLisDBContext context;

        public HomeController(HotelWebLisDBContext context)
        {
            this.context = context;

        }



        [HttpGet("GetLogin")]
        public async Task<ActionResult<object>> Login()
        {
            var users = await context.Users
                .Where(u => u.IsActive == true)
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.PasswordHash,
                    u.RoleId,
                    u.BranchId
                })
                .ToListAsync();

            var branches = await context.Branches
                .Select(b => new
                {
                    b.BranchId,
                    b.BranchName,
                    b.Address,
                    b.Phone,
                    b.Email
                })
                .ToListAsync();

            var roles = await context.Roles
                .Select(r => new
                {
                    r.RoleId,
                    r.RoleName
                })
                .ToListAsync();

            return Ok(new
            {
                Users = users,
                Branches = branches,
                Roles = roles
            });
        }



        [HttpPost("Login")]
        public async Task<ActionResult<object>> Login(string txtusername, string txtpassword, int ddltype)
        {

            var data = await context.Users

                .Where(u =>
                    EF.Functions.Collate(u.Username, "Latin1_General_CS_AS") == txtusername &&
                    EF.Functions.Collate(u.PasswordHash, "Latin1_General_CS_AS") == txtpassword &&
                    u.RoleId == ddltype)
                .FirstOrDefaultAsync();

            if (data != null)
            {
                return Ok(new
                {
                    Success = true,
                    UserId = data.UserId,
                    BranchId = data.BranchId
                });
            }
            else
            {
                return Ok(new
                {
                    Success = false,
                    Message = "Invalid credentials"
                });
            }
        }



        [HttpPost("User")]
        public ActionResult User([FromBody] User request, [FromQuery] string Action)
        {
            //if (string.IsNullOrWhiteSpace(action))
            //    return BadRequest("Action parameter is required.");

            //action = action.Trim().ToLower();

            if (Action == "submit")
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                    return BadRequest("Username is required.");

                var existingUser = context.Users.FirstOrDefault(u => u.Username.ToLower() == request.Username.ToLower());
                if (existingUser != null)
                    return Conflict($"Username '{request.Username}' already exists.");

                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = request.PasswordHash,
                    Email = request.Email,
                    RoleId = request.RoleId,
                    BranchId = request.BranchId,
                    IsActive = request.IsActive
                };

                context.Users.Add(user);
                int result = context.SaveChanges();

                return result == 1
                    ? Ok(new { success = true, message = "User added successfully." })
                    : StatusCode(500, "An error occurred while saving the user.");
            }
            else if (Action == "update")
            {
                if (request.UserId <= 0)
                    return BadRequest("Valid UserId is required for update.");

                var userToUpdate = context.Users.FirstOrDefault(u => u.UserId == request.UserId);
                if (userToUpdate == null)
                    return NotFound("User not found for update.");

                userToUpdate.Username = request.Username;
                userToUpdate.PasswordHash = request.PasswordHash;
                userToUpdate.Email = request.Email;
                userToUpdate.RoleId = request.RoleId;
                userToUpdate.BranchId = request.BranchId;
                userToUpdate.IsActive = request.IsActive;

                context.SaveChanges();

                return Ok(new { success = true, message = "User updated successfully." });
            }
            else if (Action == "delete")
            {
                if (request.UserId <= 0)
                    return BadRequest("Valid UserId is required for deletion.");

                var userToDelete = context.Users.FirstOrDefault(u => u.UserId == request.UserId);
                if (userToDelete == null)
                    return NotFound("User not found for deletion.");

                context.Users.Remove(userToDelete);
                context.SaveChanges();

                return Ok(new { success = true, message = "User deleted successfully." });
            }

            return BadRequest("Invalid action. Allowed values: Submit, Update, Delete.");
        }




        [HttpGet("getuserdata")]
        public async Task<IActionResult> GetUserData()
        {
            var data = context.Users
                .Select(e => new
                {
                    UserId = e.UserId,
                    Username = e.Username,
                    PasswordHash = e.PasswordHash,
                    Email = e.Email,
                    RoleId = e.RoleId,
                    BranchId = e.BranchId,
                    IsActive = e.IsActive

                }).ToList();


            return Ok(data);
        }


        public static bool IsIdInUse(DbContext context, string idPropertyName, object idValue, Type excludeEntityType)
        {
            var dbSets = context.GetType()
                                .GetProperties()
                                .Where(p => p.PropertyType.IsGenericType &&
                                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            foreach (var dbSetProp in dbSets)
            {
                var entityType = dbSetProp.PropertyType.GetGenericArguments().First();

                // Skip the entity/table we want to exclude (like Branches)
                if (entityType == excludeEntityType)
                    continue;

                var property = entityType.GetProperty(idPropertyName);
                if (property == null)
                    continue;

                var dbSet = dbSetProp.GetValue(context);
                var queryable = dbSet as IQueryable;

                var parameter = Expression.Parameter(entityType, "x");
                var propertyAccess = Expression.Property(parameter, property);
                var constant = Expression.Constant(idValue);
                var equality = Expression.Equal(propertyAccess, Expression.Convert(constant, property.PropertyType));
                var lambda = Expression.Lambda(equality, parameter);

                var anyMethod = typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(entityType);

                var result = (bool)anyMethod.Invoke(null, new object[] { queryable, lambda });
                if (result)
                    return true;
            }

            return false;
        }




        [HttpPost("Branches")]
        public IActionResult Branches([FromBody] Branch request, [FromQuery] string Action)
        {
            //if (string.IsNullOrWhiteSpace(action))
            //    return BadRequest("Action parameter is required.");

            //action = action.Trim().ToLower();

            if (Action == "submit")
            {
                if (string.IsNullOrWhiteSpace(request.BranchName))
                    return BadRequest("BranchName is required.");

                var existingBranch = context.Branches
                    .FirstOrDefault(b => b.BranchName.ToLower() == request.BranchName.ToLower());

                if (existingBranch != null)
                    return Conflict($"Branch '{request.BranchName}' already exists.");

                var newBranch = new Branch
                {
                    BranchName = request.BranchName,
                    Address = request.Address,
                    Phone = request.Phone,
                    Email = request.Email
                };

                context.Branches.Add(newBranch);
                context.SaveChanges();

                return Ok(new { success = true, message = "Branch added successfully." });
            }

            if (Action == "update")
            {
                if (request.BranchId <= 0)
                    return BadRequest("Valid BranchId is required for update.");

                var branchToUpdate = context.Branches.FirstOrDefault(b => b.BranchId == request.BranchId);
                if (branchToUpdate == null)
                    return NotFound("Branch not found for update.");

                branchToUpdate.BranchName = request.BranchName;
                branchToUpdate.Address = request.Address;
                branchToUpdate.Phone = request.Phone;
                branchToUpdate.Email = request.Email;

                context.SaveChanges();

                return Ok(new { success = true, message = "Branch updated successfully." });
            }

            if (Action == "delete")
            {
                if (request.BranchId <= 0)
                    return BadRequest("Valid BranchId is required for deletion.");

                var branchToDelete = context.Branches.FirstOrDefault(b => b.BranchId == request.BranchId);
                if (branchToDelete == null)
                    return NotFound("Branch not found for deletion.");

                // Check if any users are linked to this branch
                //bool isBranchInUse = context.Users.Any(u => u.BranchId == request.BranchId);
                //if (isBranchInUse)
                //    return Conflict("Cannot delete this branch because it is linked to one or more users.");

                if (IsIdInUse(context, "BranchId", request.BranchId, typeof(Branch)))
                {
                    return Conflict("Cannot delete this Branch as it is assigned in one or more other tables.");
                }


                context.Branches.Remove(branchToDelete);
                context.SaveChanges();

                return Ok(new { success = true, message = "Branch deleted successfully." });
            }


            return BadRequest("Invalid action. Allowed values: Submit, Update, Delete.");
        }



        [HttpGet("getbranchdata")]
        public async Task<IActionResult> GetBranchData()
        {
            var data = context.Branches
                .Select(e => new
                {
                    BranchId = e.BranchId,
                    BranchName = e.BranchName,
                    Phone = e.Phone,
                    Address = e.Address,
                    Email = e.Email,

                }).ToList();


            return Ok(data);
        }




        [HttpPost("Roles")]
        public ActionResult Roles([FromBody] Role request, [FromQuery] string action)
        {
            if (string.IsNullOrWhiteSpace(action))
                return BadRequest("Action parameter is required.");

            action = action.Trim().ToLower();

            if (action == "submit")
            {
                if (string.IsNullOrWhiteSpace(request.RoleName))
                    return BadRequest("RoleName is required.");

                var existingRole = context.Roles.FirstOrDefault(r => r.RoleName.ToLower() == request.RoleName.ToLower());
                if (existingRole != null)
                    return Conflict($"RoleName '{request.RoleName}' already exists.");

                var role = new Role
                {
                    RoleName = request.RoleName
                };

                context.Roles.Add(role);
                int result = context.SaveChanges();

                return result == 1
                    ? Ok(new { success = true, message = "Role added successfully." })
                    : StatusCode(500, "An error occurred while saving the role.");
            }
            else if (action == "update")
            {
                if (request.RoleId <= 0)
                    return BadRequest("Valid RoleId is required for update.");

                var roleToUpdate = context.Roles.FirstOrDefault(r => r.RoleId == request.RoleId);
                if (roleToUpdate == null)
                    return NotFound("Role not found for update.");

                roleToUpdate.RoleName = request.RoleName;
                context.SaveChanges();

                return Ok(new { success = true, message = "Role updated successfully." });
            }
            else if (action == "delete")
            {
                if (request.RoleId <= 0)
                    return BadRequest("Valid RoleId is required for deletion.");

                var roleToDelete = context.Roles.FirstOrDefault(r => r.RoleId == request.RoleId);
                if (roleToDelete == null)
                    return NotFound("Role not found for deletion.");

                if (IsIdInUse(context, "RoleId", request.RoleId, typeof(Role)))
                    return Conflict("Cannot delete this role as it is assigned in one or more tables.");

                context.Roles.Remove(roleToDelete);
                context.SaveChanges();

                return Ok(new { success = true, message = "Role deleted successfully." });
            }

            return BadRequest("Invalid action. Allowed values: Submit, Update, Delete.");
        }




        [HttpGet("getrolesdata")]
        public async Task<IActionResult> GetRolesData()
        {
            var data = context.Roles
                .Select(e => new
                {
                    RoleId = e.RoleId,
                    RoleName = e.RoleName
                }).ToList();

            return Ok(data);
        }



    }
}
