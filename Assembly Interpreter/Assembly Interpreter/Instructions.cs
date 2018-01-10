using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembly_Interpreter
{
    [Flags]
    enum Instruct : byte { MOV, SWP, SAV, NOP, ADD, SUB, NEG, JMP, JEZ, JNZ, JGZ, JLZ, JRO }; // bytes 0-12
    
    public struct Instruction
    {
        public byte FUNC { get; set; }
        public object SRC { get; set; }
        public byte DEST { get; set; }

        public Instruction(byte opCode, object source, byte destination = (byte)Registers.ACC)
        {
            FUNC = opCode;
            SRC = source;
            DEST = destination;
        }
    }
}
