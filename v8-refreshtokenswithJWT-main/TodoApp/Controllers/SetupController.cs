using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoApp.Data;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {

        private readonly ApiDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SetupController> _logger;

        public SetupController(ApiDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<SetupController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {

            var roleExist = await _roleManager.RoleExistsAsync(name);

            if (!roleExist)
            {

                var roleResult = await _roleManager.CreateAsync(new IdentityRole(name));

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"The Role {name} has been added successfully");

                    return Ok(new
                    {
                        result = $"The Role {name} has been added succesfully"
                    });
                }
                else
                {
                    _logger.LogInformation($"The Role {name} has NOT been added successfully");

                    return BadRequest(new
                    {
                        error = $"The Role {name} has NOT been added succesfully"
                    });
                }

            }

            return BadRequest(new { error = "Role already exist" });
        }



        [HttpGet]
        [Route("getallusers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            return Ok(users);
        }




        [HttpPost]
        [Route("addusertorole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"The User with the {email} does not exist");

                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                _logger.LogInformation($"The Role with the {roleName} does not exist");

                return BadRequest(new
                {
                    error = "Role does not exist"
                });
            }


            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return Ok(new
                {
                    result = "Success,user has been added to the role"
                });
            }
            else
            {
                _logger.LogInformation($"The User was not abel to be added to the role");

                return BadRequest(new
                {
                    error = "The User was not abel to be added to the role"
                });
            }

        }




        [HttpPost]
        [Route("addclaimstouser")]
        public async Task<IActionResult> AddClaimsToUser(string email, string claimName, string claimValue)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"The User with the {email} does not exist");

                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }


            var userClaim = new Claim(claimName, claimValue);

            var result = await _userManager.AddClaimAsync(user, userClaim);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    result = $"User {user.Email} has a claim {claimName} added to them"
                });
            }

            return BadRequest(new
            {
                error = $"Unable to add claim to the user {user.Email}"
            });
        }





        [HttpGet]
        [Route("getuserroles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"The User with the {email} does not exist");

                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }





        [HttpGet]
        [Route("getalluserclaims")]
        public async Task<IActionResult> GetAllUserClaims(string email)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"The User with the {email} does not exist");

                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }


            var userClaims = await _userManager.GetClaimsAsync(user);

            return Ok(userClaims);
        }





        [HttpPost]
        [Route("removeuserfromrole")]
        public async Task<IActionResult> RemoveUserFromRole(string email, string roleName)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"The User with the {email} does not exist");

                return BadRequest(new
                {
                    error = "User does not exist"
                });
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                _logger.LogInformation($"The Role with the {roleName} does not exist");

                return BadRequest(new
                {
                    error = "Role does not exist"
                });
            }


            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    result = $"User has been removed from role {roleName}"
                });
            }

            return BadRequest(new
            {
                error = $"Unable to remove User {email} from role {roleName}"
            });
        }





    }
}
