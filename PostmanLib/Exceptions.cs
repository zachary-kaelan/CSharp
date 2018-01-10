using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostmanLib
{
    public class RegexFailedException : Exception
    {
        public RegexFailedException() : 
            base("A Regex has failed.") {}

        public RegexFailedException(string rgxName) : 
            base("The " + rgxName +  " Regex has failed.") {}

    }

    public class UnknownPDFTypeException : Exception
    {
        public UnknownPDFTypeException() :
            base("Encountered a PDF of an unknown type.") {}

        public UnknownPDFTypeException(string pdfName) :
            base("The type of '" + pdfName + "' could not be determined.") {}
    }
}
