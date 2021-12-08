using System;

namespace Com.Bit34Games.Unity.Update
{
    public class UpdateCallbackData
    {
        //  MEMBERS
        public readonly object              owner;
        public readonly UpdateTimeTypes     timeType;
        public readonly UpdateCallbackTypes callbackType;
        public readonly Action              callback;
        public readonly TimeSpan            delay;
        public readonly bool                removeAfterCall;
        public          bool                IsPaused { get; private set; }
        public          DateTime            LastCall { get; private set; }
        //      Internal
        private DateTime _pauseTime;


        //  CONSTRUCTOR
        public UpdateCallbackData(object owner, UpdateTimeTypes timeType, UpdateCallbackTypes updateType, Action callback, TimeSpan delay, DateTime now, bool removeAfterCall)
        {
            this.owner           = owner;
            this.timeType        = timeType;
            this.callbackType    = updateType;
            this.callback        = callback;
            this.delay           = delay;
            this.removeAfterCall = removeAfterCall;
            LastCall             = now;
        }


        //  METHODS
        public void Call(DateTime now)
        {
            LastCall = now;
            callback();
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