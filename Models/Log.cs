using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCTest.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Campaign Campaign { get; set; }
        [DataType(DataType.Date)]
        public DateTime date { get; set; }
    }
}
