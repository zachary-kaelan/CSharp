

using System;

namespace DNALib.GeneticAlgorithm
{
			public class ByteChromosome
		{
			private byte Length { get; set; }
			public Byte Value { get; private set; }
			private byte[] Bytes { get; set; }

			public ByteChromosome(Byte value, byte length)
			{
				Value = value;
				Bytes = BitConverter.GetBytes(value);
				Length = length;
			}
		}
			public class UInt16Chromosome
		{
			private byte Length { get; set; }
			public UInt16 Value { get; private set; }
			private byte[] Bytes { get; set; }

			public UInt16Chromosome(UInt16 value, byte length)
			{
				Value = value;
				Bytes = BitConverter.GetBytes(value);
				Length = length;
			}
		}
			public class UInt32Chromosome
		{
			private byte Length { get; set; }
			public UInt32 Value { get; private set; }
			private byte[] Bytes { get; set; }

			public UInt32Chromosome(UInt32 value, byte length)
			{
				Value = value;
				Bytes = BitConverter.GetBytes(value);
				Length = length;
			}
		}
			public class UInt64Chromosome
		{
			private byte Length { get; set; }
			public UInt64 Value { get; private set; }
			private byte[] Bytes { get; set; }

			public UInt64Chromosome(UInt64 value, byte length)
			{
				Value = value;
				Bytes = BitConverter.GetBytes(value);
				Length = length;
			}
		}
	}