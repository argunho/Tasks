using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tasks.Models
{
    public class Tags
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Done { get; set; }
        public int TaskId { get; set; }
    }
}
