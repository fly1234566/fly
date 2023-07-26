using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 附和导线近似平差
{
    class calculate
    {
        static public double G_A(Point p1,Point p2)//计算方位角
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double A = -1;
            if(dy==0)
            {
                if(dx==0)
                {
                    A = -1;
                }
                else
                {
                    if(dx>0)
                    {
                        A = 0;
                    }
                    else
                    {
                        A = Math.PI;
                    }
                }
            }
            else
            {
                A= Math.PI - Math.PI / 2 * Math.Sign(dy) - Math.Atan(dx / dy);
            }
            if (A > Math.PI * 2) 
            {
                A -= Math.PI * 2;
            }
            if (A < 0) 
            {
                A += Math.PI * 2;
            }
            return A;
        }
        static public double JTH(JD j)//角度转弧度
        {
            return (j.d + j.f / 60 + j.m / 3600) * Math.PI / 180;
        }
        static public JD HTJ(double H)//弧度转角度
        {
            JD j = new JD();
            double temp = H * 180 / Math.PI;
            j.d = Math.Floor(temp);
            j.f = Math.Floor(60 * (temp - j.d));
            j.m = 3600 * (temp - j.d - j.f / 60);
            return j;
        }
        static public double JTH2(string j)
        {
            double d = Math.Floor(double.Parse(j));
            double f = Math.Floor((double.Parse(j) - d) * 100);
            double m = ((double.Parse(j) - d) * 100 - f) * 100;
            return (d + f / 60 + m / 3600) * Math.PI / 180;
        }
        static public void JSA(List<Point> konwn_Ps, List<CZ> CZs, List<double> As, ref double fj, bool flag)
        {
            As.Clear();
            As.Add(G_A(konwn_Ps[0], konwn_Ps[1]));
            int i = 0;
            double sum = 0;
            foreach (CZ cz in CZs)
            {
                cz.ZJ = cz.gczs[1].value - cz.gczs[0].value;//前-后=左角
                if (cz.ZJ < 0)
                {
                    cz.ZJ += Math.PI * 2;
                }
                sum += cz.ZJ;
                double A;//方位角
                if (flag)//左角
                {
                    A = cz.ZJ + As[i] - Math.PI;
                    while (A < 0) 
                    {
                        A += Math.PI * 2;
                    }
                    while (A > Math.PI * 2) 
                    {
                        A -= Math.PI * 2;
                    }
                }
                else//右角
                {
                    A = cz.ZJ - As[i] + Math.PI;
                    while (A < 0)
                    {
                        A += Math.PI * 2;
                    }
                    while (A > Math.PI * 2)
                    {
                        A -= Math.PI * 2;
                    }
                }
                As.Add(A);
                i++;
            }
            double temp1 = G_A(konwn_Ps[2], konwn_Ps[3]);
            double temp2 = (As[0] + sum - (CZs.Count) * Math.PI);
            while (temp2 < 0) 
            {
                temp2 += Math.PI * 2;
            }
            while (temp2 > Math.PI * 2) 
            {
                temp2 -= Math.PI * 2;
            }
            fj = temp2-temp1;
            if (Math.Abs(fj)*180/Math.PI*3600 <= 40 * Math.Sqrt(CZs.Count))//注意单位
            {
                double v = -fj / CZs.Count;
                int j = 1;
                foreach(CZ cz in CZs)
                {
                    cz.ZJ = cz.gczs[1].value - cz.gczs[0].value + v;
                    if (cz.ZJ > Math.PI * 2) 
                    {
                        cz.ZJ -= Math.PI * 2;
                    }
                    if (cz.ZJ < 0)
                    {
                        cz.ZJ += Math.PI * 2;
                    }
                    if (flag)
                    {
                        As[j] = cz.ZJ + As[j-1] - Math.PI;
                        while (As[j] < 0)
                        {
                            As[j] += Math.PI * 2;
                        }
                        while (As[j] > Math.PI * 2)
                        {
                            As[j] -= Math.PI * 2;
                        }
                    }//左角
                    else
                    {
                        As[j] = cz.ZJ - As[j-1] + Math.PI;
                        while (As[j] < 0)
                        {
                            As[j] += Math.PI * 2;
                        }
                        while (As[j] > Math.PI * 2)
                        {
                            As[j] -= Math.PI * 2;
                        }
                    }//右角
                    j++;
                }
            }
            else
            {
                return ;
            }
        }
        static public void JSZBZL(List<CZ> CZs, List<double> As, ref double fx, ref double fy, ref double sumS, Point PA, Point PC)
        {
            double sumx = 0, sumy = 0;
            int i = 1;
            for (int k = 0; k < CZs.Count - 1; k++) 
            {
                CZ cz = CZs[k];
                cz.dx = cz.gczs[2].value * Math.Cos(As[i]);
                cz.dy = cz.gczs[2].value * Math.Sin(As[i]);
                sumx += cz.dx;
                sumy += cz.dy;
                sumS += cz.gczs[2].value;
                i++;
            }
            fx = PA.X + sumx - PC.X;
            fy = PA.Y + sumy - PC.Y;
            double fs = Math.Sqrt(Math.Pow(fx, 2) + Math.Pow(fy, 2)) / sumS;
            double fsxc = (double)1 / 5000;
            if (fs < fsxc) 
            {
                //第一个测站传递
                CZs[0].X = PA.X;
                CZs[0].Y = PA.Y;
                for (int j = 1; j < CZs.Count - 1; j++)
                {
                    CZ czj = CZs[j];
                    CZ czi = CZs[j - 1];
                    czi.dx = czi.dx - fx * czi.gczs[2].value / sumS;
                    czi.dy = czi.dy - fy * czi.gczs[2].value / sumS;
                    czj.X = czi.X + czi.dx;
                    czj.Y = czi.Y + czi.dy;
                }
                //最后一个测站坐标
                CZs[CZs.Count - 1].X = PC.X;
                CZs[CZs.Count - 1].Y = PC.Y;
            }
            else
            {
                return;
            }

        }

    }
}
