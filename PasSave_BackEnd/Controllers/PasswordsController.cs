using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PasSave_BackEnd.Auxiliar_Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PasSave_BackEnd.Data;
using PasSave_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace PasSave_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PasswordsController : ControllerBase
    {
        private readonly PasSave_BackEndContext _context;
        private readonly string _Key;
        public PasswordsController(PasSave_BackEndContext context,IConfiguration configuration)
        {
            _context = context;
            _Key = configuration.GetSection("Key").Value;
        }

        // GET: api/Passwords
        [HttpGet]

        public async Task<ActionResult<IEnumerable<PasswordDTO>>> GetPassword()
        {
            var currentUser = HttpContext.User;
            string user = null;
            if (currentUser.HasClaim(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
            {
               user= currentUser.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            else
            {
                return Unauthorized();
            }
            return await _context.Password.Where(x => x.UserId == user).Select(x => DecryptPassword(x,_Key)).Select(x => ItemToDTO(x)).ToListAsync();

        }

        // GET: api/Passwords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PasswordDTO>> GetPassword(int id)
        {
            var currentUser = HttpContext.User;
            var password = await _context.Password.FindAsync(id);

            if (password == null)
            {
                return NotFound();
            }
            string user = null;
            if (currentUser.HasClaim(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
            {
                user = currentUser.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            else
            {
                return Unauthorized();
            }
            if(password.UserId!=user)
            {
                return NotFound();
            }
            password = DecryptPassword(password,_Key);
            return ItemToDTO(password);
        }

        // PUT: api/Passwords/5
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPassword(int id, PasswordDTO password)
        {
            var currentUser = HttpContext.User;
            if (password == null)
                return Content("Body Error!");
            var realpass = await _context.Password.FindAsync(id);
            if(realpass==null)
            {
                return NotFound();
            }
            string user = null;
            if (currentUser.HasClaim(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
            {
                user = currentUser.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            else
            {
                return Unauthorized();
            }
            if(user!=realpass.UserId)
            {
                return Unauthorized();
            }
            if (id != password.Id)
            {
                return BadRequest();
            }
            realpass.Nome = password.Nome;
            realpass.Pass = password.Pass;
            realpass.Url = password.Url;
            realpass.Username = password.Username;
            realpass = EncryptPassword(realpass,_Key);
            _context.Entry(realpass).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PasswordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Passwords
        
        [HttpPost]
        public async Task<ActionResult<Password>> PostPassword(PasswordDTO passwordDTO)
        {
            var currentUser = HttpContext.User;
            string user = null;
            if (currentUser.HasClaim(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
            {
                user = currentUser.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            else
            {
                return Unauthorized();
            }

            if (passwordDTO == null)
                return Content("Body Error!");
            var password = new Password()
            {
                DateCreated = DateTime.Now,
                Id = passwordDTO.Id,
                Nome = passwordDTO.Nome,
                Url = passwordDTO.Url,
                Username = passwordDTO.Nome,
                Pass = passwordDTO.Pass,
                UserId=user

            };
            password = EncryptPassword(password,_Key);
            _context.Password.Add(password);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPassword", new { id = password.Id }, password);
        }

        // DELETE: api/Passwords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassword(int id)
        {
            var currentUser = HttpContext.User;
            string user = null;
            if (currentUser.HasClaim(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))
            {
                user = currentUser.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            }
            else
            {
                return Unauthorized();
            }
            var password = await _context.Password.FindAsync(id);
            if (password == null)
            {
                return NotFound();
            }
            if(password.UserId!=user)
            {
                return Unauthorized();
            }
            _context.Password.Remove(password);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PasswordExists(int id)
        {
            return _context.Password.Any(e => e.Id == id);
        }
        private static PasswordDTO ItemToDTO(Password password) =>
            new PasswordDTO
            {
                Id = password.Id,
                Nome = password.Nome,
                Url=password.Url,
                Username=password.Username,
                Pass=password.Pass,
                
            
            };
        private static Password EncryptPassword(Password password, string _Key) =>
           new Password
           {
               Id=password.Id,
               Nome = AESEncryption.Encrypt(_Key, password.Nome),
               Url = AESEncryption.Encrypt(_Key, password.Url),
               Username = AESEncryption.Encrypt(_Key, password.Username),
               Pass= AESEncryption.Encrypt(_Key, password.Pass),
               DateCreated=DateTime.Now,
               UserId = password.UserId

           };
        private static Password DecryptPassword(Password password, string _Key) =>
          new Password
          {
              Id = password.Id,
              Nome = AESEncryption.Decrypt(_Key, password.Nome),
              Url = AESEncryption.Decrypt(_Key, password.Url),
              Username = AESEncryption.Decrypt(_Key, password.Username),
              Pass = AESEncryption.Decrypt(_Key, password.Pass),
              DateCreated = DateTime.Now,
              UserId = password.UserId
          };
        private static PasswordDTO EncryptPasswordDTO(PasswordDTO password, string _Key) =>
           new PasswordDTO
           {
               Id = password.Id,
               Nome = AESEncryption.Encrypt(_Key, password.Nome),
               Url = AESEncryption.Encrypt(_Key, password.Url),
               Username = AESEncryption.Encrypt(_Key, password.Username),
               Pass = AESEncryption.Encrypt(_Key, password.Pass),
               
           };
        private static PasswordDTO DecryptPasswordDTO(PasswordDTO password, string _Key) =>
          new PasswordDTO
          {
              Id = password.Id,
              Nome = AESEncryption.Decrypt(_Key, password.Nome),
              Url = AESEncryption.Decrypt(_Key, password.Url),
              Username = AESEncryption.Decrypt(_Key, password.Username),
              Pass = AESEncryption.Decrypt(_Key, password.Pass),
              
          };
    }
}
