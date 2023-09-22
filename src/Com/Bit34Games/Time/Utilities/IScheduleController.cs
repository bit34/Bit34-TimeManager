using System;
using Com.Bit34Games.Time.Constants;

namespace Com.Bit34Games.Time.Utilities
{
    public interface IScheduleController
    {
        //  METHODS
        void SetUpdate(Action updateMethod);
        DateTime GetNow(TimeTypes timeType);
        TimeSpan GetDelta(TimeTypes timeType);
    }
}
