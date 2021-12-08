using System;
using System.Collections.Generic;
using UnityEngine;


namespace Com.Bit34Games.Unity.Update
{
    public static class UpdateManager
    {
        //  MEMBERS
        private static UpdateManagerComponent         _component;
        private static object                         _defaultOwner;
        private static LinkedList<UpdateCallbackData> _callbacks;
        private static bool                           _isUpdating;
        private static LinkedList<Action>             _postUpdateMethods;


        //  METHODS
        public static DateTime GetNow(UpdateTimeTypes timeType)
        {
            if (timeType == UpdateTimeTypes.Utc)
            {
                return DateTime.UtcNow;
            }

            if (timeType == UpdateTimeTypes.UnityScaled)
            {
                return DateTime.MinValue.Add(TimeSpan.FromSeconds(Time.time));
            }

            if (timeType == UpdateTimeTypes.UnityUnscaled)
            {
                return DateTime.MinValue.Add(TimeSpan.FromSeconds(Time.unscaledTime));
            }
            throw new Exception("Not implemented");
        }

        public static void Add(Action              callback,
                               object              owner,
                               TimeSpan?           interval = null,
                               UpdateTimeTypes     timeType = UpdateTimeTypes.Utc,
                               UpdateCallbackTypes callbackType = UpdateCallbackTypes.MonoBehaviourUpdate)
        {
            if (_component == null) { Init(); }

            if (owner == null) { owner = _defaultOwner; }
            TimeSpan           delay        = interval==null ? TimeSpan.Zero : (TimeSpan)interval;
            DateTime           now          = GetNow(timeType);
            UpdateCallbackData callbackData = new UpdateCallbackData(owner, timeType, callbackType, callback, delay, now, false);

            if(!_isUpdating)
            {
                AddCallback(callbackData);
            }
            else
            {
                _postUpdateMethods.AddLast(()=>
                {
                    AddCallback(callbackData);
                });
            }
        }

        public static void AddOnce(Action              callback,
                                   object              owner = null,
                                   TimeSpan?           delay = null,
                                   UpdateTimeTypes     timeType = UpdateTimeTypes.Utc,
                                   UpdateCallbackTypes callbackType = UpdateCallbackTypes.MonoBehaviourUpdate)
        {
            if (_component == null) { Init(); }

            if (owner == null) { owner = _defaultOwner; }
            TimeSpan           theDelay     = delay==null ? TimeSpan.Zero : (TimeSpan)delay;
            DateTime           now          = GetNow(timeType);
            UpdateCallbackData callbackData = new UpdateCallbackData(owner, timeType, callbackType, callback, theDelay, now, true);

            if(!_isUpdating)
            {
                AddCallback(callbackData);
            }
            else
            {
                _postUpdateMethods.AddLast(()=>
                {
                    AddCallback(callbackData);
                });
            }
        }

        public static void PauseAllFrom(object owner)
        {
            if (_component == null) { Init(); }

            DateTime utcNow           = GetNow(UpdateTimeTypes.Utc);
            DateTime unityScaledNow   = GetNow(UpdateTimeTypes.UnityScaled);
            DateTime unityUnscaledNow = GetNow(UpdateTimeTypes.UnityUnscaled);

            if(!_isUpdating)
            {
                PauseCallbacksFrom(owner, utcNow, unityScaledNow, unityUnscaledNow);
            }
            else
            {
                _postUpdateMethods.AddLast(()=>
                {
                    PauseCallbacksFrom(owner, utcNow, unityScaledNow, unityUnscaledNow);
                });
            }
        }

        public static void ResumeAllFrom(object owner)
        {
            if (_component == null) { Init(); }

            DateTime utcNow           = GetNow(UpdateTimeTypes.Utc);
            DateTime unityScaledNow   = GetNow(UpdateTimeTypes.UnityScaled);
            DateTime unityUnscaledNow = GetNow(UpdateTimeTypes.UnityUnscaled);

            if(!_isUpdating)
            {
                ResumeCallbacksFrom(owner, utcNow, unityScaledNow, unityUnscaledNow);
            }
            else
            {
                _postUpdateMethods.AddLast(()=>
                {
                    ResumeCallbacksFrom(owner, utcNow, unityScaledNow, unityUnscaledNow);
                });
            }
        }

        public static void Remove(Action callback)
        {
            if (_component == null) { Init(); }

            if(!_isUpdating)
            {
                RemoveCallback(callback);
            }
            else
            {
                _postUpdateMethods.AddLast(()=>
                {
                    RemoveCallback(callback);
                });
            }
        }

        public static void RemoveAllFrom(object owner)
        {
            if (_component == null) { Init(); }

            if(!_isUpdating)
            {
                RemoveCallbacksFrom(owner);
            }
            else
            {
                _postUpdateMethods.AddLast(()=>
                {
                    RemoveCallbacksFrom(owner);
                });
            }
        }


        private static void Init()
        {
            GameObject updaterObject = new GameObject("[UpdateManager]");
            GameObject.DontDestroyOnLoad(updaterObject);
            _component = updaterObject.AddComponent<UpdateManagerComponent>();
            _component.Init(Update, LateUpdate);

            _callbacks  = new LinkedList<UpdateCallbackData>();

            _defaultOwner = new System.Object();

            _isUpdating = false;
            _postUpdateMethods = new LinkedList<Action>();
        }

        private static void AddCallback(UpdateCallbackData callbackData)
        {
            _callbacks.AddLast(callbackData);
        }

        private static void RemoveCallback(Action callback)
        {
            LinkedListNode<UpdateCallbackData> callbackNode = _callbacks.First;
            while(callbackNode!=null)
            {
                if (callbackNode.Value.callback==callback)
                {
                    _callbacks.Remove(callbackNode);
                    break;
                }
                callbackNode = callbackNode.Next;
            }
        }

        private static void RemoveCallbacksFrom(object owner)
        {
            LinkedListNode<UpdateCallbackData> callbackNode = _callbacks.First;
            while(callbackNode!=null)
            {
                LinkedListNode<UpdateCallbackData> nextNode = callbackNode.Next;
                if (callbackNode.Value.owner==owner)
                {
                    _callbacks.Remove(callbackNode);
                }
                callbackNode = nextNode;
            }
        }

        private static void PauseCallbacksFrom(object owner, DateTime utcNow, DateTime unityScaledNow, DateTime unityUnscaledNow)
        {
            LinkedListNode<UpdateCallbackData> callbackNode = _callbacks.First;
            while(callbackNode!=null)
            {
                UpdateCallbackData callback = callbackNode.Value;
                if (callback.owner==owner)
                {
                    if(callback.timeType==UpdateTimeTypes.Utc)           { callback.Pause(utcNow); }
                    else
                    if(callback.timeType==UpdateTimeTypes.UnityScaled)   { callback.Pause(unityScaledNow); }
                    else
                    if(callback.timeType==UpdateTimeTypes.UnityUnscaled) { callback.Pause(unityUnscaledNow); }
                }
                callbackNode = callbackNode.Next;
            }
        }

        private static void ResumeCallbacksFrom(object owner, DateTime utcNow, DateTime unityScaledNow, DateTime unityUnscaledNow)
        {
            LinkedListNode<UpdateCallbackData> callbackNode = _callbacks.First;
            while(callbackNode!=null)
            {
                UpdateCallbackData callback = callbackNode.Value;
                if (callback.owner==owner)
                {
                    if(callback.timeType==UpdateTimeTypes.Utc)           { callback.Resume(utcNow);           }
                    else
                    if(callback.timeType==UpdateTimeTypes.UnityScaled)   { callback.Resume(unityScaledNow);   }
                    else
                    if(callback.timeType==UpdateTimeTypes.UnityUnscaled) { callback.Resume(unityUnscaledNow); }
                }
                callbackNode = callbackNode.Next;
            }
        }

        private static void Update()
        {
            if (_component != null)
            {
                _isUpdating = true;

                LinkedListNode<UpdateCallbackData> callbackNode = _callbacks.First;
                while(callbackNode!=null)
                {
                    UpdateCallbackData callback = callbackNode.Value;

                    if (callback.callbackType==UpdateCallbackTypes.MonoBehaviourUpdate)
                    {
                        if(!callback.IsPaused)
                        {
                            DateTime now = GetNow(callback.timeType);
                            if(callback.LastCall+callback.delay<now)
                            {
                                callback.Call(now);

                                if(callback.removeAfterCall)
                                {
                                    LinkedListNode<UpdateCallbackData> nextCallbackNode = callbackNode.Next;
                                    _callbacks.Remove(callbackNode);
                                    callbackNode = nextCallbackNode;
                                    continue;
                                }
                            }
                        }
                    }
                    callbackNode = callbackNode.Next;
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
        }

        private static void LateUpdate()
        {
            if (_component != null)
            {
                _isUpdating = true;

                LinkedListNode<UpdateCallbackData> callbackNode = _callbacks.First;
                while(callbackNode!=null)
                {
                    UpdateCallbackData callback = callbackNode.Value;

                    if (callback.callbackType==UpdateCallbackTypes.MonoBehaviourLateUpdate)
                    {
                        if(!callback.IsPaused)
                        {
                            DateTime now = GetNow(callback.timeType);
                            if(callback.LastCall+callback.delay<now)
                            {
                                callback.Call(now);

                                if(callback.removeAfterCall)
                                {
                                    LinkedListNode<UpdateCallbackData> nextCallbackNode = callbackNode.Next;
                                    _callbacks.Remove(callbackNode);
                                    callbackNode = nextCallbackNode;
                                    continue;
                                }
                            }
                        }
                    }
                    callbackNode = callbackNode.Next;
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
        }

    }
}
