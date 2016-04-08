using ResilienceDemo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResilienceDemo.Client.Components
{
    public class ServiceOption
    {
        public ServiceOption(string name, ITestService service)
        {
            Name = name;
            Service = service;
        }

        public string Name { get; set; }

        public ITestService Service { get; set; }
    }
}
