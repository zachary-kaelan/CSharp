using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworksLib.Exceptions
{
    public class InvalidNetworkParametersException : Exception
    {
        public InvalidNetworkParametersException(string message) : base(message)
        {

        }
    }
}
