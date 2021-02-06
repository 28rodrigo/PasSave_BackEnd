using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PasSave_BackEnd.Auxiliar_Classes;
using PasSave_BackEnd.Data;
using PasSave_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PasSave_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly PasSave_BackEndContext _context;
        private readonly string _Key;
        private readonly string _Email;
        private readonly string _Password;
        public LoginController(PasSave_BackEndContext context,IConfiguration config)
        {
            _config = config;
            _context = context;
            _Key = config.GetSection("Key").Value;
            _Email = config.GetSection("EmailCredentials").GetSection("Email").Value;
            _Password = config.GetSection("EmailCredentials").GetSection("Password").Value;

        }
        [HttpPost]
        [AllowAnonymous]
        [Route("/api/register")]
        public IActionResult Register([FromBody] UserDTO user)
        {
            if(_context.User.Any(x=>x.Username==user.Username))
            {
                return Conflict();
            }
            if(_context.User.Any(x=>x.Email==user.Email))
            {
                return Conflict();
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Pass);
            User new_user = new User() { Username = user.Username, Email = user.Email, Pass = passwordHash };
            new_user=_context.User.Add(new_user).Entity;
            _context.SaveChangesAsync();
            //mandar email
            try
            {
                string encuser = AESEncryption.Encrypt(_Key, new_user.Username);

                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(_Email);
                mail.To.Add(new_user.Email);
                mail.Subject = "Email de Confirmação";
                mail.Body = "Olá " + new_user.Username + "\nConfirmar Conta da App PasSave:\n Clique neste link:\n https://localhost:44346/Clientes/Confirmar?userId=" + encuser+"\n\n PasSave Team";

                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(_Email, _Password);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
                
                return Ok(new {Username=new_user.Username,Email=new_user.Email });
            }
            catch (Exception e)
            {
                
                return Content("Email not Sended : \n"+e.Message);
            }
            


        }
        [AllowAnonymous]
        [HttpPost]
        [Route("/")]
        public async Task<IActionResult> Login([FromBody] LoginDTO u)
        {
            

            IActionResult response = Unauthorized();
            var user =await AuthenticateUser(u);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }
        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddDays(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<User> AuthenticateUser(LoginDTO u)
        {
            var user = await _context.User.FindAsync(u.Username);
            if (user == null)
            {
                return null;
            }
            if (!BCrypt.Net.BCrypt.Verify(u.Pass, user.Pass))
            {
                return null;
            }

            return user;
        }
        
        //recuperar
    }
}
