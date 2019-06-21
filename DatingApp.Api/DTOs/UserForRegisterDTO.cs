using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Api.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="La password es muy corta")]
        public string Password { get; set; }
    }
}