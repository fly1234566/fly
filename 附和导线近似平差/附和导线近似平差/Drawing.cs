using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace 附和导线近似平差
{
    class Drawing
    {

        static double maxX, maxY, minX, minY;

        static double scale = 1;
        static int margin;
        static bool mousedown = false;
        static System.Drawing.Point endPoint, startPoint;


        public static void Create(List<Point> points, Rectangle rect)//寻找最大最小值以及比例尺，确定画布
        {
            maxX = double.MinValue; maxY = double.MinValue; minX = double.MaxValue; minY = double.MaxValue;
            double dsx = 1, dsy = 1; margin = rect.Width / 5;
            foreach (Point point in points)
            {
                if (point.X > maxX)
                    maxX = point.X;
                if (point.Y > maxY)
                    maxY = point.Y;
                if (point.X < minX)
                    minX = point.X;
                if (point.X < minY)
                    minY = point.Y;
            }

            dsx = Math.Abs(maxX - minX) / (rect.Height - 2 * margin);
            dsy = Math.Abs(maxY - minY) / (rect.Width - 2 * margin);
            scale = Math.Max(dsx, dsy);
        }

        public static void DrawAx(Graphics g, Rectangle rect)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen pb = new Pen(Color.Black, (float)3);
            Pen pbx = new Pen(Color.Black, (float)2);
            Font fb = new Font("宋体", 16, FontStyle.Bold);
            Font fn = new Font("宋体", 10, FontStyle.Bold);
            Font fn1 = new Font("宋体", 8, FontStyle.Bold);
            Brush bb = new SolidBrush(Color.Black);
            Brush bg = new SolidBrush(Color.Gray);
            double tmargin = margin / 2;
            double AX = rect.Height / 6;
            double ax = rect.Height / 36;
            double BL = rect.Height / 10;
            double bl = rect.Height / 40;

            #region 坐标轴
            g.DrawLine(pb, (float)tmargin, (float)(rect.Height - tmargin), (float)tmargin, (float)tmargin);
            g.DrawLine(pb, (float)tmargin, (float)(rect.Height - tmargin), (float)(rect.Width - tmargin), (float)(rect.Height - tmargin));
            double dy = (rect.Height - 2 * tmargin) / 4.0 * scale;
            double dx = (rect.Width - 2 * tmargin) / 6 * scale;
            for (int i = 0; i < 4; i++)
            {
                double y = (rect.Height - tmargin) - (i * dy) / scale;
                g.DrawLine(pbx, (float)tmargin, (float)y, (float)(tmargin - 10), (float)y);
                string st = (minX - ((margin - tmargin) * scale) + i * dy).ToString("0.0");
                g.DrawString(st, fn, bb, (float)(tmargin - 90), (float)(y - 5));
            }
            for (int i = 0; i < 6; i++)
            {
                double x = tmargin + (i * dx) / scale;
                g.DrawLine(pbx, (float)x, (float)(rect.Height - tmargin), (float)x, (float)(rect.Height - tmargin + 10));
                string st = (minY - ((margin - tmargin) * scale) + i * dx).ToString("0.0");
                g.DrawString(st, fn, bb, (float)(x - 5), (float)(rect.Height - tmargin + 15));
            }



            g.DrawLine(pb, (float)(tmargin - ax), (float)(tmargin + ax), (float)tmargin, (float)tmargin);
            g.DrawLine(pb, (float)(tmargin + ax), (float)(tmargin + ax), (float)(tmargin), (float)(tmargin));

            g.DrawLine(pb, (float)(rect.Width - tmargin - ax), (float)(rect.Height - tmargin - ax), (float)(rect.Width - tmargin), (float)(rect.Height - tmargin));
            g.DrawLine(pb, (float)(rect.Width - tmargin - ax), (float)(rect.Height - tmargin + ax), (float)(rect.Width - tmargin), (float)(rect.Height - tmargin));

            g.DrawString("X", fb, bb, (float)tmargin, (float)(tmargin - rect.Height / 36));
            g.DrawString("Y", fb, bb, (float)((rect.Width - tmargin + ax)), (float)(rect.Height - tmargin));

            #endregion

            #region 方向标
            PointF[] pointf = new PointF[5];

            pointf[0].X = (float)(rect.Width - tmargin);
            pointf[0].Y = (float)(tmargin + ax);

            pointf[1].X = (float)(rect.Width - tmargin - ax);
            pointf[1].Y = (float)(tmargin + BL);

            pointf[2].X = (float)(rect.Width - tmargin);
            pointf[2].Y = (float)(tmargin + (AX) / 2);

            pointf[3].X = (float)(rect.Width - tmargin + ax);
            pointf[3].Y = (float)(tmargin + BL);

            pointf[4].X = (float)(rect.Width - tmargin);
            pointf[4].Y = (float)(tmargin + ax);

            g.FillPolygon(bg, pointf);

            g.DrawString("N", fn, bb, (float)(rect.Width - tmargin + ax), (float)(tmargin + ax));
            #endregion

            g.DrawString("附和导线示意图", fb, bb, (float)(rect.Width / 2 - AX), (float)BL);

        }//底图

        public static void DrawPoints(Graphics g, List<CZ> czs, Rectangle rect,bool click)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Brush b = new SolidBrush(Color.Black);
            Pen p = new Pen(Color.Black);
            Font fb = new Font("宋体", 10, FontStyle.Bold);
            List<Point> points = new List<Point>();
            foreach (CZ cz in czs)
            {
                Point pt = new Point();
                pt.X = cz.X;
                pt.Y = cz.Y;
                pt.name = cz.name;
                points.Add(pt);
            }
            foreach (Point point in points)
            {
                double y = (rect.Height - margin) - (point.X - minX) / scale;
                double x = (point.Y - minY) / scale + margin;
                if (x < margin / 2 || y > rect.Height - margin / 2)
                    continue;
                if (x > rect.Width - margin / 2 || y < margin / 2)
                    continue;
                g.DrawEllipse(p, (float)x, (float)y, (float)2, (float)2);
                g.FillEllipse(b, (float)x, (float)y, (float)2, (float)2);
               if(click) g.DrawString((point.name), fb, b, (float)(x +5), (float)(y));

            }
        }//控制点

        public static void DrawYZPoints(Graphics g, List<Point> points, Rectangle rect,bool click)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Brush bb = new SolidBrush(Color.Black);
            Brush br = new SolidBrush(Color.Red);
            Pen pr = new Pen(Color.Red);
            Pen p = new Pen(Color.Black);
            Font fb = new Font("宋体", 10, FontStyle.Bold);
            foreach (Point point in points)
            {
                double y = (rect.Height - margin) - (point.X - minX) / scale;
                double x = (point.Y - minY) / scale + margin;
                if (x < margin / 2 || y > rect.Height - margin / 2)
                    continue;
                if (x > rect.Width - margin / 2 || y < margin / 2)
                    continue;
                PointF[] pointf = new PointF[4];
                pointf[0].X = (float)x;
                pointf[0].Y = (float)(y - 3);
                pointf[1].X = (float)(x + 3);
                pointf[1].Y = (float)(y + 3);
                pointf[2].X = (float)(x - 3);
                pointf[2].Y = (float)(y + 3);
                pointf[3].X = (float)x;
                pointf[3].Y = (float)(y - 3);
                g.DrawPolygon(pr, pointf);
                g.FillPolygon(br, pointf);
               if(click) g.DrawString(point.name, fb, bb, (float)(x + 5), (float)(y));
            }
        }//已知点


        public static void DrawczLines(Graphics g, List<CZ> CZs, Rectangle rect, Color color)//连线
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen pb = new Pen(color, (float)0.8);
            List<Point> points = new List<Point>();
            foreach (CZ cz in CZs)
            {
                Point pt = new Point();
                pt.X = cz.X;
                pt.Y = cz.Y;
                points.Add(pt);
            }
            for (int i = 1; i < points.Count; i++)
            {
                double y = (rect.Height - margin) - (points[i].X - minX) / scale;
                double x = (points[i].Y - minY) / scale + margin;
                double y0 = (rect.Height - margin) - (points[i - 1].X - minX) / scale;
                double x0 = (points[i - 1].Y - minY) / scale + margin;
                if (x < margin / 2 || y > rect.Height - margin / 2)
                    continue;
                if (x > rect.Width - margin / 2 || y < margin / 2)
                    continue;
                if (x0 < margin / 2 || y0 > rect.Height - margin / 2)
                    continue;
                if (x0 > rect.Width - margin / 2 || y0 < margin / 2)
                    continue;
                g.DrawLine(pb, (float)x, (float)y, (float)x0, (float)y0);

            }
        }
        public static void DrawYZLines(Graphics g, List<Point> points, Rectangle rect, Color color)//连线
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen pb = new Pen(color, (float)0.8);
            for (int i = 1; i < 2; i++)
            {
                double y = (rect.Height - margin) - (points[i].X - minX) / scale;
                double x = (points[i].Y - minY) / scale + margin;
                double y0 = (rect.Height - margin) - (points[i - 1].X - minX) / scale;
                double x0 = (points[i - 1].Y - minY) / scale + margin;
                if (x < margin / 2 || y > rect.Height - margin / 2)
                    continue;
                if (x > rect.Width - margin / 2 || y < margin / 2)
                    continue;
                if (x0 < margin / 2 || y0 > rect.Height - margin / 2)
                    continue;
                if (x0 > rect.Width - margin / 2 || y0 < margin / 2)
                    continue;
                g.DrawLine(pb, (float)x, (float)y, (float)x0, (float)y0);
            }
            for (int i = 3; i < 4; i++)
            {
                double y = (rect.Height - margin) - (points[i].X - minX) / scale;
                double x = (points[i].Y - minY) / scale + margin;
                double y0 = (rect.Height - margin) - (points[i - 1].X - minX) / scale;
                double x0 = (points[i - 1].Y - minY) / scale + margin;
                if (x < margin / 2 || y > rect.Height - margin / 2)
                    continue;
                if (x > rect.Width - margin / 2 || y < margin / 2)
                    continue;
                if (x0 < margin / 2 || y0 > rect.Height - margin / 2)
                    continue;
                if (x0 > rect.Width - margin / 2 || y0 < margin / 2)
                    continue;
                g.DrawLine(pb, (float)x, (float)y, (float)x0, (float)y0);
            }
        }

        public static void Delta(MouseEventArgs e)
        {
            if (e.Delta > 0)
                scale *= 0.9;
            if (e.Delta < 0)
                scale *= 1.1;
        }//滑轮滚动

        public static void MouseUp(MouseEventArgs e)//鼠标抬起
        {
            mousedown = false;
        }

        public static void MouseDown(MouseEventArgs e)//鼠标按下
        {
            mousedown = true;
            startPoint.X = e.X;
            startPoint.Y = e.Y;
            endPoint.X = e.X;
            endPoint.Y = e.Y;
        }

        public static void MouseMove(MouseEventArgs e)
        {
            double dx = 0, dy = 0;
            if (mousedown)
            {
                endPoint = e.Location;
                dy = (endPoint.X - startPoint.X) * scale;
                dx = (endPoint.Y - startPoint.Y) * scale;
                minX += dx;
                minY -= dy;
                startPoint = e.Location;
            }
        }//鼠标移动



    }
}

