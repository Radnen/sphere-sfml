using NUnit.Framework;
using System;
using Engine.Objects;

namespace Engine
{
    [TestFixture()]
    public class ColorTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTest();
            Program._engine.Evaluate("var col = CreateColor(0, 0, 0, 255);");
            Program._engine.Evaluate("var col_op = CreateColor(0, 0, 0);");
        }

        [Test()]
        public void TestCreateColor()
        {
            object col = Program._engine.Evaluate("col;");
            Assert.IsInstanceOf<ColorInstance>(col);
        }

        [Test()]
        public void TestColorGetSet()
        {
            Program._engine.Evaluate("col.red   = 50;");
            Program._engine.Evaluate("col.green = 50;");
            Program._engine.Evaluate("col.blue  = 50;");
            Program._engine.Evaluate("col.alpha = 50;");
            object red   = Program._engine.Evaluate("col.red;");
            object green = Program._engine.Evaluate("col.green;");
            object blue  = Program._engine.Evaluate("col.blue;");
            object alpha = Program._engine.Evaluate("col.alpha;");

            Assert.IsInstanceOf<int>(red);
            Assert.IsInstanceOf<int>(green);
            Assert.IsInstanceOf<int>(blue);
            Assert.IsInstanceOf<int>(alpha);
            Assert.AreEqual(red  , 50);
            Assert.AreEqual(green, 50);
            Assert.AreEqual(blue , 50);
            Assert.AreEqual(alpha, 50);
        }

        [Test()]
        public void TestColorOptionalParam()
        {
            object alpha = Program._engine.Evaluate("col_op.alpha;");
            Assert.IsInstanceOf<int>(alpha);
            Assert.AreEqual(alpha, 255);
        }

        [Test()]
        public void TestColorToString()
        {
            object str = Program._engine.Evaluate("col.toString();");
            Assert.IsInstanceOf<string>(str);
            Assert.AreEqual(str, "[object color]");
        }
    }
}

