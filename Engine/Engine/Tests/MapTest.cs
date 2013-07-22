using NUnit.Framework;
using System;
using Jurassic.Library;

namespace Engine
{
    [TestFixture()]
    public class MapTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTestEnvironment();
        }

        [Test()]
        public void TestMapEngine()
        {
            object func = Program._engine.Evaluate("MapEngine;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsMapEngineRunning()
        {
            object func = Program._engine.Evaluate("IsMapEngineRunning;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestUpdateMapEngine()
        {
            object func = Program._engine.Evaluate("UpdateMapEngine;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRenderMap()
        {
            object func = Program._engine.Evaluate("RenderMap;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestChangeMap()
        {
            object func = Program._engine.Evaluate("ChangeMap;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetUpdateScript()
        {
            object func = Program._engine.Evaluate("SetUpdateScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetRenderScript()
        {
            object func = Program._engine.Evaluate("SetRenderScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetLayerRenderer()
        {
            object func = Program._engine.Evaluate("SetLayerRenderer;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetCameraX()
        {
            object func = Program._engine.Evaluate("SetCameraX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetCameraY()
        {
            object func = Program._engine.Evaluate("SetCameraY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetCameraX()
        {
            object func = Program._engine.Evaluate("GetCameraX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetCameraY()
        {
            object func = Program._engine.Evaluate("GetCameraY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestAttachInput()
        {
            object func = Program._engine.Evaluate("AttachInput;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestAttachCamera()
        {
            object func = Program._engine.Evaluate("AttachCamera;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestDetachInput()
        {
            object func = Program._engine.Evaluate("DetachInput;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestDetachCamera()
        {
            object func = Program._engine.Evaluate("DetachCamera;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsCameraAttached()
        {
            object func = Program._engine.Evaluate("IsCameraAttached;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsInputAttached()
        {
            object func = Program._engine.Evaluate("IsInputAttached;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetCameraPerson()
        {
            object func = Program._engine.Evaluate("GetCameraPerson;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetInputPerson()
        {
            object func = Program._engine.Evaluate("GetInputPerson;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

