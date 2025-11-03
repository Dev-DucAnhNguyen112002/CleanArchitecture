using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitectureTest.Domain.Configs
{
    public class ConnectionStringConfig
    {
        public required string MasterDb { get; set; }
    }
}
