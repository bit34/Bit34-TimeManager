#if UNITY_2018_1_OR_NEWER

using UnityEngine;
using Com.Bit34Games.Time.Utilities;


namespace Com.Bit34Games.Time.Unity
{
    
    public class TimeForUnity : MonoBehaviour   
    {
        //  MEMBERS
        public TimeManager Manager { get{ return _timeManager; } }
        //      Private
        private TimeData     _timeData;
        private TimeManager  _timeManager;
        

        //  METHODS
#region Unity callbacks

        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
            _timeData    = new TimeData();
            _timeManager = new TimeManager(_timeData);
        }

        private void Update()
        {
            _timeData.Update();
        }

#endregion

    }
}

#endif
