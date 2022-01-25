using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinmineCheater
{
    internal enum MouseEventFlag : uint
    {
        Move = 0x0001,       // Mouse move
        LeftDown = 0x0002,       // Mouse left button down
        LeftUp = 0x0004,       // Mouse left button up
        RightDown = 0x0008,       // Mouse right button down
        RightUp = 0x0010,       // Mouse right button up
        MiddleDown = 0x0020,       // Mouse wheel down
        MiddleUp = 0x0040,       // Mouse wheel up
        XDown = 0x0080,
        XUp = 0x0100,
        Wheel = 0x0800,       // Mouse wheel scorll
        VirtualDesk = 0x4000,
        Absolute = 0x8000
    }
}

