using System;
using RBTree.Exceptions;

namespace RBTree
{
    public class Node<T> where T : IComparable<T>
    {
        private static int id;
        public int Id { get; }

        private Node<T> _parent;
        public Node<T> Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                if (_parent != null)
                {
                    if (Disposition == DispositionNode.Left)
                    {
                        _parent._left = this;
                    }
                    else
                    {
                        _parent._right = this;
                    }
                }
            }
        }

        public Node<T> Uncle
        {
            get
            {
                var grandParent = GrandParent;
                if (grandParent == null)
                {
                    return null;
                }

                grandParent.CheckChildNodes();
                return Parent.Disposition == DispositionNode.Left
                           ? grandParent.Right
                           : grandParent.Left;
            }
        }
        public Node<T> GrandParent => Parent?.Parent;

        private Node<T> _left;
        public Node<T> Left
        {
            get => _left;
            set
            {
                _left = value;
                _left._parent = this;
            }
        }

        private Node<T> _right;
        public Node<T> Right
        {
            get => _right;
            set
            {
                _right = value;
                _right._parent = this;
            }
        }

        public NodeColor Color { get; set; }
        public DispositionNode? Disposition => Parent == null
                                                   ? (DispositionNode?) null
                                                   : this > Parent
                                                       ? DispositionNode.Right
                                                       : DispositionNode.Left;

        public T Value { get; set; }

        public bool IsRoot => Parent == null;
        public bool IsNil => Left == null && Right == null;

        public Node()
        {
            Id = id++;
        }

        public void CheckChildNodes()
        {
            if (Left == null ^ Right == null)
            {
                throw new NotNormalChildNodes<T>(this);
            }
        }

        public bool IsRedParentAndRedUncle()
        {
            return !IsRoot &&
                   Parent.Color == NodeColor.Red &&
                   Uncle.Color == NodeColor.Red;
        }

        public bool IsRedParentAndBlackUncle()
        {
            return !IsRoot &&
                   Parent.Color == NodeColor.Red &&
                   Uncle.Color == NodeColor.Black;
        }

        public void ChangeColor()
        {
            Color = IsRoot
                        ? NodeColor.Black
                        : Color == NodeColor.Red
                            ? NodeColor.Black
                            : NodeColor.Red;
        }

        public static bool operator >(Node<T> node1, Node<T> node2)
        {
            return node1.Value.CompareTo(node2.Value) > 0;
        }

        public static bool operator <(Node<T> node1, Node<T> node2)
        {
            return node1.Value.CompareTo(node2.Value) < 0;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return $"[Node: {Value}|{$"{Color:F}"[0]}]";
        }
    }

    public enum NodeColor
    {
        Red,
        Black
    }

    public enum DispositionNode
    {
        Left,
        Right
    }
}
