using System.Collections.Generic;

namespace Domain.Network
{
    public sealed class SynchronizeMap : Dictionary<int, int>
    {
        public int GetInnerId(int serverId)
        {
            foreach ((int innerId, int _)in this)
            {
                if (this[innerId] == serverId)
                {
                    return innerId;
                }
            }

            return -1;
        }
    }
}