using System;
using Com.Bit34Games.Time.Constants;

namespace Com.Bit34Games.Time.Utilities
{
    public interface ITimeManager
    {
        //  METHODS
        DateTime GetNow(TimeTypes timeType);
        TimeSpan GetDelta(TimeTypes timeType);
        void SetScale(float timeScale);
    }
}
