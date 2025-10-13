﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace jjodel_persistence.Models.Entity {

    public class ApplicationUser : IdentityUser {

        public string? _Id { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        [Required]
        public string Affiliation { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        public DateTime DeletionDate { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public DateTime? LastLogin { get; set; }

        [Required]
        public bool NewsletterEnabled { get; set; }
        public DateTime NewsletterEnableDate { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        [NotMapped]
        public string FullName { get { return this.Surname + " " + this.Name; } }

        public ICollection<ApplicationUserRole> ApplicationRoles { get; set; }

        [InverseProperty("Author")]
        public ICollection<Project> Author { get; set; } // author of..

        //[InverseProperty("AuthorId")]
        public ICollection<Project> Collaborators { get; set; } // collaborate to many projects

    }
}
