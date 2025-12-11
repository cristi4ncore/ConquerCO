// * This Project Created by Mostafa Desha
// * Copyright MostafaDesha © 2025


namespace System.Threading
{
    public class LazyDelegate : TimerRule
    {
        public LazyDelegate(Action<int> action, int dueTime, ThreadPriority priority = ThreadPriority.Normal)
            : base(action, dueTime, priority)
        {
            this.bool_0 = false;
        }
    }
}