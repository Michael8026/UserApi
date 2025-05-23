﻿using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Account
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; }

        [Required]
        public int Age { get; set; }
    }
}
