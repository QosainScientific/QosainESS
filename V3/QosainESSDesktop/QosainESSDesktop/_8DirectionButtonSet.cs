﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace QosainESSDesktop
{
    public class _8DirectionButtonSet : Panel
    {
        public delegate void ButtonClickHandler(int id);
        public event ButtonClickHandler OnButtonClick;
        public _8DirectionButtonSet()
        {
            DoubleBuffered = true;
        }
        int hoverID = 8;
        bool mouseDown = false;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (mouseDown)
                return;
            hoverID = getID(e.X, e.Y);
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hoverID = 8;
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDown = true;
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseDown = false;

            OnButtonClick?.Invoke(hoverID);
            Invalidate();
        }
        int getID(double cX, double cY)
        {
            double x = cX - Width / 2;
            double y = -cY + Height / 2;
            if (y == 0 && x == 0)
                return 8;
            if (x < Width / 6 && x > -Width / 6)
                if (y < Height / 6 && y > -Height / 6)
                    return 8;
            double angle = Math.Atan2(y, x) * 180 / Math.PI;
            if (angle > 45 && angle < 135)
            {
                if (y > Height / 6 * 2)
                    return 1;
                return 0;
            }
            else if (angle > -135 && angle < -45)
            {
                if (y > -Height / 6 * 2)
                    return 2;
                return 3;
            }
            else if (angle > -45 && angle < 45)
            {
                if (x > Height / 6 * 2)
                    return 7;
                return 6;
            }
            else
            {
                if (x < -Height / 6 * 2)
                    return 5;
                return 4;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            for (int i = 0; i < 8; i++)
                drawButton(g, i, i == hoverID ? (mouseDown ? 2 : 1) : 0);
        }
        void drawButton(Graphics g, int index, int state)
        {
            if (index == 2 || index == 3)
            {
                g.ScaleTransform(1, -1);
                g.TranslateTransform(0, -Height);
            }
            else if (index == 4 || index == 5)
            {
                g.RotateTransform(-90);
                g.TranslateTransform(-Width, 0);
            }
            else if (index >= 6)
            {
                g.RotateTransform(90);
                g.TranslateTransform(0, -Height);
            }
            if (index % 2 == 1)
                DrawUpDownButton(g, 0 * Height / 6, 1 * Height / 6, state, true);
            else if (index % 2 == 0)
                DrawUpDownButton(g, 1 * Height / 6, 2 * Height / 6, state, false);

            g.ResetTransform();
        }
        void DrawUpDownButton(Graphics g, float start, float end, int state, bool two)
        {
            var c = Color.FromArgb(180, 180, 180);
            if (state == 1) // hover
                c = Color.FromArgb(170, 170, 170);
            if (state == 2) // down
                c = Color.FromArgb(120, 120, 120);
            g.FillPolygon(new SolidBrush(c), new PointF[] {
                new PointF(start, start),
                new PointF(Width - start, start),
                new PointF(Width - end, end),
                new PointF(end, end),
            });
            g.DrawPolygon(new Pen(Parent.BackColor, 1), new PointF[] {
                new PointF(start, start),
                new PointF(Width - start, start),
                new PointF(Width - end, end),
                new PointF(end, end),
            });
            if (state == 0)
            {
                g.DrawPolygon(new Pen(Color.Black), MakeArrows(start, end, two));
            }
            else
                g.FillPolygon(new SolidBrush(Color.Black), MakeArrows(start, end, two));
        }
        PointF[] MakeArrows(float start, float end, bool two)
        {
            float sz = 4;
            float ratio = 1.2F;
            if (!two)
                return new PointF[] {
                new PointF(Width / 2, (start + end) / 2 - sz),
                new PointF(Width / 2 + sz, (start + end) / 2 + sz),
                new PointF(Width / 2 - 4, (start + end) / 2 + sz),
            };
            else
                return new PointF[] {
                new PointF(Width / 2, (start + end) / 2 - sz * ratio - sz * ratio),
                new PointF(Width / 2 + sz * ratio, (start + end) / 2),
                new PointF(Width / 2, (start + end) / 2 + sz * ratio - sz * ratio),
                new PointF(Width / 2 + sz * ratio, (start + end) / 2 + sz * ratio * 3.0F - sz * ratio),
                new PointF(Width / 2 - sz * ratio, (start + end) / 2 + sz * ratio * 3.0F - sz * ratio),
                new PointF(Width / 2, (start + end) / 2),
                new PointF(Width / 2 - sz * ratio, (start + end) / 2 + sz * ratio - sz * ratio),
                };
        }
    }
}