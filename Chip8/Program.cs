using System;
System.Diagnostics.Debug.WriteLine(args[0]);
using var game = new Chip8.Game1(args[0]);
game.Run();