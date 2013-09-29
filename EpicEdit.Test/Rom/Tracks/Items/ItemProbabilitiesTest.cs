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
using EpicEdit.Rom.Tracks.Items;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Tracks.Items
{
    [TestFixture]
    internal class ItemProbabilitiesTest
    {
        [Test]
        public void TestGetBytes()
        {
            byte[] dataBefore =
            {
                0x06, 0x00, 0x00, 0x0E, 0x16, 0x1C, 0x1E, 0x20, 0x81,
                0x02, 0x00, 0x00, 0x0C, 0x16, 0x18, 0x00, 0x20, 0x81,
                0x0A, 0x00, 0x0F, 0x11, 0x13, 0x1D, 0x1F, 0x00, 0x81,
                0x06, 0x0E, 0x00, 0x16, 0x18, 0x1C, 0x1E, 0x20, 0x80,
                0x02, 0x04, 0x00, 0x0C, 0x16, 0x18, 0x00, 0x20, 0x80,
                0x08, 0x10, 0x12, 0x13, 0x14, 0x17, 0x1F, 0x00, 0x80,
                0x08, 0x00, 0x00, 0x10, 0x16, 0x1C, 0x1E, 0x20, 0x81,
                0x02, 0x00, 0x00, 0x0A, 0x14, 0x16, 0x00, 0x20, 0x81,
                0x0A, 0x00, 0x11, 0x13, 0x15, 0x1D, 0x1F, 0x00, 0x81,
                0x06, 0x0C, 0x00, 0x12, 0x18, 0x1C, 0x1E, 0x20, 0x80,
                0x02, 0x04, 0x00, 0x0A, 0x14, 0x16, 0x00, 0x20, 0x80,
                0x08, 0x10, 0x13, 0x15, 0x17, 0x1D, 0x1F, 0x00, 0x80,
                0x06, 0x0A, 0x00, 0x10, 0x16, 0x1C, 0x1E, 0x20, 0x80,
                0x02, 0x04, 0x00, 0x0E, 0x16, 0x18, 0x00, 0x20, 0x80,
                0x0A, 0x0B, 0x10, 0x12, 0x14, 0x1D, 0x1F, 0x00, 0x80,
                0x06, 0x0A, 0x00, 0x10, 0x16, 0x1C, 0x1E, 0x20, 0x80,
                0x02, 0x04, 0x00, 0x0E, 0x18, 0x1A, 0x00, 0x20, 0x80,
                0x0C, 0x0D, 0x11, 0x13, 0x15, 0x1D, 0x1F, 0x00, 0x80,
                0x06, 0x0C, 0x00, 0x12, 0x18, 0x1C, 0x1E, 0x20, 0x80,
                0x02, 0x04, 0x00, 0x0E, 0x16, 0x18, 0x00, 0x20, 0x80,
                0x0C, 0x10, 0x14, 0x16, 0x18, 0x1D, 0x1F, 0x00, 0x80,
                0x06, 0x00, 0x07, 0x0E, 0x16, 0x1C, 0x00, 0x20, 0x84,
                0x02, 0x00, 0x00, 0x0C, 0x14, 0x16, 0x00, 0x20, 0x84,
                0x0D, 0x00, 0x11, 0x12, 0x13, 0x1F, 0x00, 0x00, 0x84,
                0x06, 0x0E, 0x0F, 0x13, 0x18, 0x1C, 0x00, 0x20, 0x83,
                0x02, 0x04, 0x00, 0x0E, 0x14, 0x16, 0x00, 0x20, 0x83,
                0x0A, 0x12, 0x18, 0x19, 0x1A, 0x1F, 0x00, 0x00, 0x83,
                0x08, 0x00, 0x09, 0x0E, 0x14, 0x1C, 0x00, 0x20, 0x84,
                0x02, 0x00, 0x00, 0x0A, 0x14, 0x16, 0x00, 0x20, 0x84,
                0x0C, 0x00, 0x13, 0x14, 0x15, 0x1F, 0x00, 0x00, 0x84,
                0x06, 0x0C, 0x0D, 0x12, 0x18, 0x1C, 0x00, 0x20, 0x83,
                0x02, 0x04, 0x00, 0x0A, 0x14, 0x16, 0x00, 0x20, 0x83,
                0x0B, 0x0F, 0x15, 0x16, 0x17, 0x1F, 0x00, 0x00, 0x83,
                0x08, 0x0A, 0x0B, 0x10, 0x16, 0x1C, 0x00, 0x20, 0x83,
                0x02, 0x04, 0x00, 0x0E, 0x18, 0x1A, 0x00, 0x20, 0x83,
                0x0C, 0x0E, 0x13, 0x14, 0x15, 0x1F, 0x00, 0x00, 0x83,
                0x08, 0x0A, 0x0B, 0x10, 0x16, 0x1C, 0x00, 0x20, 0x83,
                0x02, 0x04, 0x00, 0x0E, 0x18, 0x1A, 0x00, 0x20, 0x83,
                0x0C, 0x12, 0x17, 0x18, 0x19, 0x1F, 0x00, 0x00, 0x83,
                0x06, 0x0C, 0x0D, 0x12, 0x16, 0x1C, 0x00, 0x20, 0x83,
                0x02, 0x04, 0x00, 0x0E, 0x18, 0x1A, 0x00, 0x20, 0x83,
                0x0C, 0x12, 0x17, 0x18, 0x19, 0x1F, 0x00, 0x00, 0x83,
                0x02, 0x06, 0x08, 0x0A, 0x14, 0x1E, 0x20, 0x00, 0x82
            };

            ItemProbabilities probabilities = new ItemProbabilities(dataBefore);

            byte[] dataAfter = probabilities.GetBytes();

            Assert.AreEqual(dataBefore, dataAfter);
        }
    }
}