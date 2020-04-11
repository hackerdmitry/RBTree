using System;
using RBTree.Exceptions;

namespace RBTree {
    public class Node<T> where T : IComparable<T>
    {
        public Node<T> Parent { get; set; }
        public Node<T> Uncle
        {
            get
            {
                var parent = Parent;
                if (parent == null)
                {
                    return null;
                }
                
                parent.CheckChildNodes();
                return Disposition == DispositionNode.Left
                           ? parent.Right
                           : parent.Left;
            }
        }
        public Node<T> GrandParent => Parent?.Parent;

        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public NodeColor Color { get; set; }
        public DispositionNode? Disposition => Parent == null
                                                   ? (DispositionNode?) null
                                                   : this > Parent
                                                       ? DispositionNode.Right
                                                       : DispositionNode.Left;

        public T Value { get; set; }

        public bool IsRoot => Parent == null;
        public bool IsNil => Left == null && Right == null;

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
