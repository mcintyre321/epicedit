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
using System.ComponentModel;
using System.Drawing;
using EpicEdit.Rom.Tracks.Road;

namespace EpicEdit.Rom.Tracks.Start
{
    /// <summary>
    /// The starting position of the drivers on a GP track.
    /// </summary>
    internal class GPStartPosition : INotifyPropertyChanged
    {
        public const int Size = 6;
        public const int Height = 168;
        private const int PixelLimit = TrackMap.Limit * Tile.Size;
        private const int SecondRowMin = -256;
        private const int SecondRowMax = 255;

        public event PropertyChangedEventHandler PropertyChanged;

        private Point location;
        public Point Location
        {
            get { return this.location; }
            set
            {
                int x = value.X;
                int y = value.Y;

                if (this.SecondRowOffset > 0)
                {
                    if (x < Tile.Size)
                    {
                        x = Tile.Size;
                    }
                    else if (x + this.SecondRowOffset > GPStartPosition.PixelLimit)
                    {
                        x = GPStartPosition.PixelLimit - this.SecondRowOffset;
                    }
                }
                else
                {
                    if (x + this.SecondRowOffset < Tile.Size)
                    {
                        x = Tile.Size - this.SecondRowOffset;
                    }
                    else if (x > GPStartPosition.PixelLimit)
                    {
                        x = GPStartPosition.PixelLimit;
                    }
                }

                if (y < Tile.Size)
                {
                    y = Tile.Size;
                }
                else if (y > GPStartPosition.PixelLimit - GPStartPosition.Height)
                {
                    y = GPStartPosition.PixelLimit - GPStartPosition.Height;
                }

                if (this.X != x || this.Y != y)
                {
                    this.location = new Point(x, y);
                    this.OnPropertyChanged(PropertyNames.GPStartPosition.Location);
                }
            }
        }

        private int secondRowOffset;
        public int SecondRowOffset
        {
            get { return this.secondRowOffset; }
            set
            {
                if (this.X + value < Tile.Size)
                {
                    value = Tile.Size - this.X;
                }
                else if (this.X + value > GPStartPosition.PixelLimit)
                {
                    value = GPStartPosition.PixelLimit - this.X;
                }
                else if (value < GPStartPosition.SecondRowMin)
                {
                    value = GPStartPosition.SecondRowMin;
                }
                else if (value > GPStartPosition.SecondRowMax)
                {
                    value = GPStartPosition.SecondRowMax;
                }

                if (this.secondRowOffset != value)
                {
                    this.secondRowOffset = value;
                    this.OnPropertyChanged(PropertyNames.GPStartPosition.SecondRowOffset);
                }
            }
        }

        public int X
        {
            get { return this.location.X; }
        }

        public int Y
        {
            get { return this.location.Y; }
        }

        /// <summary>
        /// Gets the left bound of the GPStartPosition, depending on the <see cref="SecondRowOffset"/>.
        /// </summary>
        public int Left
        {
            get { return this.X + Math.Min(0, this.SecondRowOffset); }
        }

        /// <summary>
        /// Gets the right bound of the GPStartPosition, depending on the <see cref="SecondRowOffset"/>.
        /// </summary>
        public int Right
        {
            get { return this.X + Math.Max(0, this.SecondRowOffset); }
        }

        public GPStartPosition(short x, short y, short secondRowOffset)
        {
            this.Location = new Point(x, y);
            this.SecondRowOffset = secondRowOffset;
        }

        public GPStartPosition(byte[] data)
        {
            this.SetBytes(data);
        }

        public void SetBytes(byte[] data)
        {
            int x = (data[1] << 8) + data[0];
            int y = (data[3] << 8) + data[2];
            this.Location = new Point(x, y);

            int rowOffset = data[4];
            if (data[5] != 0x00)
            {
                // All original tracks have either has 0x00 or 0xFF for the 6th byte,
                // but we would need something more flexible to match the game behavior
                // if the value is neither 0x00 nor 0xFF (something which shouldn't happen).
                rowOffset -= 256;
            }

            this.SecondRowOffset = rowOffset;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Returns the GPStartPosition data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The GPStartPosition bytes.</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[GPStartPosition.Size];
            data[0] = (byte)(this.X & 0xFF);
            data[1] = (byte)((this.X & 0xFF00) >> 8);
            data[2] = (byte)(this.Y & 0xFF);
            data[3] = (byte)((this.Y & 0xFF00) >> 8);

            if (this.SecondRowOffset < 0)
            {
                data[4] = (byte)(this.SecondRowOffset + 256);
                data[5] = 0xFF;
            }
            else
            {
                data[4] = (byte)this.SecondRowOffset;
                data[5] = 0x00;
            }

            return data;
        }

        public bool IntersectsWith(Point point)
        {
            return point.X >= this.Left - Tile.Size &&
                point.X <= this.Right + (Tile.Size - 1) &&
                point.Y >= this.Y - Tile.Size &&
                point.Y <= this.Y + GPStartPosition.Height + (Tile.Size - 1);
        }
    }
}