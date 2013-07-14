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
        public void TestIsAnyKeyPressed()
        {
            object func = Program._engine.Evaluate("IsAnyKeyPressed;");
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
        public void TestSetMousePosition()
        {
            object func = Program._engine.Evaluate("SetMousePosition;");
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

        [Test()]
        public void TestGetMouseWheelEvent()
        {
            object func = Program._engine.Evaluate("GetMouseWheelEvent;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetNumMouseWheelEvents()
        {
            object func = Program._engine.Evaluate("GetNumMouseWheelEvents;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetNumJoysticks()
        {
            object func = Program._engine.Evaluate("GetNumJoysticks;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetNumJoystickButtons()
        {
            object func = Program._engine.Evaluate("GetNumJoystickButtons;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetNumJoystickAxes()
        {
            object func = Program._engine.Evaluate("GetNumJoystickAxes;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetJoystickAxis()
        {
            object func = Program._engine.Evaluate("GetJoystickAxis;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetTalkActivationButton()
        {
            object func = Program._engine.Evaluate("GetTalkActivationButton;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetTalkActivationButton()
        {
            object func = Program._engine.Evaluate("SetTalkActivationButton;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetTalkActivationKey()
        {
            object func = Program._engine.Evaluate("GetTalkActivationKey;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetTalkActivationKey()
        {
            object func = Program._engine.Evaluate("SetTalkActivationKey;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

