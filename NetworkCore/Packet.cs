using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{
    public enum PacketType : short
    {
        C2S_CREATE_ROOM = 1,
        C2S_ENTER_ROOM,
        C2S_LEAVE_ROOM,

        C2S_EXIT,
    }
}
