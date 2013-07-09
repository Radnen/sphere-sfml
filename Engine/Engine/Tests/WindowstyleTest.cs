using NUnit.Framework;
using System;
using Engine.Objects;
using Jurassic.Library;

namespace Engine
{
    [TestFixture()]
    public class WindowstyleTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            if (Program._engine == null)
                Program._engine = Program.GetSphereEngine();

            Program._engine.Evaluate("var wnd = LoadWindowStyle(\"test.rws\");");
        }

        [Test()]
        public void TestLoadWindowStyle()
        {
            object wnd = Program._engine.Evaluate("wnd;");
            Assert.IsInstanceOf<WindowStyleInstance>(wnd);
        }

        [Test()]
        public void TestWindowStyleSetColorMask()
        {
            object func = Program._engine.Evaluate("wnd.setColorMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestWindowStyleDrawWindow()
        {
            object func = Program._engine.Evaluate("wnd.drawWindow;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestWindowStyleClone()
        {
            object func = Program._engine.Evaluate("wnd.clone;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object wnd = Program._engine.Evaluate("wnd.clone();");
            Assert.IsInstanceOf<WindowStyleInstance>(func);
        }

        [Test()]
        public void TestWindowStyleSave()
        {
            object func = Program._engine.Evaluate("wnd.save;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestWindowStyleGetColorMask()
        {
            object func = Program._engine.Evaluate("wnd.getColorMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object col = Program._engine.Evaluate("wnd.getColorMask();");
            Assert.IsInstanceOf<ColorInstance>(col);
        }

        [Test()]
        public void TestWindowStyleToString()
        {
            object func = Program._engine.Evaluate("wnd.toString;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object str = Program._engine.Evaluate("wnd.toString();");
            Assert.IsInstanceOf<string>(str);
            Assert.AreEqual(str, "[object windowstyle]");
        }

        [Test()]
        public void TestGetSystemWindowStyle()
        {
            object func = Program._engine.Evaluate("GetSystemWindowStyle;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object obj = Program._engine.Evaluate("GetSystemWindowStyle();");
            Assert.IsInstanceOf<WindowStyleInstance>(obj);
        }
    }
}

