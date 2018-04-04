using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Storage.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<IdentityUserRole<Guid>> Roles { get; set;} =  new List<IdentityUserRole<Guid>>();
        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; } = new List<IdentityUserClaim<Guid>>();
        public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; } = new List<IdentityUserLogin<Guid>>();
    }
}