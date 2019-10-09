using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharacterEngineTest
{
    public class MessageHandler : IMessageHandler
    {
        public static readonly MessageHandler Instance = new MessageHandler();
    }
}
