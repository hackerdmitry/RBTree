using System;

namespace RBTree
{
    public class ConsolePaintModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        
        public char[,] CharMap { get; set; }
        public ConsoleColor[,] Foreground { get; set; }
        public ConsoleColor[,] Background { get; set; }
    }
}
