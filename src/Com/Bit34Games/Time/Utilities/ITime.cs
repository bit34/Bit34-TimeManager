using System;
using Com.Bit34Games.Time.Constants;

namespace Com.Bit34Games.Time.Utilities
{
    public interface ITime
    {
        //  MEMBERS
        float TickInterval { get; }
        float TimeScale { get; }

        //  METHODS
        void AddTickMethod(Action method);
        void SetTimeScale(float timeScale);
        DateTime GetNow(TimeTypes timeType);
        TimeSpan GetDelta(TimeTypes timeType);
    }
}
