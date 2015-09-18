using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace memuse_convert
{
    class memuse : IComparable<memuse>
    {
        public DateTime dt { get; set; }
        public uint procID { get; set; }
        public string procname { get; set; }
        public uint procmem { get; set; }

        public memuse(DateTime d, uint ID, string name, uint memory)
        {
            dt = d;
            procID = ID;
            procname = name;
            procmem = memory;
        }
        public int CompareTo(memuse other)
        {
            return this.procID.CompareTo(other.procID);
        }

        public class Comparer : IEqualityComparer<memuse>
        {
            public bool Equals(memuse x, memuse y)
            {
                return x.procname == y.procname;
            }

            public int GetHashCode(memuse obj)
            {
                unchecked  // overflow is fine
                {
                    int hash = 17;
                    hash = hash * 23 + (obj.procname ?? "").GetHashCode();
                    return hash;
                }
            }
        }
        public class TimeComparer : IEqualityComparer<memuse>
        {
            public bool Equals(memuse x, memuse y)
            {
                return x.dt == y.dt;
            }

            public int GetHashCode(memuse obj)
            {
                unchecked  // overflow is fine
                {
                    int hash = 17;
                    hash = hash * 23 + obj.dt.GetHashCode();
                    return hash;
                }
            }
        }
    }
}
