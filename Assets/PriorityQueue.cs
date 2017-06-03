// C# does not implement its own PriorityQueue, so I had to implement my own.
// Code take from this tutorial and explanation:
//  https://visualstudiomagazine.com/Articles/2012/11/01/Priority-Queues-with-C.aspx?Page=1
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data;

    public PriorityQueue()
    {
        data = new List<T>();
    }

    public T Peek()
    {
        return data[0];
    }

    public int Count()
    {
        return data.Count;
    }

    public bool Contains(T value)
    {
        return data.Contains(value);
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int ci = data.Count - 1;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (data[ci].CompareTo(data[pi]) >= 0)
                break;
            T tmp = data[ci];
            data[ci] = data[pi];
            data[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        // Assumes priority queue isn't empty
        int li = data.Count - 1;
        T frontItem = data[0];
        data[0] = data[li];
        data.RemoveAt(li);

        --li;
        int pi = 0;
        while (true)
        {
            int ci = pi * 2 + 1;
            if (ci > li)
                break;

            int rc = ci + 1;
            if (rc <= li && data[rc].CompareTo(data[ci]) < 0)
                ci = rc;
            if (data[pi].CompareTo(data[ci]) <= 0)
                break;

            T tmp = data[pi];
            data[pi] = data[ci];
            data[ci] = tmp;
            pi = ci;
        }
        return frontItem;
    }

    public bool IsConsistent()
    {
        if (data.Count == 0) return true;
        int li = data.Count - 1; // last index
        for (int pi = 0; pi < data.Count; ++pi) // each parent index
        {
            int lci = 2 * pi + 1; // left child index
            int rci = 2 * pi + 2; // right child index
            if ((lci <= li && data[pi].CompareTo(data[lci]) > 0) || (rci <= li && data[pi].CompareTo(data[rci]) > 0))
                return false;
        }
        return true; // Passed all checks
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < data.Count; ++i)
            s += data[i].ToString() + " ";
        s += "count = " + data.Count;
        return s;
    }
}
