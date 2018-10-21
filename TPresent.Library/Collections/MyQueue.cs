using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPresenter.Library
{
    //  Queue might be overwhelmed with messages to queue. List<T> toCommit with it's own lock
    //  should be added to accommodate all enquing requests with minimum delay.
    public class MyQueue<T>
    {
        Queue<T> queue = new Queue<T>();
        MyLock queueLock = new MyLock();

        public int Count
        {
            get
            {
                queueLock.Enter();
                try
                {
                    return queue.Count();
                }
                finally
                {
                    queueLock.Exit();
                }
            }
        }

        public void Enqueue(T obj)
        {
            queueLock.Enter();
            try
            {
                queue.Enqueue(obj);
            }
            finally
            {
                queueLock.Exit();
            }
        }

        public bool TryDequeue(out T obj)
        {
            queueLock.Enter();
            try
            {
                if(Count > 0)
                {
                    obj = queue.Dequeue();
                    return true;
                }
                else
                {
                    obj = default(T);
                    return false;
                }
            }
            finally
            {
                queueLock.Exit();
            }
        }
    }
}
