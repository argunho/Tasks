using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks.Models;

namespace Tasks.Interfaces
{
    public interface IHelpers
    {
        string TokenGenerator(Users user);
    }
}
