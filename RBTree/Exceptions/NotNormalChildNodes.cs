using System;

namespace RBTree.Exceptions
{
    public class NotNormalChildNodes<T> : Exception where T : IComparable<T>
    {
        public NotNormalChildNodes(Node<T> node) 
            : base($"Узел со значением '{node.Value}' имеет ненормальные дочерние узлы") { }
    }
}

