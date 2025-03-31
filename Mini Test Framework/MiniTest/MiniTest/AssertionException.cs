using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTest
{
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }
}
