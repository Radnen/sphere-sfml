using System;
using Jurassic;
using Jurassic.Library;
using NUnit.Framework;
using Engine;
using Engine.Objects;

namespace Engine
{
    [TestFixture()]
    public class ImageTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTest();
            Program._engine.Evaluate("var img = LoadImage(\"blockman.png\");");
        }

        [Test()]
        public void TestLoadImage()
        {
            object image = Program._engine.Evaluate("img;");
            Assert.IsInstanceOf<ImageInstance>(image);
        }

        [Test()]
        public void TestGrabImage()
        {
            object func = Program._engine.Evaluate("GrabImage;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object image = Program._engine.Evaluate("GrabImage();");
            Assert.IsInstanceOf<ImageInstance>(image);
        }

        [Test()]
        public void TestImageSave()
        {
            object func = Program._engine.Evaluate("img.save;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestImageCreateSurface()
        {
            object func = Program._engine.Evaluate("img.createSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object image = Program._engine.Evaluate("img.createSurface();");
            Assert.IsInstanceOf<SurfaceInstance>(image);
        }

        [Test()]
        public void TestGetSystemArrow()
        {
            object func = Program._engine.Evaluate("GetSystemArrow;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object image = Program._engine.Evaluate("GetSystemArrow();");
            Assert.IsInstanceOf<SurfaceInstance>(image);
        }

        [Test()]
        public void TestGetSystemUpArrow()
        {
            object func = Program._engine.Evaluate("GetSystemUpArrow;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object image = Program._engine.Evaluate("GetSystemUpArrow();");
            Assert.IsInstanceOf<SurfaceInstance>(image);
        }

        [Test()]
        public void TestGetSystemDownArrow()
        {
            object func = Program._engine.Evaluate("GetSystemDownArrow;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object image = Program._engine.Evaluate("GetSystemDownArrow();");
            Assert.IsInstanceOf<SurfaceInstance>(image);
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
        public void TestImageBlit()
        {
            object func = Program._engine.Evaluate("img.blit");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestImageBlitMask()
        {
            object func = Program._engine.Evaluate("img.blitMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestImageRotateBlit()
        {
            object func = Program._engine.Evaluate("img.rotateBlit;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestImageZoomBlit()
        {
            object func = Program._engine.Evaluate("img.zoomBlit;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestImageTransformBlit()
        {
            object func = Program._engine.Evaluate("img.transformBlit;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestImageTransformBlitMask()
        {
            object func = Program._engine.Evaluate("img.transformBlitMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
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
            object func = Program._engine.Evaluate("img.toString");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object str = Program._engine.Evaluate("img.toString();");
            Assert.IsInstanceOf<string>(str);
            Assert.AreEqual(str, "[object image]");
        }

        [Test()]
        public void TestImageClone()
        {
            object func = Program._engine.Evaluate("img.clone;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object obj = Program._engine.Evaluate("img.clone();");
            Assert.IsInstanceOf<ImageInstance>(obj);
        }
    }
}

