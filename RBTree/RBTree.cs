using System;
using System.Collections.Generic;

namespace RBTree
{
    public class RBTree<T> where T : IComparable<T>
    {
        public Node<T> Root { get; set; }

        public RBTree()
        {
            Root = null;
        }

        public RBTree(T initial)
        {
            Root = new Node<T> {Value = initial};
        }

        /// <summary>
        /// Каждый узел окрашен либо в красный, либо в черный цвет
        /// </summary>
        public bool IsTrueProperty1()
        {
            // Так как NodeColor содержит только Red и Black - свойство всегда соблюдается
            return true;
        }

        /// <summary>
        /// Корень окрашен в черный цвет
        /// </summary>
        public bool IsTrueProperty2()
        {
            return Root.Color == NodeColor.Black;
        }

        /// <summary>
        /// Листья окрашены в черный цвет
        /// </summary>
        public bool IsTrueProperty3()
        {
            var queue = new Queue<Node<T>>();
            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.IsNIL)
                {
                    if (node.Color == NodeColor.Black)
                    {
                        continue;
                    }

                    return false;
                }

                node.CheckChildNodes();
                queue.Enqueue(node.Left);
                queue.Enqueue(node.Right);
            }

            return true;
        }

        /// <summary>
        /// Каждый красный узел должен иметь два черных дочерних узла
        /// </summary>
        public bool IsTrueProperty4()
        {
            var queue = new Queue<Node<T>>();
            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Color == NodeColor.Red)
                {
                    node.CheckChildNodes();
                    if (node.Left.Color == NodeColor.Black &&
                        node.Right.Color == NodeColor.Black)
                    {
                        continue;
                    }

                    return false;
                }

                node.CheckChildNodes();
                queue.Enqueue(node.Left);
                queue.Enqueue(node.Right);
            }

            return true;
        }

        /// <summary>
        /// Пути от узла к его листьям должны содержать одинаковое количество черных узлов
        /// </summary>
        public bool IsTrueProperty5()
        {
            var blackHeight = -1;
            
            var queue = new Queue<(Node<T>, int)>();
            queue.Enqueue((Root, 0));

            while (queue.Count > 0)
            {
                var (node, height) = queue.Dequeue();
                if (node.IsNIL)
                {
                    if (blackHeight == -1)
                    {
                        blackHeight = height;
                    }
                    
                    if (blackHeight == height)
                    {
                        continue;
                    }

                    return false;
                }

                node.CheckChildNodes();
                queue.Enqueue((node.Left, height + 1));
                queue.Enqueue((node.Right, height + 1));
            }

            return true;
        }

        /// <summary>
        /// Проверка на то, что все свойства КЧ дерева соблюдаются
        /// </summary>
        public bool IsTrueAllProperties()
        {
            return IsTrueProperty1() &&
                   IsTrueProperty2() &&
                   IsTrueProperty3() &&
                   IsTrueProperty4() &&
                   IsTrueProperty5();
        }
    }
}
