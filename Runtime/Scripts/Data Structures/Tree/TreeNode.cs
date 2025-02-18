using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHG.Common.Runtime
{
    [Serializable]
    public class TreeNode<TNode, TData> where TNode : TreeNode<TNode, TData>
    {
        public TData Value => value;
        public TNode Parent => parent;
        public IReadOnlyList<TNode> Children => children;
        public Vector3 Position { get => position; set => position = value; }

        [SerializeField] private TData value;
        [SerializeField] private TNode parent;
        [SerializeField] private List<TNode> children = new List<TNode>();
        [SerializeField] private Vector3 position;

        public TreeNode(TData value)
        {
            this.value = value;
        }

        public void AddChild(TNode node)
        {
            node.parent = (TNode)this;
            children.Add(node);
        }

        public void AddChildren(IEnumerable<TNode> nodes)
        {
            foreach (TNode node in nodes)
            {
                AddChild(node);
            }
        }

        public void RemoveChildren(IEnumerable<TNode> nodes)
        {
            foreach (TNode node in nodes)
            {
                RemoveChild(node);
            }
        }

        public void RemoveChild(TNode node)
        {
            if (children.Contains(node))
            {
                node.RemoveFromParent();
            }
        }

        public void RemoveChildAt(int index)
        {
            if (index >= 0 && index < children.Count)
            {
                children[index].RemoveFromParent();
            }
        }

        public void RemoveFromParent()
        {
            if (parent != null)
            {
                parent.children.Remove((TNode)this);
                parent = null;
            }
        }

        public bool TryGetChild(int i, out TNode child)
        {
            if (i >= 0 && i < children.Count)
            {
                child = children[i];
                return true;
            }
            else
            {
                child = default;
                return false;
            }
        }

        public List<TNode> GetLeafNodes()
        {
            List<TNode> leaves = new List<TNode>();
            Queue<TNode> queue = new Queue<TNode>();
            queue.Enqueue((TNode)this);
            while (queue.Count > 0)
            {
                TNode node = queue.Dequeue();
                if (node.children.Count == 0)
                {
                    leaves.Add(node);
                }
                else
                {
                    foreach (TNode child in node.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
            return leaves;
        }

        public TNode GetRootNode()
        {
            TNode node = (TNode)this;
            while (node.parent != null)
            {
                node = node.Parent;
            }
            return node;
        }

        public List<TNode> GetAllNodes()
        {
            List<TNode> nodes = new List<TNode>();
            GetAllNodes(nodes);
            return nodes;
        }

        public void GetAllNodes(List<TNode> nodes)
        {
            nodes.Clear();
            nodes.Add((TNode)this);
            foreach (TNode child in Children)
            {
                child.GetAllNodes(nodes);
            }
        }
    }
}