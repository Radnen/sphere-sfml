using NUnit.Framework;
using Jurassic;
using System;
using Engine;
using Engine.Objects;

namespace Engine
{
    [TestFixture()]
    public class ImageTest
    {
        [SetUp()]
        public void Init()
        {
            if (Program._engine == null)
                Program._engine = Program.GetSphereEngine();

            Program._engine.Evaluate("var img = LoadImage(\"blockman.png\");");
        }

        [Test()]
        public void TestLoadImage()
        {
            object image = Program._engine.Evaluate("img");
            Assert.IsInstanceOf<ImageInstance>(image);
        }

        [Test()]
        public void TestImageWidth()
        {
            object width = Program._engine.Evaluate("img.width;");
            Assert.IsInstanceOf<int>(width);
            Assert.IsTrue(width.Equals(48));
        }

        [Test()]
        public void TestImageHeight()
        {
            object height = Program._engine.Evaluate("img.height;");
            Assert.IsInstanceOf<int>(height);
            Assert.IsTrue(height.Equals(48));
        }

        [Test()]
        public void TestImageBlitBinding()
        {
            object func = Program._engine.Evaluate("img.blit");
            Assert.IsInstanceOf<Jurassic.Library.FunctionInstance>(func);
        }

        [Test()]
        public void TestImageBlitMaskBinding()
        {
            object func = Program._engine.Evaluate("img.blitMask;");
            Assert.IsInstanceOf<Jurassic.Library.FunctionInstance>(func);
        }

        [Test()]
        public void TestImageRotateBlitBinding()
        {
            object func = Program._engine.Evaluate("img.rotateBlit;");
            Assert.IsInstanceOf<Jurassic.Library.FunctionInstance>(func);
        }

        [Test()]
        public void TestImageZoomBlitBinding()
        {
            object func = Program._engine.Evaluate("img.zoomBlit;");
            Assert.IsInstanceOf<Jurassic.Library.FunctionInstance>(func);
        }

        [Test()]
        public void TestImageTransformBlitBinding()
        {
            object func = Program._engine.Evaluate("img.transformBlit;");
            Assert.IsInstanceOf<Jurassic.Library.FunctionInstance>(func);
        }

        [Test()]
        public void TestImageSettingWidth()
        {
            object width = Program._engine.Evaluate("img.width = 5; img.width;");
            Assert.IsInstanceOf<int>(width);
            Assert.IsTrue(width.Equals(48));
        }

        [Test()]
        public void TestImageSettingHeight()
        {
            object height = Program._engine.Evaluate("img.height = 5; img.height;");
            Assert.IsInstanceOf<int>(height);
            Assert.IsTrue(height.Equals(48));
        }

        [Test()]
        public void TestImageToString()
        {
            object str = Program._engine.Evaluate("img.toString();");
            Assert.IsInstanceOf<string>(str);
            Assert.AreEqual(str, "[object image]");
        }
    }
}

