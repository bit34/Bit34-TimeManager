using System.Collections.Generic;

namespace Com.Bit34Games.Time.VOs
{
    public class ScheduleOwnerVO
    {
        //  MEMBERS
        public readonly object                          owner;
        public readonly LinkedList<ScheduledCallbackVO> tickCallbacks;
        public readonly LinkedList<ScheduledCallbackVO> intervalCallbacks;

        //  CONSTRUCTORS
        public ScheduleOwnerVO(object owner)
        {
            this.owner        = owner;
            tickCallbacks     = new LinkedList<ScheduledCallbackVO>();
            intervalCallbacks = new LinkedList<ScheduledCallbackVO>();
        }
    }
}
