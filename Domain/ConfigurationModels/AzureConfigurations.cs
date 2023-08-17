using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ConfigurationModels
{
    public class AzureConfigurations
    {

        public string Section { get; set; } = "Azure";
        public string BlobContainerName { get; set; }
        public string BlobConnectionString { get; set; }
    }
}
