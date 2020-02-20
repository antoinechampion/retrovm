using System;
using System.Collections.Generic;
using System.Text;

namespace RetroVm.Core.Exceptions
{
    class ConfigurationDeserializationException: Exception
    {
        public ConfigurationDeserializationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
