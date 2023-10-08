using System;
using System.Collections.Generic;
using Com.Bit34Games.Time.Constants;
using Com.Bit34Games.Time.VOs;

namespace Com.Bit34Games.Time.Utilities
{
    public class Scheduler : IScheduler
    {
        //  MEMBERS
        //      Private
        private ITime                               _time;
        private Dictionary<object, ScheduleOwnerVO> _owners;
        private static bool                         _isUpdating;
        private LinkedList<Action>                  _postUpdateMethods;

        //  CONSTRUCTORS
        public Scheduler(ITime time)
        {
            _time              = time;
            _owners            = new Dictionary<object, ScheduleOwnerVO>();
            _postUpdateMethods = new LinkedList<Action>();

            _time.AddTickMethod(Update);
        }

        //  METHODS
        public void AddTick(object owner, TimeTypes timeType, Action<float> callback)
        {
            ScheduleOwnerVO     scheduleOwner     = GetOrCreateOwner(owner);
            ScheduledCallbackVO scheduledCallback = new ScheduledCallbackVO(timeType, callback, TimeSpan.FromSeconds(0), -1, _time.GetNow(timeType));

            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ scheduleOwner.tickCallbacks.AddLast(scheduledCallback); });
                return;
            }

            scheduleOwner.tickCallbacks.AddLast(scheduledCallback);
        }

        public void AddTick(object owner, TimeTypes timeType, Action<float> callback, int callCount)
        {
            ScheduleOwnerVO     scheduleOwner     = GetOrCreateOwner(owner);
            ScheduledCallbackVO scheduledCallback = new ScheduledCallbackVO(timeType, callback, TimeSpan.FromSeconds(0), callCount, _time.GetNow(timeType));
            
            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ scheduleOwner.tickCallbacks.AddLast(scheduledCallback); });
                return;
            }

            scheduleOwner.tickCallbacks.AddLast(scheduledCallback);
        }

        public void AddInterval(object owner, TimeTypes timeType, Action<float> callback, TimeSpan interval)
        {
            ScheduleOwnerVO     scheduleOwner     = GetOrCreateOwner(owner);
            ScheduledCallbackVO scheduledCallback = new ScheduledCallbackVO(timeType, callback, interval, -1, _time.GetNow(timeType));

            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ scheduleOwner.intervalCallbacks.AddLast(scheduledCallback); });
            }

            scheduleOwner.intervalCallbacks.AddLast(scheduledCallback);
        }

        public void AddInterval(object owner, TimeTypes timeType, Action<float> callback, TimeSpan interval, int callCount)
        {
            ScheduleOwnerVO     scheduleOwner     = GetOrCreateOwner(owner);
            ScheduledCallbackVO scheduledCallback = new ScheduledCallbackVO(timeType, callback, interval, callCount, _time.GetNow(timeType));
            
            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ scheduleOwner.intervalCallbacks.AddLast(scheduledCallback); });
            }

            scheduleOwner.intervalCallbacks.AddLast(scheduledCallback);
        }

        public void Pause(object owner, Action<float> callback)
        {
            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ Pause(owner, callback); });
                return;
            }

            ScheduledCallbackVO scheduledCallback = FindCallback(owner, callback);
            if (scheduledCallback != null)
            {
                scheduledCallback.Pause(_time.GetNow(scheduledCallback.timeType));
            }
        }

        public void PauseAllFrom(object owner)
        {
            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ PauseAllFrom(owner); });
                return;
            }

            ScheduleOwnerVO scheduleOwner;
            if (_owners.TryGetValue(owner, out scheduleOwner))
            {
                LinkedListNode<ScheduledCallbackVO> scheduledCallbackNode;

                scheduledCallbackNode = scheduleOwner.tickCallbacks.First;
                while(scheduledCallbackNode != null)
                {
                    scheduledCallbackNode.Value.Pause(_time.GetNow(scheduledCallbackNode.Value.timeType));
                    scheduledCallbackNode = scheduledCallbackNode.Next;
                }
                
                scheduledCallbackNode = scheduleOwner.intervalCallbacks.First;
                while(scheduledCallbackNode != null)
                {
                    scheduledCallbackNode.Value.Pause(_time.GetNow(scheduledCallbackNode.Value.timeType));
                    scheduledCallbackNode = scheduledCallbackNode.Next;
                }
            }
        }

        public void Resume(object owner, Action<float> callback)
        {
            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ Resume(owner, callback); });
                return;
            }

            ScheduledCallbackVO scheduledCallback = FindCallback(owner, callback);
            if (scheduledCallback != null)
            {
                scheduledCallback.Resume(_time.GetNow(scheduledCallback.timeType));
            }
        }

        public void ResumeAllFrom(object owner)
        {
            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ ResumeAllFrom(owner); });
                return;
            }

            ScheduleOwnerVO scheduleOwner;
            if (_owners.TryGetValue(owner, out scheduleOwner))
            {
                LinkedListNode<ScheduledCallbackVO> scheduledCallbackNode;

                scheduledCallbackNode = scheduleOwner.tickCallbacks.First;
                while(scheduledCallbackNode != null)
                {
                    scheduledCallbackNode.Value.Resume(_time.GetNow(scheduledCallbackNode.Value.timeType));
                    scheduledCallbackNode = scheduledCallbackNode.Next;
                }
                
                scheduledCallbackNode = scheduleOwner.intervalCallbacks.First;
                while(scheduledCallbackNode != null)
                {
                    scheduledCallbackNode.Value.Resume(_time.GetNow(scheduledCallbackNode.Value.timeType));
                    scheduledCallbackNode = scheduledCallbackNode.Next;
                }
            }
        }

        public void Remove(object owner, Action<float> callback)
        {
            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ Remove(owner, callback); });
                return;
            }

            ScheduleOwnerVO scheduleOwner;
            if (_owners.TryGetValue(owner, out scheduleOwner))
            {
                LinkedListNode<ScheduledCallbackVO> scheduledCallbackNode;

                scheduledCallbackNode = scheduleOwner.tickCallbacks.First;
                while(scheduledCallbackNode != null)
                {
                    if (scheduledCallbackNode.Value.callback == callback)
                    {
                        scheduleOwner.tickCallbacks.Remove(scheduledCallbackNode);
                        return;
                    }
                    scheduledCallbackNode = scheduledCallbackNode.Next;
                }
                
                scheduledCallbackNode = scheduleOwner.intervalCallbacks.First;
                while(scheduledCallbackNode != null)
                {
                    if (scheduledCallbackNode.Value.callback == callback)
                    {
                        scheduleOwner.intervalCallbacks.Remove(scheduledCallbackNode);
                        return;
                    }
                    scheduledCallbackNode = scheduledCallbackNode.Next;
                }
            }
        }

        public void RemoveAllFrom(object owner)
        {
            if (_isUpdating)
            {
                _postUpdateMethods.AddLast(()=>{ RemoveAllFrom(owner); });
                return;
            }

            ScheduleOwnerVO scheduleOwner;
            if (_owners.TryGetValue(owner, out scheduleOwner))
            {
                scheduleOwner.tickCallbacks.Clear();
                scheduleOwner.intervalCallbacks.Clear();
            }
        }

        private void Update()
        {
            _isUpdating = true;

            foreach (ScheduleOwnerVO scheduleOwner in _owners.Values)
            {
                foreach (ScheduledCallbackVO scheduledCallback in scheduleOwner.tickCallbacks)
                {
                    if (!scheduledCallback.IsPaused)
                    {
                        TimeSpan elapsed = _time.GetDelta(scheduledCallback.timeType);
                        scheduledCallback.Call(elapsed);
                        if (scheduledCallback.RemainingCallCount==0)
                        {
                            Remove(scheduleOwner.owner, scheduledCallback.callback);
                        }
                    }
                }

                foreach (ScheduledCallbackVO scheduledCallback in scheduleOwner.intervalCallbacks)
                {
                    if (!scheduledCallback.IsPaused)
                    {
                        DateTime now     = _time.GetNow(scheduledCallback.timeType);
                        TimeSpan elapsed = now - scheduledCallback.LastCall;
                        if (elapsed >= scheduledCallback.interval)
                        {
                            scheduledCallback.Call(elapsed);
                            if (scheduledCallback.RemainingCallCount==0)
                            {
                                Remove(scheduleOwner.owner, scheduledCallback.callback);
                            }
                        }
                    }
                }
            }
            _isUpdating = false;
                
            LinkedListNode<Action> node = _postUpdateMethods.First;
            while(node!=null)
            {
                node.Value();
                node = node.Next;
            }
            _postUpdateMethods.Clear();
        }

        private ScheduleOwnerVO GetOrCreateOwner(object owner)
        {
            ScheduleOwnerVO scheduleOwner;
            if (_owners.TryGetValue(owner, out scheduleOwner) == false)
            {
                scheduleOwner = new ScheduleOwnerVO(owner);
                _owners.Add(owner, scheduleOwner);
            }
            return scheduleOwner;
        }

        private ScheduledCallbackVO FindCallback(object owner, Action<float> callback)
        {
            ScheduleOwnerVO scheduleOwner;
            if (_owners.TryGetValue(owner, out scheduleOwner) == false)
            {
                return null;
            }

            LinkedListNode<ScheduledCallbackVO> scheduledCallbackNode;

            scheduledCallbackNode = scheduleOwner.tickCallbacks.First;
            while(scheduledCallbackNode != null)
            {
                if (scheduledCallbackNode.Value.callback == callback)
                {
                    return scheduledCallbackNode.Value;
                }
                scheduledCallbackNode = scheduledCallbackNode.Next;
            }
            
            scheduledCallbackNode = scheduleOwner.intervalCallbacks.First;
            while(scheduledCallbackNode != null)
            {
                if (scheduledCallbackNode.Value.callback == callback)
                {
                    return scheduledCallbackNode.Value;
                }
                scheduledCallbackNode = scheduledCallbackNode.Next;
            }

            return null;
        }

    }
}
