#if UNITY_2018_1_OR_NEWER

using System;
using System.Collections.Generic;
using Com.Bit34Games.Time.Constants;
using Com.Bit34Games.Time.Utilities;

namespace Com.Bit34Games.Time.Unity
{

    internal class TimeData : ITime
    {
        //  MEMBERS
        public float TickInterval { get{ return 1.0f / UnityEngine.Application.targetFrameRate; } }
        public float TimeScale    { get{ return UnityEngine.Time.timeScale; } }
        //      Private
        private List<Action> _updateMethods;

        //  CONSTRUCTOR
        public TimeData()
        {
            _updateMethods = new List<Action>();
        }

        //  METHODS
        public void Update()
        {
            for (int i = 0; i < _updateMethods.Count; i++)
            {
                _updateMethods[i]();
            }
        }

#region ITime implementation

        public void AddTickMethod(Action method)
        {
            _updateMethods.Add(method);
        }

        public void SetTimeScale(float timeScale)
        {
            UnityEngine.Time.timeScale = timeScale;
        }

        public DateTime GetNow(TimeTypes timeType)
        {
            if (timeType == TimeTypes.Utc)               {   return DateTime.UtcNow; }
            if (timeType == TimeTypes.Application)       {   return DateTime.MinValue.Add(TimeSpan.FromSeconds(UnityEngine.Time.unscaledTime)); }
            if (timeType == TimeTypes.ApplicationScaled) {   return DateTime.MinValue.Add(TimeSpan.FromSeconds(UnityEngine.Time.time)); }
            throw new Exception("Not implemented");
        }

        public TimeSpan GetDelta(TimeTypes timeType)
        {
            if (timeType == TimeTypes.Utc)               {   return TimeSpan.FromSeconds(UnityEngine.Time.unscaledDeltaTime); }
            if (timeType == TimeTypes.Application)       {   return TimeSpan.FromSeconds(UnityEngine.Time.unscaledDeltaTime); }
            if (timeType == TimeTypes.ApplicationScaled) {   return TimeSpan.FromSeconds(UnityEngine.Time.deltaTime); }
            throw new Exception("Not implemented");
        }

#endregion
    }
}

#endif
