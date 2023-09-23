using System;
using Com.Bit34Games.Time.Constants;

namespace Com.Bit34Games.Time.Utilities
{
    public interface IScheduleManager
    {
        //  METHODS
        //  Call every tick until removed
        void AddTick(object owner, TimeTypes timeType, Action<float> callback);
        //  Call every tick until removed or call count completed
        void AddTick(object owner, TimeTypes timeType, Action<float> callback, int callCount);
        //  Call every interval until removed
        void AddInterval(object owner, TimeTypes timeType, Action<float> callback, TimeSpan interval);
        //  Call every interval until removed or call count completed
        void AddInterval(object owner, TimeTypes timeType, Action<float> callback, TimeSpan interval, int callCount);
        //  Pause callback
        void Pause(object owner, Action<float> callback);
        //  Pause all callbacks from same owner
        void PauseAllFrom(object owner);
        //  Resume callback
        void Resume(object owner, Action<float> callback);
        //  Resume all callbacks from same owner
        void ResumeAllFrom(object owner);
        //  Remove callback
        void Remove(object owner, Action<float> callback);
        //  Remove all callbacks from same owner
        void RemoveAllFrom(object owner);
    }
}
