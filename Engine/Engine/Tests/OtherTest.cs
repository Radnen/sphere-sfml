using NUnit.Framework;
using System;
using Jurassic.Library;

namespace Engine
{
    [TestFixture()]
    public class OtherTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTestEnvironment();
        }

        [Test()]
        public void TestGetScreenWidth()
        {
            object func = Program._engine.Evaluate("GetScreenWidth;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object value = Program._engine.Evaluate("GetScreenWidth();");
            Assert.IsInstanceOf<int>(value);
            Assert.AreEqual(value, 320);
        }

        [Test()]
        public void TestGetScreenHeight()
        {
            object func = Program._engine.Evaluate("GetScreenHeight;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object value = Program._engine.Evaluate("GetScreenHeight();");
            Assert.IsInstanceOf<int>(value);
            Assert.AreEqual(value, 240);
        }

        [Test()]
        public void TestAbort()
        {
            object func = Program._engine.Evaluate("Abort;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestExit()
        {
            object func = Program._engine.Evaluate("Exit;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetVersion()
        {
            object func = Program._engine.Evaluate("GetVersion;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetVersionString()
        {
            object func = Program._engine.Evaluate("GetVersionString;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFlipScreen()
        {
            object func = Program._engine.Evaluate("FlipScreen;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetFrameRate()
        {
            object func = Program._engine.Evaluate("SetFrameRate;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetFrameRate()
        {
            object func = Program._engine.Evaluate("GetFrameRate;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetTime()
        {
            object func = Program._engine.Evaluate("GetTime;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestApplyColorMask()
        {
            object func = Program._engine.Evaluate("ApplyColorMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

