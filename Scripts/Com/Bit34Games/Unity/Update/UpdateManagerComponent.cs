using System;
using UnityEngine;


namespace Com.Bit34Games.Unity.Update
{
    //	MEMBERS
    public class UpdateManagerComponent : MonoBehaviour
    {
        //	MEMBERS
        private Action _updateMethod;
        private Action _lateUpdateMethod;


        //	METHODS
        public void Init(Action updateMethod, Action lateUpdateMethod)
        {
            if(_updateMethod==null)
            {
                _updateMethod     = updateMethod;
                _lateUpdateMethod = lateUpdateMethod;
            }
        }

        void Update()
        {
            _updateMethod();
        }

        void LateUpdate()
        {
            _lateUpdateMethod();
        }
    }
}
