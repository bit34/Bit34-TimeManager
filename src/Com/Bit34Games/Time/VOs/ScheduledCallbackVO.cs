using System;
using Com.Bit34Games.Time.Constants;

namespace Com.Bit34Games.Time.VOs
{
    public class ScheduledCallbackVO
    {
        //  MEMBERS
        public readonly TimeTypes     timeType;
        public readonly Action<float> callback;
        public readonly TimeSpan      interval;
        public int                    RemainingCallCount { get; private set; }
        public DateTime               LastCall { get; private set; }
        public bool                   IsPaused { get; private set; }
        //      Internal
        private DateTime _pauseTime;


        //  CONSTRUCTORS
        public ScheduledCallbackVO(TimeTypes     timeType,
                                  Action<float> callback,
                                  TimeSpan      interval,
                                  int           callCount,
                                  DateTime      now)
        {
            this.timeType      = timeType;
            this.callback      = callback;
            this.interval      = interval;
            RemainingCallCount = callCount;
            LastCall           = now;
        }


        //  METHODS
        public void Call(TimeSpan elapsed)
        {
            LastCall += elapsed;
            callback((float)elapsed.TotalSeconds);
            if (RemainingCallCount>0)
            {
                RemainingCallCount--;
            }
        }

        public void Pause(DateTime now)
        {
            if(!IsPaused)
            {
                IsPaused = true;
                _pauseTime = now;
            }
        }

        public void Resume(DateTime now)
        {
            if(IsPaused)
            {
                IsPaused = false;
                TimeSpan elapsedTime = now - _pauseTime;
                LastCall = LastCall.Add(elapsedTime);
            }
        }
    }
}