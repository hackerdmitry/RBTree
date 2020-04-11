using System;
using RBTree.Exceptions;

namespace RBTree {
    public class Node<T> where T : IComparable<T>
    {
        public Node<T> Parent { get; set; }
        
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public NodeColor Color { get; set; }

        public T Value { get; set; }

        public bool IsRoot => Parent == null;
        public bool IsNIL => Left == null && Right == null;

        public void CheckChildNodes()
        {
            if (Left == null ^ Right == null)
            {
                throw new NotNormalChildNodes<T>(this);
            }
        }
    }

    public enum NodeColor
    {
        Red,
        Black
    }
}
