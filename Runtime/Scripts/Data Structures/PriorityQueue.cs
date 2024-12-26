using System;
using System.Collections;
using System.Collections.Generic;

namespace HHG.Common.Runtime
{
    public class PriorityQueue<T, TPriority> : ICollection<KeyValuePair<T, TPriority>> where TPriority : IComparable<TPriority>
    {
        private List<KeyValuePair<T, TPriority>> elements = new List<KeyValuePair<T, TPriority>>();
        private Dictionary<T, int> elementIndices = new Dictionary<T, int>();

        public int Count => elements.Count;

        public bool IsReadOnly => false;

        public void Enqueue(T item, TPriority priority)
        {
            var kvp = new KeyValuePair<T, TPriority>(item, priority);
            elements.Add(kvp);
            elementIndices[item] = elements.Count - 1;
            HeapifyUp(elements.Count - 1);
        }

        public T Dequeue()
        {
            if (elements.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            T result = elements[0].Key;
            Swap(0, elements.Count - 1);
            elements.RemoveAt(elements.Count - 1);
            elementIndices.Remove(result);
            HeapifyDown(0);

            return result;
        }

        public bool Contains(T item)
        {
            return elementIndices.ContainsKey(item);
        }

        public void Add(KeyValuePair<T, TPriority> item)
        {
            Enqueue(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<T, TPriority> item)
        {
            return elementIndices.ContainsKey(item.Key) && elements[elementIndices[item.Key]].Value.Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<T, TPriority>[] array, int arrayIndex)
        {
            elements.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<T, TPriority> item)
        {
            if (Contains(item))
            {
                int index = elementIndices[item.Key];
                Swap(index, elements.Count - 1);
                elements.RemoveAt(elements.Count - 1);
                elementIndices.Remove(item.Key);
                HeapifyDown(index);
                HeapifyUp(index);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            elements.Clear();
            elementIndices.Clear();
        }

        public IEnumerator<KeyValuePair<T, TPriority>> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (elements[index].Value.CompareTo(elements[parentIndex].Value) >= 0)
                {
                    break;
                }

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = elements.Count - 1;
            while (true)
            {
                int leftChildIndex = 2 * index + 1;
                int rightChildIndex = 2 * index + 2;
                int smallestIndex = index;

                if (leftChildIndex <= lastIndex && elements[leftChildIndex].Value.CompareTo(elements[smallestIndex].Value) < 0)
                {
                    smallestIndex = leftChildIndex;
                }

                if (rightChildIndex <= lastIndex && elements[rightChildIndex].Value.CompareTo(elements[smallestIndex].Value) < 0)
                {
                    smallestIndex = rightChildIndex;
                }

                if (smallestIndex == index)
                {
                    break;
                }

                Swap(index, smallestIndex);
                index = smallestIndex;
            }
        }

        private void Swap(int index1, int index2)
        {
            var temp = elements[index1];
            elements[index1] = elements[index2];
            elements[index2] = temp;

            elementIndices[elements[index1].Key] = index1;
            elementIndices[elements[index2].Key] = index2;
        }
    }
}