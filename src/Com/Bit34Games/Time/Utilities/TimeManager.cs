using System;
using System.Timers;
using Com.Bit34Games.Time.Constants;
using Com.Bit34Games.Time.Utilities;

namespace Com.Bit34Games.Time.Utilities
{
    public class TimeManager
    {
        //  MEMBERS
        public ITime     Time { get; private set; }
        public Scheduler Scheduler { get{ return _scheduler; } }
        //      Private
        private Scheduler _scheduler;

        //  CONSTRUCTORS
        public TimeManager(ITime time)
        {
            Time       = time;
            _scheduler = new Scheduler(time);
        }
    }
}
