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
        public void TestGetCurrentMap()
        {
            object func = Program._engine.Evaluate("GetCurrentMap;");
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

        [Test()]
        public void TestGetTileWidth()
        {
            object func = Program._engine.Evaluate("GetTileWidth;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetTileHeight()
        {
            object func = Program._engine.Evaluate("GetTileHeight;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetLayerWidth()
        {
            object func = Program._engine.Evaluate("GetLayerWidth;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetLayerHeight()
        {
            object func = Program._engine.Evaluate("GetLayerHeight;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetNumLayers()
        {
            object func = Program._engine.Evaluate("GetNumLayers;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetLayerName()
        {
            object func = Program._engine.Evaluate("GetLayerName;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetLayerWidth()
        {
            object func = Program._engine.Evaluate("SetLayerWidth;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetLayerHeight()
        {
            object func = Program._engine.Evaluate("SetLayerHeight;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetLayerAngle()
        {
            object func = Program._engine.Evaluate("GetLayerAngle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetLayerAngle()
        {
            object func = Program._engine.Evaluate("SetLayerAngle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetLayerMask()
        {
            object func = Program._engine.Evaluate("GetLayerMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetLayerMask()
        {
            object func = Program._engine.Evaluate("SetLayerMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetMapEngineFrameRate()
        {
            object func = Program._engine.Evaluate("GetMapEngineFrameRate;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsLayerReflective()
        {
            object func = Program._engine.Evaluate("IsLayerReflective;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetLayerReflective()
        {
            object func = Program._engine.Evaluate("SetLayerReflective;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetLayerVisible()
        {
            object func = Program._engine.Evaluate("SetLayerVisible;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsLayerVisible()
        {
            object func = Program._engine.Evaluate("IsLayerVisible;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetLayerScaleFactorX()
        {
            object func = Program._engine.Evaluate("SetLayerScaleFactorX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetLayerScaleFactorY()
        {
            object func = Program._engine.Evaluate("SetLayerScaleFactorY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetNumTiles()
        {
            object func = Program._engine.Evaluate("GetNumTiles;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetTile()
        {
            object func = Program._engine.Evaluate("GetTile;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetTile()
        {
            object func = Program._engine.Evaluate("SetTile;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestReplaceTilesOnLayer()
        {
            object func = Program._engine.Evaluate("ReplaceTilesOnLayer;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetTileName()
        {
            object func = Program._engine.Evaluate("GetTileName;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetTileImage()
        {
            object func = Program._engine.Evaluate("GetTileImage;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetTileImage()
        {
            object func = Program._engine.Evaluate("SetTileImage;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetTileSurface()
        {
            object func = Program._engine.Evaluate("SetTileSurface;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetNextAnimatedTile()
        {
            object func = Program._engine.Evaluate("GetNextAnimatedTile;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetNextAnimatedTile()
        {
            object func = Program._engine.Evaluate("SetNextAnimatedTile;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetTileDelay()
        {
            object func = Program._engine.Evaluate("GetTileDelay;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetTileDelay()
        {
            object func = Program._engine.Evaluate("SetTileDelay;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetMapEngineFrameRate()
        {
            object func = Program._engine.Evaluate("SetMapEngineFrameRate;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void SetColorMask()
        {
            object func = Program._engine.Evaluate("SetColorMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetMapEngine()
        {
            object func = Program._engine.Evaluate("GetMapEngine;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestMapSave()
        {
            object func = Program._engine.Evaluate("GetMapEngine().save;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestMapLayerAppend()
        {
            object func = Program._engine.Evaluate("GetMapEngine().layerAppend;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestCallMapScript()
        {
            object func = Program._engine.Evaluate("CallMapScript");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestCallDefaultMapScript()
        {
            object func = Program._engine.Evaluate("CallDefaultMapScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetDefaultMapScript()
        {
            object func = Program._engine.Evaluate("SetDefaultMapScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetDelayScript()
        {
            object func = Program._engine.Evaluate("SetDelayScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestAreZonesAt()
        {
            object func = Program._engine.Evaluate("AreZonesAt;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestExecuteZones()
        {
            object func = Program._engine.Evaluate("ExecuteZones;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetNumZones()
        {
            object func = Program._engine.Evaluate("GetNumZones;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetCurrentZone()
        {
            object func = Program._engine.Evaluate("GetCurrentZone;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestExecuteZoneScript()
        {
            object func = Program._engine.Evaluate("ExecuteZoneScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetZoneX()
        {
            object func = Program._engine.Evaluate("GetZoneX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetZoneY()
        {
            object func = Program._engine.Evaluate("GetZoneY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetZoneWidth()
        {
            object func = Program._engine.Evaluate("GetZoneWidth;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetZoneHeight()
        {
            object func = Program._engine.Evaluate("GetZoneHeight;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetZoneLayer()
        {
            object func = Program._engine.Evaluate("GetZoneLayer;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetZoneLayer()
        {
            object func = Program._engine.Evaluate("SetZoneLayer;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsTriggerAt()
        {
            object func = Program._engine.Evaluate("IsTriggerAt;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestExecuteTrigger()
        {
            object func = Program._engine.Evaluate("ExecuteTrigger;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

