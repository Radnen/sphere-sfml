using NUnit.Framework;
using System;
using Jurassic.Library;

namespace Engine
{
    [TestFixture()]
    public class InputTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTestEnvironment();
        }

        [Test()]
        public void TestGetKey()
        {
            object func = Program._engine.Evaluate("GetKey;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestAreKeysLeft()
        {
            object func = Program._engine.Evaluate("AreKeysLeft;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object value = Program._engine.Evaluate("AreKeysLeft();");
            Assert.IsInstanceOf<bool>(value);
            Assert.AreEqual(value, false);
        }

        [Test()]
        public void TestIsKeyPressed()
        {
            object func = Program._engine.Evaluate("IsKeyPressed;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestBindKey()
        {
            object func = Program._engine.Evaluate("BindKey;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestUnbindKey()
        {
            object func = Program._engine.Evaluate("UnbindKey;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetKeyString()
        {
            object func = Program._engine.Evaluate("GetKeyString;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetMouseX()
        {
            object func = Program._engine.Evaluate("GetMouseX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetMouseY()
        {
            object func = Program._engine.Evaluate("GetMouseY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetMouseX()
        {
            object func = Program._engine.Evaluate("SetMouseX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetMouseY()
        {
            object func = Program._engine.Evaluate("SetMouseY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsMouseButtonPressed()
        {
            object func = Program._engine.Evaluate("IsMouseButtonPressed;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsJoystickButtonPressed()
        {
            object func = Program._engine.Evaluate("IsJoystickButtonPressed;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

