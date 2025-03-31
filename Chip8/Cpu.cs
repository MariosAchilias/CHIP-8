using System;
using System.Security.Cryptography;

internal struct Instruction
{
	byte opcode;
	byte kk;
	byte x;
	byte y;
	byte n;
	ushort nnn;
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
public class Cpu
{
	private byte[] _mem = new byte[4096];
	private ushort[] _stack;
	private ushort _pc = 0x200;
	private byte _sp;
	public bool[] _display = new bool[64 * 32];

	public Cpu()
	{

	}
	public void load(string rom)
	{

	}
	public void cycle()
	{

	}
	private Instruction fetch()
	{
		ushort next = (ushort)((((ushort)_mem[_pc]) << 8) ^ (ushort)_mem[_pc + 1]);
		_pc += 2;
		return new Instruction(next);

	}
	private void run(Instruction instruction)
	{

	}
}
