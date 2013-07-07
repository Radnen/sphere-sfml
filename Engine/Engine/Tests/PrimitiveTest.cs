using NUnit.Framework;
using System;
using Jurassic.Library;

namespace Engine
{
    [TestFixture()]
    public class PrimitiveTest
    {
        [SetUp()]
        public void Init()
        {
            if (Program._engine == null)
                Program._engine = Program.GetSphereEngine();
        }

        [Test()]
        public void TestLine()
        {
            object func = Program._engine.Evaluate("Line;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestLineSeries()
        {
            object func = Program._engine.Evaluate("LineSeries;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGradientLine()
        {
            object func = Program._engine.Evaluate("GradientLine;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestPoint()
        {
            object func = Program._engine.Evaluate("Point;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestPointSeries()
        {
            object func = Program._engine.Evaluate("PointSeries;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRectangle()
        {
            object func = Program._engine.Evaluate("Rectangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGradientRectangle()
        {
            object func = Program._engine.Evaluate("GradientRectangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestOutlinedRectangle()
        {
            object func = Program._engine.Evaluate("OutlinedRectangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestTriangle()
        {
            object func = Program._engine.Evaluate("Triangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGradientTriangle()
        {
            object func = Program._engine.Evaluate("GradientTriangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFilledCircle()
        {
            object func = Program._engine.Evaluate("FilledCircle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestOutlinedCircle()
        {
            object func = Program._engine.Evaluate("OutlinedCircle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGradientCircle()
        {
            object func = Program._engine.Evaluate("GradientCircle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestPolygon()
        {
            object func = Program._engine.Evaluate("Polygon;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFilledComplex()
        {
            object func = Program._engine.Evaluate("FilledComplex;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGradientComplex()
        {
            object func = Program._engine.Evaluate("GradientComplex;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestOutlinedComplex()
        {
            object func = Program._engine.Evaluate("OutlinedComplex;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFilledEllipse()
        {
            object func = Program._engine.Evaluate("FilledEllipse;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestOutlinedEllipse()
        {
            object func = Program._engine.Evaluate("FilledEllipse;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
        
        [Test()]
        public void TestBezierCurve()
        {
            object func = Program._engine.Evaluate("BezierCurve;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

