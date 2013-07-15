using NUnit.Framework;
using System;
using Engine.Objects;
using Jurassic.Library;
using SFML.Graphics;

namespace Engine
{
    [TestFixture()]
    public class SurfaceTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTestEnvironment();
            Program._engine.Evaluate("var surf = CreateSurface(10, 10, CreateColor(255, 255, 255));");
            Program._engine.Evaluate("var r_surf = CreateSurface(10, 10, CreateColor(255, 255, 255));");
        }

        [Test()]
        public void TestCreateSurface()
        {
            object surf = Program._engine.Evaluate("surf;");
            Assert.IsInstanceOf<SurfaceInstance>(surf);
        }

        [Test()]
        public void TestGrabSurface()
        {
            object func = Program._engine.Evaluate("GrabSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object surf = Program._engine.Evaluate("GrabSurface();");
            Assert.IsInstanceOf<SurfaceInstance>(surf);
        }

        [Test()]
        public void TestLoadSurface()
        {
            object func = Program._engine.Evaluate("LoadSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object surf = Program._engine.Evaluate("LoadSurface(\"blockman.png\");");
            Assert.IsInstanceOf<SurfaceInstance>(surf);
        }

        [Test()]
        public void TestSurfaceSetAlpha()
        {
            object func = Program._engine.Evaluate("surf.setAlpha;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceSetBlendMode()
        {
            object func = Program._engine.Evaluate("surf.setBlendMode;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceBlit()
        {
            object func = Program._engine.Evaluate("surf.blit;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceBlitSurface()
        {
            object func = Program._engine.Evaluate("surf.blitSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceBlitImage()
        {
            object func = Program._engine.Evaluate("surf.blitImage;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceBlitMaskSurface()
        {
            object func = Program._engine.Evaluate("surf.blitMaskSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceRotateBlitSurface()
        {
            object func = Program._engine.Evaluate("surf.rotateBlitSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceRotateBlitMaskSurface()
        {
            object func = Program._engine.Evaluate("surf.rotateBlitMaskSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceZoomBlitSurface()
        {
            object func = Program._engine.Evaluate("surf.zoomBlitSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceZoomBlitMaskSurface()
        {
            object func = Program._engine.Evaluate("surf.zoomBlitMaskSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceTransformBlitSurface()
        {
            object func = Program._engine.Evaluate("surf.transformBlitSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceClone()
        {
            object func = Program._engine.Evaluate("surf.clone;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object clone = Program._engine.Evaluate("surf.clone();");
            Assert.IsInstanceOf<SurfaceInstance>(clone);
        }

        [Test()]
        public void TestSurfaceCloneSection()
        {
            object func = Program._engine.Evaluate("surf.cloneSection;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object clone = Program._engine.Evaluate("surf.cloneSection(0, 0, 5, 5);");
            Assert.IsInstanceOf<SurfaceInstance>(clone);
            Assert.AreEqual(((ObjectInstance)clone).GetPropertyValue("width"), 5);
            Assert.AreEqual(((ObjectInstance)clone).GetPropertyValue("height"), 5);
        }

        [Test()]
        public void TestSurfaceCreateImage()
        {
            object func = Program._engine.Evaluate("surf.createImage;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object image = Program._engine.Evaluate("surf.createImage();");
            Assert.IsInstanceOf<ImageInstance>(image);
        }

        [Test()]
        public void TestSurfaceSetPixel()
        {
            object func = Program._engine.Evaluate("surf.setPixel;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceSetGetPixel()
        {
            Program._engine.Evaluate("surf.setPixel(0, 0, CreateColor(0, 0, 0));");
            object value = Program._engine.Evaluate("surf.getPixel(0, 0);");
            Assert.IsInstanceOf<ColorInstance>(value);

            Color col = ((ColorInstance)value).GetColor();

            Assert.AreEqual(col.R, 0);
            Assert.AreEqual(col.G, 0);
            Assert.AreEqual(col.B, 0);
            Assert.AreEqual(col.A, 255);
        }

        [Test()]
        public void TestSurfaceGetPixel()
        {
            object func = Program._engine.Evaluate("surf.getPixel;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object value = Program._engine.Evaluate("surf.getPixel(1, 1);");
            Assert.IsInstanceOf<ColorInstance>(value);

            Color col = ((ColorInstance)value).GetColor();

            Assert.AreEqual(col.R, 255);
            Assert.AreEqual(col.G, 255);
            Assert.AreEqual(col.B, 255);
            Assert.AreEqual(col.A, 255);
        }

        [Test()]
        public void TestSurfaceToString()
        {
            object func = Program._engine.Evaluate("surf.toString;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object str = Program._engine.Evaluate("surf.toString();");
            Assert.IsInstanceOf<string>(str);
            Assert.IsTrue(str.Equals("[object surface]"));
        }

        [Test()]
        public void TestSurfaceWidth()
        {
            object width = Program._engine.Evaluate("surf.width;");
            Assert.IsInstanceOf<int>(width);
            Assert.AreEqual(width, 10);
        }

        [Test()]
        public void TestSurfaceSetWidth()
        {
            object width = Program._engine.Evaluate("surf.width = 5; surf.width;");
            Assert.AreEqual(width, 10);
        }

        [Test()]
        public void TestSurfaceHeight()
        {
            object height = Program._engine.Evaluate("surf.height;");
            Assert.IsInstanceOf<int>(height);
            Assert.AreEqual(height, 10);
        }

        [Test()]
        public void TestSurfaceSetHeight()
        {
            object height = Program._engine.Evaluate("surf.height = 5; surf.height;");
            Assert.AreEqual(height, 10);
        }

        [Test()]
        public void TestSurfaceRectangle()
        {
            object func = Program._engine.Evaluate("surf.rectangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
        
        [Test()]
        public void TestSurfaceReplaceColor()
        {
            object func = Program._engine.Evaluate("r_surf.replaceColor;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            Program._engine.Evaluate("r_surf.replaceColor(CreateColor(255, 255, 255), CreateColor(0, 0, 0));");
            object color = Program._engine.Evaluate("r_surf.getPixel(2, 2);");

            Color col = ((ColorInstance)color).GetColor();

            Assert.AreEqual(col.R, 0);
            Assert.AreEqual(col.G, 0);
            Assert.AreEqual(col.B, 0);
            Assert.AreEqual(col.A, 255);
        }
        
        [Test()]
        public void TestSurfacePointSeries()
        {
            object func = Program._engine.Evaluate("surf.pointSeries;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
        
        [Test()]
        public void TestSurfaceLine()
        {
            object func = Program._engine.Evaluate("surf.line;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
        
        [Test()]
        public void TestSurfaceGradientLine()
        {
            object func = Program._engine.Evaluate("surf.gradientLine;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
        
        [Test()]
        public void TestSurfaceLineSeries()
        {
            object func = Program._engine.Evaluate("surf.lineSeries;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceBezierCurve()
        {
            object func = Program._engine.Evaluate("surf.bezierCurve;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceOutlinedRectangle()
        {
            object func = Program._engine.Evaluate("surf.outlinedRectangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceGradientRectangle()
        {
            object func = Program._engine.Evaluate("surf.gradientRectangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceTriangle()
        {
            object func = Program._engine.Evaluate("surf.triangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceGradientTriangle()
        {
            object func = Program._engine.Evaluate("surf.gradientTriangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfacePolygon()
        {
            object func = Program._engine.Evaluate("surf.polygon;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceOutlinedEllipse()
        {
            object func = Program._engine.Evaluate("surf.outlinedEllipse;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceFilledEllipse()
        {
            object func = Program._engine.Evaluate("surf.filledEllipse;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceOutlinedCircle()
        {
            object func = Program._engine.Evaluate("surf.outlinedCircle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceFilledCircle()
        {
            object func = Program._engine.Evaluate("surf.filledCircle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceGradientCircle()
        {
            object func = Program._engine.Evaluate("surf.gradientCircle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceDrawText()
        {
            object func = Program._engine.Evaluate("surf.drawText;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceDrawZoomedText()
        {
            object func = Program._engine.Evaluate("surf.drawZoomedText;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceDrawTextBox()
        {
            object func = Program._engine.Evaluate("surf.drawTextBox;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceRotate()
        {
            object func = Program._engine.Evaluate("surf.rotate;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceResize()
        {
            object func = Program._engine.Evaluate("surf.resize;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceRescale()
        {
            object func = Program._engine.Evaluate("surf.rescale;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceFlipHorizontally()
        {
            object func = Program._engine.Evaluate("surf.flipHorizontally;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceFlipVertically()
        {
            object func = Program._engine.Evaluate("surf.flipVertically;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceApplyLookup()
        {
            object func = Program._engine.Evaluate("surf.applyLookup;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
   
        [Test()]
        public void TestSurfaceApplyColorFX()
        {
            object func = Program._engine.Evaluate("surf.applyColorFX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSurfaceApplyColorFX4()
        {
            object func = Program._engine.Evaluate("surf.applyColorFX4;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

