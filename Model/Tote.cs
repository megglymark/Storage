using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Storage.Models
{
    public class Tote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Location { get; set; }
        public Guid OwnerId { get; set; }
        public Owner Owner { get; set; }
        public List<Bricabrac> Bricabracs { get; set; }
    }
}