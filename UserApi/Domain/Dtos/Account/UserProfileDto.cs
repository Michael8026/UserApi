﻿namespace api.Dtos.Account
{
    public class UserProfileDto
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Gender { get; set; }

        public int Age { get; set; }
    }
}
