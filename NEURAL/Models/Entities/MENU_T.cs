using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NEURAL.Models.Entities
{
    public class MENU_T
    {
        public string MENU_ID { get; set; }
        public string MENU { get; set; }
        public string PARENT { get; set; }
        public string PARENT_ID { get; set; }
        public string URL { get; set; }
        public string RESTRICTION_TYPE { get; set; }
        public string TYPE { get; set; }

    }
}
