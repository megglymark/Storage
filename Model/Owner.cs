using System;

namespace Storage.Models
{
    public class Owner
    {
        public Guid Id { get; set; }
        public Guid IdentityId { get; set; }
        public AppUser Identity { get; set; }

    }
}