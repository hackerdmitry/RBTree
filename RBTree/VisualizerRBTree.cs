using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBTree
{
    public class VisualizerRBTree
    {
        private const ConsoleColor DefaultBackground = ConsoleColor.White;
        private const ConsoleColor NodeBackground = ConsoleColor.White;
        private const ConsoleColor BlackNode = ConsoleColor.Black;
        private const ConsoleColor RedNode = ConsoleColor.Red;
        private const ConsoleColor ConnectionColor = ConsoleColor.DarkRed;
        
        private const int FrameToValueIntervalX = 1; // промежуток между рамкой и числом по оси OX
        private const int FrameToValueIntervalY = 0; // промежуток между рамкой и числом по оси OY
        
        private const int NodeToNodeIntervalX = 1; // промежуток между двумя нодами по оси OX
        private const int NodeToNodeIntervalY = -1; // промежуток между двумя нодами по оси OY
        
        public ConsolePaintModel CreatePaintModel<T>(RBTree<T> rbTree) where T : IComparable<T>
        {
            var map = CreateMapRBTree(rbTree).ToArray();
            return PaintMap(map);
        }

        private ConsolePaintModel PaintMap<T>((Node<T>, int)[] map) where T : IComparable<T>
        {
            var frames = new Dictionary<Node<T>, Frame<T>>();
            
            var symbolsLength = map.Sum(x => x.Item1.Value.ToString().Length);
            var maxHeight = map.Max(x => x.Item2);

            var width = map.Length * (FrameToValueIntervalX + 1) * 2 +
                        (map.Length - 1) * NodeToNodeIntervalX +
                        symbolsLength;
            var height = (maxHeight + 1) * (3 + FrameToValueIntervalY * 2) + 
                         maxHeight * NodeToNodeIntervalY;

            var initial = CreateInitial(width, height);
            var charMap = initial.CharMap;
            var background = initial.Background;
            var foreground = initial.Foreground;
            var curX = 0;
            foreach (var cell in map)
            {
                var curY = cell.Item2 * (3 + FrameToValueIntervalY * 2 + NodeToNodeIntervalY);
                var node = cell.Item1;
                var frame = PaintFrame(curX, curY, node, charMap, out var endX);
                PaintFrameColor(frame, foreground, background);
                frames.Add(node, frame);
                curX = endX + FrameToValueIntervalX + 1;
            }

            foreach (var frame1 in frames.Values)
            {
                var node = frame1.Node;
                if (node.Parent != null && frames.ContainsKey(node.Parent))
                {
                    var frame2 = frames[node.Parent];
                    
                    var x1 = frame1.CenterX;
                    var y1 = frame1.StartY;
                    var x2 = node.Disposition == DispositionNode.Left ? frame2.StartX : frame2.EndX;
                    var y2 = frame2.CenterY;
                    
                    PaintConnection(x1, y1, x2, y2, charMap, foreground);
                }
            }

            return new ConsolePaintModel
            {
                Width = width,
                Height = height,
                CharMap = charMap,
                Background = background,
                Foreground = foreground
            };
        }

        private void PaintFrameColor<T>(Frame<T> frame, ConsoleColor[,] foreground, ConsoleColor[,] background) where T : IComparable<T>
        {
            var fgColor = frame.Node.Color == NodeColor.Black ? BlackNode : RedNode;
            var bgColor = NodeBackground;

            for (var y = frame.StartY; y <= frame.EndY; y++)
            {
                for (var x = frame.StartX; x <= frame.EndX; x++)
                {
                    foreground[y, x] = fgColor;
                    background[y, x] = bgColor;
                }
            }
        }

        private string CharMapToString(char[,] charMap)
        {
            var stringBuilder = new StringBuilder();
            var height = charMap.GetLength(0);
            var width = charMap.GetLength(1);
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    stringBuilder.Append(charMap[i, j]);
                }
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }
        
        private InitialModel CreateInitial(int width, int height)
        {
            var charMap = new char[height, width];
            var foreground = new ConsoleColor[height, width];
            var background = new ConsoleColor[height, width];
            
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    charMap[i, j] = ' ';
                    foreground[i, j] = BlackNode;
                    background[i, j] = DefaultBackground;
                }
            }
            
            return new InitialModel
            {
                CharMap = charMap,
                Background = background,
                Foreground = foreground
            };
        }

        private Frame<T> PaintFrame<T>(int x, int y, Node<T> node, char[,] charMap, out int endX) where T : IComparable<T>
        {
            var strValue = node.Value.ToString();
            var valueLength = strValue.Length;

            endX = x + 2 * FrameToValueIntervalX + valueLength + 1;
            var endY = y + 2 * FrameToValueIntervalY + 2;
            
            // отрисуем углы
            charMap[y, x] = '┌';
            charMap[y, endX] = '┐';
            charMap[endY, x] = '└';
            charMap[endY, endX] = '┘';

            // отрисуем горизонтальные линии
            for (var i = x + 1; i < endX; i++)
            {
                charMap[y, i] = '─';
                charMap[endY, i] = '─';
            }

            // отрисуем вертикальные линии
            for (var i = y + 1; i < endY; i++)
            {
                charMap[i, x] = '│';
                charMap[i, endX] = '│';
            }

            var centerX = (x + endX) / 2;
            if (!node.IsRoot)
            {
                if (valueLength % 2 == 0 && node.Disposition == DispositionNode.Left)
                {
                    centerX++;
                }
                charMap[y, centerX] = '┴';
            }

            var centerY = (y + endY) / 2;

            for (int i = 0, posX = x + FrameToValueIntervalX + 1; i < valueLength; i++, posX++)
            {
                charMap[centerY, posX] = strValue[i];
            }

            if (!node.Left.IsNil)
            {
                charMap[centerY, x] = '┤';
            }

            if (!node.Right.IsNil)
            {
                charMap[centerY, endX] = '├';
            }
            
            return new Frame<T>
            {
                StartX = x,
                CenterX = centerX,
                EndX = endX,
                StartY = y,
                CenterY = centerY,
                EndY = endY,
                Node = node
            };
        }

        /// <summary>
        /// Рисует линию от рамки до рамки, где (x1, y1) нижняя точка, (x2, y2) выше
        /// </summary>
        private void PaintConnection(int x1, int y1, int x2, int y2, char[,] charMap, ConsoleColor[,] foreground)
        {
            for (var y = y1 + 1; y < y2; y++)
            {
                charMap[y, x1] = '│';
                foreground[y, x1] = ConnectionColor;
            }

            if (x1 < x2)
            {
                charMap[y2, x1] = '┌';
                foreground[y2, x1] = ConnectionColor;
            }
            else
            {
                charMap[y2, x1] = '┐';
                foreground[y2, x1] = ConnectionColor;
                (x1, x2) = (x2, x1);
            }

            for (var x = x1 + 1; x < x2; x++)
            {
                charMap[y2, x] = '─';
                foreground[y2, x] = ConnectionColor;
            }
        }
        
        private IEnumerable<(Node<T>, int)> CreateMapRBTree<T>(RBTree<T> rbTree) where T : IComparable<T>
        {
            var curHeight = 0;
            var curNode = rbTree.Root;

            if (curNode.IsNil)
            {
                yield break;
            }

            while (!curNode.Left.IsNil)
            {
                curNode = curNode.Left;
                curHeight++;
            }

            var visited = new HashSet<Node<T>>();
            yield return (curNode, curHeight);
            visited.Add(curNode);

            while (!curNode.IsRoot ||
                   !curNode.Right.IsNil && !visited.Contains(curNode.Right))
            {
                if (!curNode.Left.IsNil && !visited.Contains(curNode.Left))
                {
                    curNode = curNode.Left;
                    curHeight++;
                    continue;
                }
                
                if (!visited.Contains(curNode))
                {
                    yield return (curNode, curHeight);
                    visited.Add(curNode);
                }
                
                if (!curNode.Right.IsNil && !visited.Contains(curNode.Right))
                {
                    curNode = curNode.Right;
                    curHeight++;
                }
                else
                {
                    curNode = curNode.Parent;
                    curHeight--;
                }
            }
        }

        class InitialModel
        {
            public char[,] CharMap { get; set; }
            public ConsoleColor[,] Foreground { get; set; }
            public ConsoleColor[,] Background { get; set; }
        }

        class Frame<T> where T : IComparable<T>
        {
            public int StartX { get; set; }
            public int CenterX { get; set; }
            public int EndX { get; set; }
            
            public int StartY { get; set; }
            public int CenterY { get; set; }
            public int EndY { get; set; }

            public Node<T> Node { get; set; }

            public override int GetHashCode() => Node.GetHashCode();
        }
    }
}
