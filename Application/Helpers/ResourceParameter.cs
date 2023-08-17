using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class ResourceParameter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Search { get; set; }
        public string Sort { get; set; }
        public string FilterBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CatalogueParameter : ResourceParameter
    { 
    }

    public class ShoppingCartParameter : ResourceParameter 
    {
    }
    public class ProductParameter : ResourceParameter
    {

        public Guid Category { get; set; }

        public IEnumerable<string> Date { get; set; } = new List<string>();
    }

    public class CategoryParameter : ResourceParameter 
    {
    }
}
