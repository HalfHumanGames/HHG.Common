using HHG.Common.Runtime;
using System.Collections.Generic;

namespace HHG.LockKeySystem.Runtime
{
    public class TreeVisitor<TNode, TData> where TNode : TreeNode<TNode, TData>
    {
        private HashSet<TNode> visited = new HashSet<TNode>();

        public void DepthFirstSearch(TNode node)
        {
            visited.Clear();
            RecursiveDepthFirstSearch(node);
        }

        private void RecursiveDepthFirstSearch(TNode node)
        {
            if (!visited.Contains(node))
            {
                Visit(node);
                visited.Add(node);
                foreach (TNode child in node.Children)
                {
                    Visit(child);
                    RecursiveDepthFirstSearch(child);
                }
            }
        }

        public void BreadthFirstSearch(TNode node)
        {
            visited.Clear();
            Queue<TNode> queue = new Queue<TNode>();
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                TNode current = queue.Dequeue();
                Visit(current);
                visited.Add(current);
                foreach (TNode child in current.Children)
                {
                    if (!visited.Contains(child))
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }

        protected virtual void Visit(TNode node)
        {

        }
    }
}