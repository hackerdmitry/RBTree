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
            RefreshRootNode(initial);
        }

        public void Add(T value)
        {
            if (Root == null)
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

            SetParent(newNode, curNode.Parent);
            if (newNode.IsRedParentAndRedUncle())
            {
                RectifySituationRedParentAndRedUncle(newNode);
            } 
            else if (newNode.IsRedParentAndBlackUncle())
            {
                RectifySituationRedParentAndBlackUncle(newNode);
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
            throw new NotImplementedException();
        }

        private void RotateRight(Node<T> node)
        {
            var parent = node.Parent;

            var leftChild = node.Left;
            var leftChildRightGrandson = leftChild.Right;

            node.Left = leftChildRightGrandson;
            node.Parent = leftChild;
            leftChild.Right = node;
            if (parent != null)
            {
                SetParent(leftChild, parent);
            }
            
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
            rightChild.Left = node;
            if (parent != null)
            {
                SetParent(rightChild, parent);
            }
            
            if (node.Color == NodeColor.Black && 
                rightChild.Color == NodeColor.Red)
            {
                rightChild.ChangeColor();
                node.ChangeColor();
            }
        }

        private static void SetParent(Node<T> node, Node<T> parent)
        {
            node.Parent = parent;
            if (node > parent)
            {
                parent.Right = node;
            }
            else
            {
                parent.Left = node;
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
