using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phases.UmbracoGenie.Services.Interfaces
{
    public interface IKernelFactory
    {
        Task<Kernel> CreateKernelAsync();
    }
}
