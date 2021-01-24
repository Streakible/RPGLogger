using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCTest.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string htmlClass { get; set; }
        [DataType(DataType.Date)]
        public DateTime date { get; set; }
        public string speaker { get; set; }
        public string message { get; set; }
        public Log log { get; set; }
    }
}
