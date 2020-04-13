using System;
using System.Collections.Generic;
using System.Linq;

namespace RBTree
{
    static class Program
    {
        private static readonly VisualizerRBTree Visualizer = new VisualizerRBTree();
        
        public static void Main()
        {
            ConsoleKeyInfo key;
            do
            {
                Console.Clear();
                WriteRandomRBTree();

                Console.ResetColor();
                Console.Write("Нажмите Enter для того, чтобы повторить и любую другую, чтобы выйти...");
                key = Console.ReadKey();
            }
            while (key.Key == ConsoleKey.Enter);
        }

        private static void WriteRandomRBTree()
        {
            const int minCountAdd = 10;
            const int maxCountAdd = 20;
            
            const int minCountRemove = 5;
            const int maxCountRemove = 8;

            const int minNumber = -50;
            const int maxNumber = 50;
            
            var rand = new Random();
            var countAdd = rand.Next(minCountAdd, maxCountAdd + 1);
            var countRemove = rand.Next(minCountRemove, maxCountRemove + 1);
            var rbTree = new RBTree<int>();
            
            var numbers = new List<int>();
            
            var toAdd = new List<int>();
            var toRemove = new List<int>();

            for (var i = 0; i < countAdd; i++)
            {
                var number = rand.Next(minNumber, maxNumber + 1);
                toAdd.Add(number);
                numbers.Add(number);
                rbTree.Add(number);
            }

            for (var i = 0; i < countRemove; i++)
            {
                var number = numbers[rand.Next(0, numbers.Count)];
                toRemove.Add(number);
                numbers.Remove(number);
                rbTree.Remove(number);
            }
            
            WriteRBTree(rbTree);
            Console.WriteLine(rbTree.IsTrueAllProperties());
            Console.WriteLine();
            Console.WriteLine($"Add: {string.Join(",", toAdd)}");
            Console.WriteLine($"Remove: {string.Join(",", toRemove)}");
        }
        
        private static void WriteRBTree<T>(RBTree<T> rbTree) where T : IComparable<T>
        {
            var paintModel = Visualizer.CreatePaintModel(rbTree);
            
            var top = Console.CursorTop;
            var left = Console.CursorLeft;

            Console.BufferHeight = Math.Max(paintModel.Height, Console.BufferHeight);
            Console.BufferWidth = Math.Max(paintModel.Width, Console.BufferWidth);

            Console.WindowWidth = Math.Min(170, Math.Max(paintModel.Width, Console.WindowWidth));
            
            for (var y = 0; y < paintModel.Height; y++)
            {
                for (var x = 0; x < paintModel.Width; x++)
                {
                    Console.ForegroundColor = paintModel.Foreground[y, x];
                    Console.BackgroundColor = paintModel.Background[y, x];
                    Console.SetCursorPosition(left + x, top + y);
                    Console.Write(paintModel.CharMap[y, x]);
                }
            }
            
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
