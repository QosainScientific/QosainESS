﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SPRGUI2
{
    public class RasterView:Panel
    {
        float patternOffsetX = 0, patternOffsetY = 0; 
        float X, Y, rWidth = 40, rHeight = 30, step = 5;
        float maxX = 55;
        float maxY = 55;
        bool noRaster = false;
        public RasterView()
        { DoubleBuffered = true; }
        public void UpdateViewXY(float x, float y)
        {
            if (x == this.X && y == this.Y)
                return;
            this.X = x;
            this.Y = y;
            Invalidate();
        } 
        public void UpdateViewRaster(float rWidth, float rHeight, float step)
        {                                     
            this.rWidth = rWidth;
            this.rHeight = rHeight;
            this.step = step;
            noRaster = false;
            Invalidate();
        }
        public void HideRaster()
        { noRaster = true; Invalidate(); }

        bool inRaster = false;
        public void beginRaster()
        {
            inRaster = true;
            patternOffsetX = X;
            patternOffsetY = Y;
        }
        public void endRaster()
        {
            inRaster = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(BackColor);
            float ppmmX = (Width - 2) / 2 / maxX;
            float ppmmY = (Height - 2) / 2 / maxY;
            g.DrawRectangle(new Pen(ForeColor, 1), Width / 2 - X * ppmmX, Y * ppmmY, maxX * ppmmX, maxY * ppmmY);
            g.FillEllipse(Brushes.Red, Width / 2 - 3, Height / 2 - 4, 6, 6);

            if (noRaster)
                return;
            float xOffset = 0, yOffset = 0;
            if (inRaster)
            {
                xOffset = -(X - patternOffsetX);
                yOffset = Y - patternOffsetY;
            }
            var cRed = Color.FromArgb(92, 35, 35);
            for (float y = 0; y < rHeight; y += step * 2)
            {
                float xS = Width / 2 + xOffset * ppmmX;
                float xE = Width / 2 + xOffset * ppmmX + ppmmX * rWidth;
                float y0 = Height / 2 - y * ppmmY + yOffset * ppmmY;
                float y1 = Height / 2 - y * ppmmY + yOffset * ppmmY - step * ppmmY;
                float y2 = Height / 2 - y * ppmmY + yOffset * ppmmY - 2 * step * ppmmY;
                g.DrawLine(new Pen(cRed, 1), xS, y0, xE, y0);
                if (y + step <= rHeight)
                {
                    g.DrawLine(new Pen(cRed, 1), xE, y0, xE, y1);
                    g.DrawLine(new Pen(cRed, 1), xE, y1, xS, y1);
                }
                if (y + step * 2 <= rHeight)
                {
                    g.DrawLine(new Pen(cRed, 1), xS, y1, xS, y2);
                }
            }
        }
    }
}
