using System;
using System.Timers;
using Com.Bit34Games.Time.Constants;

namespace Com.Bit34Games.Time.Utilities
{
    public class DefaultTime : ITime
    {
        //  MEMBERS
        public float TickInterval { get; private set; }
        public float TimeScale { get; private set; }
        //      Private
        private Action     _tickMethod;
        private DateTime   _applicationStartTime;
        private DateTime[] _nows;
        private TimeSpan[] _deltas;
        private Timer      _timer;

        //  CONSTRUCTORS
        public DefaultTime(float tickInterval, float timeScale = 1)
        {
            TickInterval              = tickInterval;
            TimeScale                 = timeScale;
            _applicationStartTime     = DateTime.UtcNow;
            _nows                     = new DateTime[3];
            _deltas                   = new TimeSpan[3];
            _nows[(int)TimeTypes.Utc] = _applicationStartTime;

            _timer = new Timer(TickInterval);
            _timer.Elapsed += OnElapsedEvent;
            _timer.Enabled = true;
        }

#region ITime implementation

        public void     AddTickMethod(Action method)  { _timer.Elapsed += (Object source, ElapsedEventArgs e)=>{ method(); }; }

        public void     SetTimeScale(float timeScale) { TimeScale = timeScale; }

        public TimeSpan GetDelta(TimeTypes timeType)  { return _deltas[(int)timeType]; }

        public DateTime GetNow(TimeTypes timeType)    { return _nows[(int)timeType]; }

#endregion

        private void OnElapsedEvent(Object source, ElapsedEventArgs e)
        {
            _deltas[(int)TimeTypes.Utc                ] = DateTime.UtcNow - GetNow(TimeTypes.Utc);
            _nows  [(int)TimeTypes.Utc                ] = GetNow(TimeTypes.Utc) + GetDelta(TimeTypes.Utc);

            _deltas[(int)TimeTypes.Application        ] = GetDelta(TimeTypes.Utc);
            _nows  [(int)TimeTypes.Application        ] = GetNow(TimeTypes.Application) + GetDelta(TimeTypes.Application);

            _deltas[(int)TimeTypes.ApplicationScaled  ] = TimeSpan.FromSeconds(GetDelta(TimeTypes.Application).TotalSeconds * TimeScale);
            _nows  [(int)TimeTypes.ApplicationScaled  ] = GetNow(TimeTypes.ApplicationScaled) + GetDelta(TimeTypes.ApplicationScaled);
        }
    }
}
