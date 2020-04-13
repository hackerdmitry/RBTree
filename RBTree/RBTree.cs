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

        public RBTree(params T[] initial)
        {
            foreach (var value in initial)
            {
                Add(value);
            }
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
            var grandParent = node.GrandParent;

            grandParent.ChangeColor();
            grandParent.Left.ChangeColor();
            grandParent.Right.ChangeColor();

            SaveIntegrityRBTree(grandParent);
        }

        private void RectifySituationRedParentAndBlackUncle(Node<T> node)
        {
            var parent = node.Parent;
            if (node.Disposition != parent.Disposition)
            {
                if (node.Disposition == DispositionNode.Right)
                {
                    RotateLeft(parent);
                }
                else
                {
                    RotateRight(parent);
                }

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

            grandParent.Color = NodeColor.Red;
            grandParent = grandParent.Parent;
            grandParent.Color = NodeColor.Black;
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

            if (leftChild.IsRoot)
            {
                Root = leftChild;
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

            if (rightChild.IsRoot)
            {
                Root = rightChild;
            }
        }

        public bool Remove(T value)
        {
            var deleteNode = Root;
            // найдем узел с ключом value
            while (deleteNode.Value.CompareTo(value) != 0 && !deleteNode.IsNil)
            {
                deleteNode = deleteNode.Value.CompareTo(value) < 0 ? deleteNode.Right : deleteNode.Left;
            }

            // не нашли? значит его нет!? не проблема! просто завершаем работу
            if (deleteNode.IsNil)
            {
                return false;
            }

            Count--;
            // !!!
            var workNode = new Node<T>();
            if (deleteNode.Left.IsNil || deleteNode.Right.IsNil)
            {
                workNode = deleteNode;
            }
            else
            {
                workNode = deleteNode.Right;
                while (!workNode.Left.IsNil)
                {
                    workNode = workNode.Left;
                }
            }

            var linkedNode = CreateNil();
            linkedNode.SetParent(workNode.Parent, false);
            if (workNode.Parent != null)
            {
                if (workNode == workNode.Parent.Left)
                {
                    workNode.Parent.Left = linkedNode;
                }
                else
                {
                    workNode.Parent.Right = linkedNode;
                }
            }
            else
            {
                Root = linkedNode;
            }

            if (workNode != deleteNode)
            {
                deleteNode.Value = workNode.Value;
            }

            if (workNode.Color == NodeColor.Black)
            {
                BalanceTreeAfterDelete(linkedNode);
            }

            /*// если детей нет - все просто
            if (deleteNode.Left.IsNil && deleteNode.Right.IsNil)
            {
                if (deleteNode.IsRoot)
                {
                    Root = CreateNil();
                    return true;
                }

                var nil = CreateNil();
                if (deleteNode.Disposition == DispositionNode.Left)
                {
                    deleteNode.Parent.Left = nil;
                }
                else
                {
                    deleteNode.Parent.Right = nil;
                }

                workNode = deleteNode.Parent;
                deleteNode.Parent = CreateNil();
            }
            // если есть один ребенок то поменяем ссылку его родителя на родителя родителя
            else if (deleteNode.Left.IsNil ^ deleteNode.Right.IsNil)
            {
                var child = deleteNode.Left.IsNil ? deleteNode.Right : deleteNode.Left;
                if (deleteNode.IsRoot)
                {
                    Root = child;
                }
                child.Parent = deleteNode.Parent;
                workNode = child;
            }
            else
            {
                workNode = deleteNode.Right;
                while (!workNode.Left.IsNil)
                {
                    workNode = workNode.Left;
                }

                workNode.Left = deleteNode.Left;
                if (deleteNode.Right != workNode)
                {
                    workNode.Parent.Left = workNode.Right;
                    workNode.Right = deleteNode.Right;
                }
                workNode.Parent = deleteNode.Parent;

                if (workNode.IsRoot)
                {
                    Root = workNode;
                }
            }

            // [работает] [при удалении 1]
            // y.Right.Color = NodeColor.Red;
            // var brother = y.Parent.Right;
            // brother.Color = NodeColor.Black;
            // y.Parent.Color = NodeColor.Black;
            // brother.Right.Color = NodeColor.Black;
            // RotateLeft(y.Parent);

            // [работает] [при удалении 2]
            // y.Left.Color = NodeColor.Red;
            // var brother = y.Parent.Right;
            // brother.Color = NodeColor.Black;
            // y.Parent.Color = NodeColor.Black;
            // brother.Right.Color = NodeColor.Black;
            // RotateLeft(y.Parent);

            // [работает] [при удалении 3]
            // y.Left.Color = NodeColor.Red;
            // var brother = y.Parent.Right;
            // brother.Color = NodeColor.Black;
            // y.Parent.Color = NodeColor.Black;
            // brother.Right.Color = NodeColor.Black;
            // RotateLeft(y.Parent);

            // [работает] [при удалении 4]
            workNode = workNode.Left;
            // y.Left.Color = NodeColor.Red;
            // var brother = y.Parent.Right;
            // brother.Color = NodeColor.Black;
            // y.Parent.Color = NodeColor.Black;
            // brother.Right.Color = NodeColor.Black;
            // RotateLeft(y.Parent);
            
            // [работает] [при удалении 6]
            // y.Left.Color = NodeColor.Red; 
            
            // [работает] [при удалении 10]
            // y.Left.Color = NodeColor.Red; 

            // if (y.Color == NodeColor.Black)
            // {
            //     FixDeleting();
            // }*/
            return true;
        }
        
        private void BalanceTreeAfterDelete(Node<T> linkedNode)
        {
            while (linkedNode != Root && linkedNode.Color == NodeColor.Black)
            {
                Node<T> workNode;
                if (linkedNode == linkedNode.Parent.Left)
                {
                    workNode = linkedNode.Parent.Right;
                    if (workNode.Color == NodeColor.Red)
                    {
                        linkedNode.Parent.Color = NodeColor.Red;
                        workNode.Color = NodeColor.Black;
                        RotateLeft(linkedNode.Parent);
                        workNode = linkedNode.Parent.Right;
                    }
                    if (workNode.Left.Color == NodeColor.Black &&
                        workNode.Right.Color == NodeColor.Black)
                    {
                        workNode.Color = NodeColor.Red;
                        linkedNode = linkedNode.Parent;
                    }
                    else
                    {
                        if (workNode.Right.Color == NodeColor.Black)
                        {
                            workNode.Left.Color = NodeColor.Black;
                            workNode.Color = NodeColor.Red;
                            RotateRight(workNode);
                            workNode = linkedNode.Parent.Right;
                        }
                        linkedNode.Parent.Color = NodeColor.Black;
                        workNode.Color = linkedNode.Parent.Color;
                        workNode.Right.Color = NodeColor.Black;
                        RotateLeft(linkedNode.Parent);
                        linkedNode = Root;
                    }
                }
                else
                {
                    workNode = linkedNode.Parent.Left;
                    if (workNode.Color == NodeColor.Red)
                    {
                        linkedNode.Parent.Color = NodeColor.Red;
                        workNode.Color = NodeColor.Black;
                        RotateRight(linkedNode.Parent);
                        workNode = linkedNode.Parent.Left;
                    }
                    if (workNode.Right.Color == NodeColor.Black &&
                        workNode.Left.Color == NodeColor.Black)
                    {
                        workNode.Color = NodeColor.Red;
                        linkedNode = linkedNode.Parent;
                    }
                    else
                    {
                        if (workNode.Left.Color == NodeColor.Black)
                        {
                            workNode.Right.Color = NodeColor.Black;
                            workNode.Color = NodeColor.Red;
                            RotateLeft(workNode);
                            workNode = linkedNode.Parent.Left;
                        }
                        workNode.Color = linkedNode.Parent.Color;
                        linkedNode.Parent.Color = NodeColor.Black;
                        workNode.Left.Color = NodeColor.Black;
                        RotateRight(linkedNode.Parent);
                        linkedNode = Root;
                    }
                }
            }
            linkedNode.Color = NodeColor.Black;
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
                if (node.IsNil)
                {
                    continue;
                }

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

                if (node.Color == NodeColor.Black)
                {
                    height++;
                }

                queue.Enqueue((node.Left, height));
                queue.Enqueue((node.Right, height));
            }

            return true;
        }

        /// <summary>
        /// Проверка на то, что все свойства КЧ дерева соблюдаются
        /// </summary>
        public string IsTrueAllProperties()
        {
            var prop1 = IsTrueProperty1();
            var prop2 = IsTrueProperty2();
            var prop3 = IsTrueProperty3();
            var prop4 = IsTrueProperty4();
            var prop5 = IsTrueProperty5();

            return $"1: {prop1}\n" +
                   $"2: {prop2}\n" +
                   $"3: {prop3}\n" +
                   $"4: {prop4}\n" +
                   $"5: {prop5}\n" +
                   $"Итог: {prop1 && prop2 && prop3 && prop4 && prop5}";
        }

    #endregion
    }
}