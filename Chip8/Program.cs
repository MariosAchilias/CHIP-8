using System;
System.Diagnostics.Debug.WriteLine(args[0]);
using var game = new Chip8.Chip8(args[0]);
game.Run();