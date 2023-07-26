using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 附和导线近似平差
{
    struct JD
    {
        public double d, f, m;//度分秒
    }
    class CZ
    {
        public string name;//前后测站
        public List<GCZ>gczs=new List<GCZ>();//观测值
        public double X, Y,ZJ,dy,dx,A;
    } 
    class Point
    {
        public string name;
        public double X, Y;
        public bool known;
    }
    class GCZ
    {
        public string CZ,type;
        public double value;
    }
}
