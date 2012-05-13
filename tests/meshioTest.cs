using System;
using NUnit.Framework;

namespace meshio {

    [TestFixture]
    public class PmxTests {
        [Test]
        public void Load() {
            var pmx=Pmx.Loader.loadFromPath("../resource/初音ミクVer2.pmx");

            Assert.AreEqual("初音ミク", pmx.Name);
            Assert.AreEqual("Miku Hatsune", pmx.EnglishName);
            Assert.AreEqual(
                    "PolyMo用モデルデータ：初音ミク ver.2.3\r\n"
                    +"(物理演算対応モデル)\r\n"
                    +"\r\n"
                    +"モデリング\t：あにまさ氏\r\n"
                    +"データ変換\t：あにまさ氏\r\n"
                    +"Copyright\t：CRYPTON FUTURE MEDIA, INC", 
                    pmx.Comment);
            Assert.AreEqual(
                    "MMD Model: Miku Hatsune ver.2.3\r\n"
                    +"(Physical Model)\r\n"
                    +"\r\n"
                    +"Modeling by\tAnimasa\r\n"
                    +"Converted by\tAnimasa\r\n"
                    +"Copyright\t\tCRYPTON FUTURE MEDIA, INC", 
                    pmx.EnglishComment);

            Assert.AreEqual(12354, pmx.Vertices.Length);
            Assert.AreEqual(22961*3, pmx.Indices.Length);

			Assert.AreEqual(1, pmx.Textures.Length);
			Assert.AreEqual(17, pmx.Materials.Length);
			Assert.AreEqual(140, pmx.Bones.Length);
        }
    }

	[TestFixture]
	public class VmdTests {
        [Test]
        public void Load() {
            var vmd=Vmd.Loader.loadFromPath("../resource/love&joyお面無しver.vmd");

            Assert.AreEqual("Vocaloid Motion Data 0002", vmd.Magic);
            Assert.AreEqual("おかめいど", vmd.ModelName);

			Assert.AreEqual(40675, vmd.BoneMotions.Length);
        }
	}
}

