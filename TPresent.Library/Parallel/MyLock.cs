using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TPresenter.Library
{
    public class MyLock
    {
        private Thread lockOwner;
        private int recoursion;

        public void Enter()
        {
            Thread caller = Thread.CurrentThread;

            if(caller == lockOwner)
            {
                Interlocked.Increment(ref recoursion);
                return;
            }

            //Will compare lock ownership until lockOwner is null.
            while (Interlocked.CompareExchange(ref lockOwner, caller, null) != null) ;
            Interlocked.Increment(ref recoursion);
        }

        public bool TryEnter()
        {
            Thread caller = Thread.CurrentThread;

            if (caller == lockOwner)
            {
                Interlocked.Increment(ref recoursion);
                return true;
            }

            bool result = Interlocked.CompareExchange(ref lockOwner, caller, null) == null;
            if(result)
                Interlocked.Increment(ref recoursion);
            return result;
        }

        public void Exit()
        {
            Thread caller = Thread.CurrentThread;

            if (caller == lockOwner)
            {
                Interlocked.Decrement(ref recoursion);
                if (recoursion == 0)
                    lockOwner = null;
            }
            else
                throw new InvalidOperationException("Cannot exit lock from a thread that does not own it!");
        }
    }
}
