using System;
using System.Collections.Generic;

namespace RBTree
{
    public class RBTree<T> where T : IComparable<T>
    {
        public Node<T> Root { get; set; } = CreateNil();
        public int Count { get; set; }

        public RBTree() { }

        public RBTree(T initial)
        {
            Add(initial);
        }

        public void Add(T value)
        {
            Count++;
        
            if (Root.IsNil)
            {
                RefreshRootNode(value);
                return;
            }

            var newNode = CreateNode(value);
            var curNode = Root;
            while (!curNode.IsNil)
            {
                curNode = newNode > curNode ? curNode.Right : curNode.Left;
            }

            newNode.Parent = curNode.Parent;
            SaveIntegrityRBTree(newNode);
        }

        private void SaveIntegrityRBTree(Node<T> node)
        {
            if (node.IsRedParentAndRedUncle())
            {
                RectifySituationRedParentAndRedUncle(node);
            } 
            else if (node.IsRedParentAndBlackUncle())
            {
                RectifySituationRedParentAndBlackUncle(node);
            }
        }

        private void RectifySituationRedParentAndRedUncle(Node<T> node)
        {
            do
            {
                var grandParent = node.GrandParent;

                grandParent.ChangeColor();
                grandParent.Left.ChangeColor();
                grandParent.Right.ChangeColor();

                node = grandParent;
            }
            while (node.IsRedParentAndRedUncle());
        }

        private void RectifySituationRedParentAndBlackUncle(Node<T> node)
        {
            var parent = node.Parent;
            if (node.Disposition == DispositionNode.Right)
            {
                RotateLeft(parent);
                parent = node;
            }

            var grandParent = parent.Parent;
            if (parent.Disposition == DispositionNode.Left)
            {
                RotateRight(grandParent);
            }
            else
            {
                RotateLeft(grandParent);
            }
            
            grandParent = grandParent.Parent;
            if (grandParent.IsRoot)
            {
                Root = grandParent;
            }
        }

        private void RotateRight(Node<T> node)
        {
            var parent = node.Parent;

            var leftChild = node.Left;
            var leftChildRightGrandson = leftChild.Right;

            node.Left = leftChildRightGrandson;
            node.Parent = leftChild;
            leftChild.Parent = parent;
            
            if (node.Color == NodeColor.Black && 
                leftChild.Color == NodeColor.Red)
            {
                leftChild.ChangeColor();
                node.ChangeColor();
            }
        }

        private void RotateLeft(Node<T> node)
        {
            var parent = node.Parent;

            var rightChild = node.Right;
            var rightChildLeftGrandson = rightChild.Left;

            node.Right = rightChildLeftGrandson;
            node.Parent = rightChild;
            rightChild.Parent = parent;
            
            if (node.Color == NodeColor.Black && 
                rightChild.Color == NodeColor.Red)
            {
                rightChild.ChangeColor();
                node.ChangeColor();
            }
        }

        private void RefreshRootNode(T value)
        {
            var leftNil = CreateNil();
            var rightNil = CreateNil();
            Root = new Node<T>
            {
                Color = NodeColor.Black,
                Value = value,
                Left = leftNil,
                Right = rightNil
            };
        }

        private static Node<T> CreateNode(T value)
        {
            var leftNil = CreateNil();
            var rightNil = CreateNil();
            return new Node<T>
            {
                Color = NodeColor.Red,
                Value = value,
                Left = leftNil,
                Right = rightNil
            };
        }

        private static Node<T> CreateNil()
        {
            return new Node<T> {Color = NodeColor.Black};
        }

    #region Проверка свойств КЧ дерева
        
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
                if (node.IsNil)
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
                if (node.IsNil)
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

    #endregion
    }
}
