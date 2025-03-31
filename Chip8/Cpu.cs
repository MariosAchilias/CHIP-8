using System;
using System.Collections;
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
		nnn = (ushort)(instr & (ushort)0XFFF);
	}
	public ushort as_ushort()
	{
		return (ushort)(((ushort)opcode << 12) ^ nnn);
	}

}

public class Cpu
{
	private byte[] _mem = new byte[4096];
	private ushort[] _stack = new ushort[16];
	private ushort _pc = 0x200;
	private byte _sp;
	private byte _dt;
	private byte _st;
	private ushort _I;
	private bool _vf;
	private byte[] _reg = new byte[16];

	private Random _rand = new Random();

	public bool[] display = new bool[64 * 32];
	public bool[] keypad = new bool[16];

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
			case 1:
				opcode1(instr);
				break;
            case 2:
                opcode2(instr);
                break;
            case 3:
                opcode3(instr);
                break;
            case 4:
                opcode4(instr);
                break;
            case 5:
                opcode5(instr);
                break;
            case 6:
				opcode6(instr);
				break;
            case 7:
                opcode7(instr);
                break;
            case 8:
                opcode8(instr);
                break;
            case 9:
                opcode9(instr);
                break;
            case 0xA:
				opcodeA(instr);
				break;
			case 0xB:
				opcodeB(instr);
				break;
            case 0xC:
                opcodeC(instr);
                break;
            case 0xD:
                opcodeD(instr);
                break;
            case 0xE:
				opcodeE(instr);
				break;
            case 0xF:
                opcodeF(instr);
                break;
            default:
				Debug.Write($"Unimplemented opcode {instr.as_ushort().ToString("x")}\n");
				break;
		}	
	}
	private Instruction fetch()
	{
		ushort next = (ushort)((((ushort)_mem[_pc]) << 8) | (ushort)_mem[_pc + 1]);
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
				_pc = _stack[_sp--];
				break;
			default:
				break;
		}
	}
	private void opcode1(Instruction instr)
	{
		_pc = instr.nnn;
	}
	private void opcode2(Instruction instr)
	{
		_stack[++_sp] = _pc;
		_pc = instr.nnn;
	}
	private void opcode3(Instruction instr)
	{
		if (_reg[instr.x] == instr.kk)
			_pc += 2;
	}
	private void opcode4(Instruction instr)
	{
		if (_reg[instr.x] != instr.kk)
			_pc += 2;
	}
	private void opcode5(Instruction instr)
	{
		if (_reg[instr.x] == _reg[instr.y])
			_pc += 2;
	}
	private void opcode6(Instruction instruction)
	{
		_reg[instruction.x] = instruction.kk;
	}
	private void opcode7(Instruction instr)
	{
		_reg[instr.x] += instr.kk;
	}
	private void opcode8(Instruction instr)
	{
		switch(instr.n)
		{
			case 0x0:
				_reg[instr.x] = _reg[instr.y];
				break;
            case 0x1:
				_reg[instr.x] |= _reg[instr.y];
                break;
            case 0x2:
				_reg[instr.x] &= _reg[instr.y];
                break;
            case 0x3:
				_reg[instr.x] ^= _reg[instr.y];
                break;
            case 0x4:
				_vf = _reg[instr.x] + _reg[instr.y] > 255;
				_reg[instr.x] += _reg[instr.y];
                break;
            case 0x5:
				_vf = _reg[instr.x] > _reg[instr.y];
				_reg[instr.x] -= _reg[instr.y];
                break;
            case 0x6:
				_vf = (_reg[instr.x] & 0x1) != 0;
				_reg[instr.x] /= 2;
                break;
            case 0x7:
				_vf = _reg[instr.y] > _reg[instr.y];
				_reg[instr.x] = (byte) (_reg[instr.y] - _reg[instr.x]);
                break;
            case 0xE:
				_vf = (_reg[instr.x] & 0x8000) != 0;
				_reg[instr.x] *= 2;
                break;

        }
    }
	private void opcode9(Instruction instr)
	{
		if (_reg[instr.x] != _reg[instr.y])
			_pc += 2;
	}
	private void opcodeA(Instruction instruction)
	{
		_I = instruction.nnn;
	}
	private void opcodeB(Instruction instr)
	{
		_I = (ushort)(instr.nnn + _reg[0]);
	}
	private void opcodeC(Instruction instr)
	{
		_reg[instr.x] = (byte)(((byte)_rand.Next(255)) & instr.kk);
	}
	private void opcodeD(Instruction instruction)
	{

		for (int i = 0; i < instruction.n; i++)
		{
            int disp_idx = _reg[instruction.x] + (i + _reg[instruction.y]) * 64;
			BitArray sprite_byte = new BitArray(new int[] { _mem[_I + i] });
            for (int j = 0; j < 8; j++)
			{
				display[disp_idx + j] = sprite_byte[7 - j];
			}
		}		
	}
	private void opcodeE(Instruction instr)
	{
		switch (instr.kk)
		{
			case 0xA1:
				if (!keypad[_reg[instr.x]])
					_pc += 2;
				break;
			case 0x9E:
				if (keypad[_reg[instr.x]])
					_pc += 2;
				break;
		}
	}
	private void opcodeF(Instruction instr)
	{
		switch (instr.kk)
		{
			case 0x07:
				_reg[instr.x] = _dt;
				break;
            case 0x0A:
				// todo
                break;
            case 0x15:
				_dt = _reg[instr.x];
                break;
            case 0x18:
				_st = _reg[instr.x];
                break;
            case 0x1E:
				_I += _reg[instr.x];
                break;
            case 0x29:
				_I = (ushort) (_reg[instr.x] * 5);
                break;
            case 0x33:
                break;
            case 0x55:
				for (int i = 0; i < instr.x; i++)
					_mem[_I + i] = _reg[i];
                break;
            case 0x65:
				for (int i = 0; i < instr.x; i++)
					_reg[i] = _mem[_I + i];
                break;
        }
	}
}
