using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryArchiLog.Models
{
    public class FilterParameter
    {
        public String? category { get; set; }
       
        public String Category
        {
            get
            {
                return category;
            }
            set
            {
                category = (value = category);
            }
        }
    }
}
