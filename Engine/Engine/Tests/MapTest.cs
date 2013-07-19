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
        public void TestRenderMapEngine()
        {
            object func = Program._engine.Evaluate("RenderMapEngine;");
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
    }
}

