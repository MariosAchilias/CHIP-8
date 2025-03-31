using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

internal struct Instruction
{
	public byte opcode;
	public byte kk;
	public byte x;
	public byte y;
	public byte n;
	public ushort nnn;

	public Instruction(ushort instr)
	{
		opcode = (byte)(instr >> 12);
		kk = (byte)(instr & (ushort)0xFF);
		x = (byte)((instr & (ushort)0xF00) >> 8);
		y = (byte)((instr & (ushort)0xF0) >> 4);
		n = (byte)(instr & (ushort)0xF);
		nnn = (byte)(instr & (ushort)0XFFF);
	}
	public ushort as_ushort()
	{
		return (ushort)(((ushort)opcode << 12) ^ nnn);
	}

}

// 00E0 6xnn Annn, Dxyn
public class Cpu
{
	private byte[] _mem = new byte[4096];
	private ushort[] _stack;
	private ushort _pc = 0x200;
	private byte _sp;
	private ushort _I;
	private byte[] _reg = new byte[16];
	public bool[] display = new bool[64 * 32];

	public Cpu()
	{

	}
	public void load(string rom)
	{
		byte[] rom_file = System.IO.File.ReadAllBytes(rom);
		uint start = 0x200;
		for (int i = 0; i < rom_file.Length; i++)
			_mem[start + i] = rom_file[i];

	}
	public void cycle()
	{
		Instruction instr = fetch();
		switch (instr.opcode)
		{
			case 0:
				opcode0(instr);
				break;
			case 6:
				opcode6(instr);
				break;
			case 0xA:
				opcodeA(instr);
				break;
			case 0xD:
				opcodeD(instr);
				break;
			default:
				Debug.Write($"Unimplemented opcode {instr.as_ushort().ToString("x")}\n");
				break;
		}	
	}
	private Instruction fetch()
	{
		ushort next = (ushort)((((ushort)_mem[_pc]) << 8) ^ (ushort)_mem[_pc + 1]);
		_pc += 2;
		return new Instruction(next);

	}
	private void opcode0(Instruction instruction)
	{
		switch (instruction.nnn)
		{
			case 0xE0:
				display = Enumerable.Repeat(false, 64 * 32).ToArray();
				break;
			case 0xEE:
				break;
			default:
				break;
		}
	}
	private void opcode6(Instruction instruction)
	{
		_reg[instruction.x] = instruction.kk;
	}
	private void opcodeA(Instruction instruction)
	{
		_I = instruction.nnn;
	}
	private void opcodeD(Instruction instruction)
	{

		for (int i = 0; i < instruction.n; i++)
		{
            int disp_line_start = _reg[instruction.x] + (i + _reg[instruction.y]) * 64;
			uint sprite_line = _mem[_I + i];
            for (int j = 0; j < 8; j++)
			{
				display[disp_line_start + j] = ((sprite_line >> j) & 0x1) != 0;
			}
		}		
	}
}
