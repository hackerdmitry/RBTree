using System;

namespace RBTree
{
    static class Program
    {
        private static readonly VisualizerRBTree Visualizer = new VisualizerRBTree();
        
        public static void Main()
        {
            var rbTree = new RBTree<int>(13);
            rbTree.Add(1);
            rbTree.Add(6);
            rbTree.Add(8);
            rbTree.Add(11);
            rbTree.Add(15);
            rbTree.Add(17);
            rbTree.Add(22);
            rbTree.Add(25);
            rbTree.Add(27);
            WriteRBTree(rbTree);
            
            Console.ResetColor();
            Console.Write("Нажмите любую кнопку для того, чтобы продолжить...");
            Console.ReadKey();
        }
        
        private static void WriteRBTree<T>(RBTree<T> rbTree) where T : IComparable<T>
        {
            var paintModel = Visualizer.CreatePaintModel(rbTree);
            
            var top = Console.CursorTop;
            var left = Console.CursorLeft;
            
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
