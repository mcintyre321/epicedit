﻿#region GPL statement
/*Epic Edit is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 3 of the License.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.*/
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Scenery;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Provides the ability to paint the graphics of a track background.
    /// </summary>
    internal sealed class BackgroundDrawer : IDisposable
    {
        public const int Zoom = 2;

        private int FrontWidth
        {
            get { return BackgroundLayout.FrontLayerWidth * Tile.Size; }
        }

        private int BackWidth
        {
            get { return BackgroundLayout.BackLayerWidth * Tile.Size; }
        }

        private int Height
        {
            get { return BackgroundLayout.RowCount * Tile.Size; }
        }

        private Theme theme;
        private Bitmap frontLayer;
        private Bitmap backLayer;

        private int x;

        public BackgroundDrawer()
        {
            // The following members are initialized so they can be disposed of
            // in each function without having to check if they're null beforehand
            this.frontLayer = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
            this.backLayer = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        public void LoadTheme(Theme theme)
        {
            this.theme = theme;
            this.InitFrontLayer();
            this.InitBackLayer();
        }

        public void RewindPreview()
        {            
            this.x = 0;
        }

        private void InitFrontLayer()
        {
            this.frontLayer.Dispose();
            this.frontLayer = this.CreateLayer(true);
        }

        private void InitBackLayer()
        {
            this.backLayer.Dispose();
            this.backLayer = this.CreateLayer(false);
        }

        private Bitmap CreateLayer(bool front)
        {
            int layerWidth = front ? BackgroundLayout.FrontLayerWidth : BackgroundLayout.BackLayerWidth;
            int imageWidth = layerWidth * Tile.Size;

            Bitmap bitmap = new Bitmap(imageWidth, this.Height, PixelFormat.Format32bppPArgb);
            Background background = this.theme.Background;

            Dictionary<int, Bitmap> tileCache = new Dictionary<int, Bitmap>();

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (!front)
                {
                    g.Clear(this.theme.BackColor);
                }

                for (int y = 0; y < BackgroundLayout.RowCount; y++)
                {
                    for (int x = 0; x < layerWidth; x++)
                    {
                        byte tileId;
                        byte properties;
                        this.theme.Background.Layout.GetTileData(x, y, front, out tileId, out properties);
                        int key = (tileId << 8) + properties;
   
                        if (!tileCache.ContainsKey(key))
                        {
                            Tile2bpp tile = this.theme.Background.Tileset[tileId];
                            tileCache.Add(key, Background.GetTileBitmap(tile, properties, front));
                        }

                        g.DrawImage(tileCache[key], x * Tile.Size, y * Tile.Size);
                    }
                }
            }

            foreach (Bitmap cacheItem in tileCache.Values)
            {
                cacheItem.Dispose();
            }

            return bitmap;
        }

        public void DrawBackgroundLayer(Graphics g, int x, bool front)
        {
            if (front)
            {
                this.DrawFrontBackgroundLayer(g, x);
            }
            else
            {
                this.DrawBackBackgroundLayer(g, x);
            }
        }

        private void DrawBackBackgroundLayer(Graphics g, int x)
        {
            using (Bitmap image = new Bitmap(this.BackWidth, this.Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                backBuffer.DrawImage(this.backLayer, x, 0);
                this.DrawImage(g, image, this.BackWidth);
            }
        }

        private void DrawFrontBackgroundLayer(Graphics g, int x)
        {
            using (Bitmap image = new Bitmap(this.FrontWidth, this.Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                backBuffer.Clear(this.theme.BackColor);
                backBuffer.DrawImage(this.frontLayer, x, 0);
                this.DrawImage(g, image, this.FrontWidth);
            }
        }

        public void DrawBackgroundPreview(Graphics g)
        {
            using (Bitmap image = new Bitmap(this.FrontWidth, this.Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                backBuffer.DrawImage(this.backLayer, x, 0);
                backBuffer.DrawImage(this.backLayer, x + BackgroundLayout.BackLayerWidth * Tile.Size, 0);

                backBuffer.DrawImage(this.frontLayer, x * 2, 0);
                backBuffer.DrawImage(this.frontLayer, x * 2 + this.FrontWidth, 0);

                this.DrawImage(g, image, this.FrontWidth);
            }
        }

        private void DrawImage(Graphics g, Bitmap image, int width)
        {
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(image, 0, 0, width * Zoom, this.Height * Zoom);
        }

        public void IncrementPreviewFrame()
        {
            this.x--;

            if (this.x < 512 - (BackgroundLayout.FrontLayerWidth * Tile.Size))
            {
                this.x = 0;
            }
        }

        public void Dispose()
        {
            this.frontLayer.Dispose();
            this.backLayer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}