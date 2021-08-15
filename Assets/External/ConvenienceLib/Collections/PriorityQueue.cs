using System;
using System.Collections.Generic;

namespace Convenience.Collections {
    /// <summary>
    /// Queue that contains the element with the highest priority in the first index. Implementation is based on binary heap.
    /// See: https://visualstudiomagazine.com/Articles/2012/11/01/Priority-Queues-with-C.aspx?Page=1
    /// </summary>
    public class PriorityQueue<T> where T : IComparable<T> {
        private readonly List<T> data;

        public PriorityQueue() {
            data = new List<T>();
        }

        public void Add(T item) {
            data.Add(item); // add item to end of list
            int itemId = data.Count - 1; // item id
            while (itemId > 0) {
                int parentId = (itemId - 1) / 2; // parent id
                if (data[itemId].CompareTo(data[parentId]) >= 0)
                    break; // item.priority >= parent.priority so we're done
                           // switch item with parent
                T tmp = data[itemId];
                data[itemId] = data[parentId];
                data[parentId] = tmp;
                itemId = parentId;
            }
        }


        public T Peek() {
            return data[0];
        }

        public T Pop() {
            T firstItem = data[0];
            return firstItem;
        }

        public T Dequeue() {
            // assumes queue is not empty; up to calling code
            int lastId = data.Count - 1; // last index (before removal)
            T frontItem = data[0];   // fetch the front
            data[0] = data[lastId];
            data.RemoveAt(lastId);

            --lastId; // last index (after removal)
            int parentId = 0; // parent index. start at front of queue
            while (true) {
                int childId = parentId * 2 + 1;     // left child index of parent
                if (childId > lastId) break;        // no children so done
                int rightChild = childId + 1;       // right child
                if (rightChild <= lastId && data[rightChild].CompareTo(data[childId]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    childId = rightChild;
                if (data[parentId].CompareTo(data[childId]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                T tmp = data[parentId]; data[parentId] = data[childId]; data[childId] = tmp; // swap parent and child
                parentId = childId;
            }
            return frontItem;
        }
    }
}
