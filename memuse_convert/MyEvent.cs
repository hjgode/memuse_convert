using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace memuse_convert
{
    public delegate void MyHandler1(object sender, MyEvent e);
    public enum MsgType
    {
        progress_max,
        progress,
        info,
        error
    }

    public class MyEvent : EventArgs
    {
        public string message;
        public MsgType msgType;
        public int num = 0;
        public MyEvent(string s)
        {
            msgType = MsgType.info;
        }
        public MyEvent(MsgType mt, string s)
        {
            msgType = mt;
            message = s;
        }
        public MyEvent(MsgType mt, int i)
        {
            msgType = mt;
            num = i;
            message = i.ToString();
        }
    }
}
