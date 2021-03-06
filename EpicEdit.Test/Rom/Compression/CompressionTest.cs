#region GPL statement
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
using EpicEdit.Rom;
using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Compression
{
    [TestFixture]
    internal class CompressionTest : TestBase
    {
        private byte[] romBuffer;
        private Game game;

        public override void Init()
        {
            this.romBuffer = File.ReadRom(Region.US);
            this.game = File.GetGame(Region.US);
        }

        private void CheckCompression(int offset, int expectedSize)
        {
            this.CheckCompression(offset, expectedSize, new FastCompressor());
            this.CheckCompression(offset, expectedSize, new OptimalCompressor());
        }

        private void CheckDoubleCompression(int offset, int expectedSize)
        {
            this.CheckDoubleCompression(offset, expectedSize, new FastCompressor());
            this.CheckDoubleCompression(offset, expectedSize, new OptimalCompressor());
        }

        private void CheckCompression(int offset, int expectedSize, ICompressor compressor)
        {
            byte[] bufferA = Codec.Decompress(File.ReadBlock(this.romBuffer, offset, expectedSize));
            byte[] bufferB = Codec.Decompress(compressor.Compress(bufferA));

            Assert.AreEqual(expectedSize, bufferA.Length);
            Assert.AreEqual(bufferA, bufferB, "(Compressor: " + compressor.GetType().Name + ")");
        }

        private void CheckDoubleCompression(int offset, int expectedSize, ICompressor compressor)
        {
            byte[] bufferA = Codec.Decompress(Codec.Decompress(File.ReadBlock(this.romBuffer, offset, expectedSize)));
            byte[] bufferB = Codec.Decompress(Codec.Decompress(compressor.Compress(compressor.Compress(bufferA))));

            Assert.AreEqual(expectedSize, bufferA.Length);
            Assert.AreEqual(bufferA, bufferB, "(Compressor: " + compressor.GetType().Name + ")");
        }

        private void CheckTrackCompression(int trackGroupId, int trackId)
        {
            this.CheckTrackCompression(trackGroupId, trackId, new FastCompressor());
            this.CheckTrackCompression(trackGroupId, trackId, new OptimalCompressor());
        }

        private void CheckTrackCompression(int trackGroupId, int trackId, ICompressor compressor)
        {
            Track track = this.game.TrackGroups[trackGroupId][trackId];

            byte[] bufferA = track.Map.GetBytes();

            // Test simple compression
            byte[] bufferC1 = compressor.Compress(bufferA);
            byte[] bufferB = Codec.Decompress(bufferC1, 0, 16384);
            Assert.AreEqual(bufferA, bufferB);

            // Test double compression
            byte[] bufferC2 = compressor.Compress(bufferC1);
            bufferB = Codec.Decompress(Codec.Decompress(bufferC2), 0, 16384);
            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestGhostPillarGraphics()
        {
            this.CheckCompression(0, 0x800);
        }

        [Test]
        public void TestMontyMoleGraphics()
        {
            this.CheckCompression(0x5D6, 0x720);
        }

        [Test]
        public void TestWinnerFlagGraphics()
        {
            this.CheckCompression(0xBB7, 0x700);
        }

        [Test]
        public void TestThwompGraphics()
        {
            this.CheckCompression(0x1070, 0x720);
        }

        [Test]
        public void TestLakituGraphics()
        {
            this.CheckCompression(0x10000, 0x1380);
        }

        [Test]
        public void TestPiranhaPlantGraphics()
        {
            this.CheckCompression(0x10AA5, 0x720);
        }

        [Test]
        public void TestPipeGraphics()
        {
            this.CheckCompression(0x10F9B, 0x720);
        }

        [Test]
        public void TestChompGraphics()
        {
            this.CheckCompression(0x60000, 0x280);
        }

        [Test]
        public void TestPodiumGraphics()
        {
            this.CheckDoubleCompression(0x737DA, 0x4A00);
        }

        [Test]
        public void TestTrackMap1()
        {
            this.CheckTrackCompression(0, 0);
        }

        [Test]
        public void TestTrackMap2()
        {
            this.CheckTrackCompression(0, 1);
        }

        [Test]
        public void TestTrackMap3()
        {
            this.CheckTrackCompression(0, 2);
        }

        [Test]
        public void TestTrackMap4()
        {
            this.CheckTrackCompression(0, 3);
        }

        [Test]
        public void TestTrackMap5()
        {
            this.CheckTrackCompression(0, 4);
        }

        [Test]
        public void TestTrackMap6()
        {
            this.CheckTrackCompression(1, 0);
        }

        [Test]
        public void TestTrackMap7()
        {
            this.CheckTrackCompression(1, 1);
        }

        [Test]
        public void TestTrackMap8()
        {
            this.CheckTrackCompression(1, 2);
        }

        [Test]
        public void TestTrackMap9()
        {
            this.CheckTrackCompression(1, 3);
        }

        [Test]
        public void TestTrackMap10()
        {
            this.CheckTrackCompression(1, 4);
        }

        [Test]
        public void TestTrackMap11()
        {
            this.CheckTrackCompression(2, 0);
        }

        [Test]
        public void TestTrackMap12()
        {
            this.CheckTrackCompression(2, 1);
        }

        [Test]
        public void TestTrackMap13()
        {
            this.CheckTrackCompression(2, 2);
        }

        [Test]
        public void TestTrackMap14()
        {
            this.CheckTrackCompression(2, 3);
        }

        [Test]
        public void TestTrackMap15()
        {
            this.CheckTrackCompression(2, 4);
        }

        [Test]
        public void TestTrackMap16()
        {
            this.CheckTrackCompression(3, 0);
        }

        [Test]
        public void TestTrackMap17()
        {
            this.CheckTrackCompression(3, 1);
        }

        [Test]
        public void TestTrackMap18()
        {
            this.CheckTrackCompression(3, 2);
        }

        [Test]
        public void TestTrackMap19()
        {
            this.CheckTrackCompression(3, 3);
        }

        [Test]
        public void TestTrackMap20()
        {
            this.CheckTrackCompression(3, 4);
        }

        [Test]
        public void TestTrackMap21()
        {
            this.CheckTrackCompression(4, 0);
        }

        [Test]
        public void TestTrackMap22()
        {
            this.CheckTrackCompression(4, 1);
        }

        [Test]
        public void TestTrackMap23()
        {
            this.CheckTrackCompression(4, 2);
        }

        [Test]
        public void TestTrackMap24()
        {
            this.CheckTrackCompression(4, 3);
        }
    }
}
