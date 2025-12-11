// * This Project Created by Mostafa Desha
// * Copyright MostafaDesha © 2025


namespace System.Threading.Generic
{
    public class LazyDelegate<T> : TimerRule<T>
    {
        public LazyDelegate(Action<T, int> action, int dueTime, ThreadPriority priority = ThreadPriority.Normal)
            : base(action, dueTime, priority)
        {
            this.bool_0 = false;
        }
    }
}