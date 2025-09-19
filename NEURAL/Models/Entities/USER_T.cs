using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NEURAL.Models.Entities
{
    public class USER_T
    {
        public string USER_ID { get; set; }
        public string NRP { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public int JOBSITE_ID { get; set; }
        public string JOBSITE { get; set; }
        public int DIVISION_ID { get; set; }
        public string DIVISION_NAME { get; set; }
        public int GROUP_ID { get; set; }
        public string GROUP { get; set; }
        public DateTime CREATED_AT { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime UPDATED_AT { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime DELETED_AT { get; set; }
        public string DELETED_BY { get; set; }

    }
}
