using NUnit.Framework;
using System;
using Engine.Objects;
using Jurassic.Library;

namespace Engine
{
    [TestFixture()]
    public class FontTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTestEnvironment();
            Program._engine.Evaluate("var font = LoadFont(\"rendered.rfn\");");
        }

        [Test()]
        public void TestLoadFont()
        {
            object func = Program._engine.Evaluate("LoadFont;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object font = Program._engine.Evaluate("font;");
            Assert.IsInstanceOf<FontInstance>(font);
        }

        [Test()]
        public void TestGetSystemFont()
        {
            object func = Program._engine.Evaluate("GetSystemFont;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object font = Program._engine.Evaluate("GetSystemFont();");
            Assert.IsInstanceOf<FontInstance>(font);
        }

        [Test()]
        public void TestFontDrawText()
        {
            object func = Program._engine.Evaluate("font.drawText;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFontGetColorMask()
        {
            object func = Program._engine.Evaluate("font.getColorMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFontSetColorMask()
        {
            object func = Program._engine.Evaluate("font.setColorMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFontDrawTextBox()
        {
            object func = Program._engine.Evaluate("font.drawTextBox;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFontGetHeight()
        {
            object func = Program._engine.Evaluate("font.getHeight;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFontGetStringWidth()
        {
            object func = Program._engine.Evaluate("font.getStringWidth;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFontWordWrapString()
        {
            object func = Program._engine.Evaluate("font.wordWrapString;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFontClone()
        {
            object func = Program._engine.Evaluate("font.clone;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFontSave()
        {
            object func = Program._engine.Evaluate("font.save;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFontToString()
        {
            object str = Program._engine.Evaluate("font.toString();");
            Assert.IsInstanceOf<string>(str);
            Assert.AreEqual(str, "[object font]");
        }
    }
}

