using System;

namespace RBTree
{
    static class Program
    {
        private static readonly VisualizerRBTree Visualizer = new VisualizerRBTree();
        
        public static void Main()
        {
            WriteRandomRBTree();
            
            Console.ResetColor();
            Console.Write("Нажмите любую кнопку для того, чтобы продолжить...");
            Console.ReadKey();
        }

        private static void WriteRandomRBTree()
        {
            const int minCount = 20;
            const int maxCount = 30;

            const int minNumber = -50;
            const int maxNumber = 50;
            
            var rand = new Random();
            var count = rand.Next(minCount, maxCount + 1);
            var rbTree = new RBTree<int>();

            for (var i = 0; i < count; i++)
            {
                rbTree.Add(rand.Next(minNumber, maxNumber + 1));
            }
            
            WriteRBTree(rbTree);
            Console.WriteLine(rbTree.IsTrueAllProperties());
        }
        
        private static void WriteRBTree<T>(RBTree<T> rbTree) where T : IComparable<T>
        {
            var paintModel = Visualizer.CreatePaintModel(rbTree);
            
            var top = Console.CursorTop;
            var left = Console.CursorLeft;

            Console.BufferHeight = Math.Max(paintModel.Height, Console.BufferHeight);
            Console.BufferWidth = Math.Max(paintModel.Width, Console.BufferWidth);

            Console.WindowWidth = paintModel.Width;
            
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
