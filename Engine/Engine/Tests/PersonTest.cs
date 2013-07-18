using NUnit.Framework;
using System;
using Jurassic;
using Jurassic.Library;

namespace Engine
{
    [TestFixture()]
    public class PersonTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTestEnvironment();
        }

        [Test()]
        public void TestCreatePerson()
        {
            object func = Program._engine.Evaluate("CreatePerson;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestDestroyPerson()
        {
            object func = Program._engine.Evaluate("DestroyPerson;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFollowPerson()
        {
            object func = Program._engine.Evaluate("FollowPerson;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonList()
        {
            object func = Program._engine.Evaluate("GetPersonList;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonX()
        {
            object func = Program._engine.Evaluate("SetPersonX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonY()
        {
            object func = Program._engine.Evaluate("SetPersonY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonX()
        {
            object func = Program._engine.Evaluate("GetPersonX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonY()
        {
            object func = Program._engine.Evaluate("GetPersonY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonXFloat()
        {
            object func = Program._engine.Evaluate("SetPersonXFloat;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonYFloat()
        {
            object func = Program._engine.Evaluate("SetPersonYFloat;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonXFloat()
        {
            object func = Program._engine.Evaluate("GetPersonXFloat;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonYFloat()
        {
            object func = Program._engine.Evaluate("GetPersonYFloat;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonXY()
        {
            object func = Program._engine.Evaluate("SetPersonXY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonXYFloat()
        {
            object func = Program._engine.Evaluate("SetPersonXYFloat;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonLayer()
        {
            object func = Program._engine.Evaluate("GetPersonLayer;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonLayer()
        {
            object func = Program._engine.Evaluate("GetPersonLayer;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonDirection()
        {
            object func = Program._engine.Evaluate("SetPersonDirection;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonDirection()
        {
            object func = Program._engine.Evaluate("GetPersonDirection;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonFrame()
        {
            object func = Program._engine.Evaluate("SetPersonFrame;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonFrame()
        {
            object func = Program._engine.Evaluate("GetPersonFrame;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestQueuePersonCommand()
        {
            object func = Program._engine.Evaluate("QueuePersonCommand;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsCommandQueueEmpty()
        {
            object func = Program._engine.Evaluate("IsCommandQueueEmpty;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestClearPersonCommands()
        {
            object func = Program._engine.Evaluate("ClearCommandQueue;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestQueuePersonScript()
        {
            object func = Program._engine.Evaluate("QueuePersonScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonSpriteset()
        {
            object func = Program._engine.Evaluate("GetPersonSpriteset;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonSpriteset()
        {
            object func = Program._engine.Evaluate("SetPersonSpriteset;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonData()
        {
            object func = Program._engine.Evaluate("GetPersonData;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonData()
        {
            object func = Program._engine.Evaluate("SetPersonData;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
        [Test()]
        public void TestGetPersonValue()
        {
            object func = Program._engine.Evaluate("GetPersonValue;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonValue()
        {
            object func = Program._engine.Evaluate("SetPersonValue;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestCallPersonScript()
        {
            object func = Program._engine.Evaluate("CallPersonScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonScript()
        {
            object func = Program._engine.Evaluate("SetPersonScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonFrameRevert()
        {
            object func = Program._engine.Evaluate("GetPersonFrameRevert;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonFrameRevert()
        {
            object func = Program._engine.Evaluate("SetPersonFrameRevert;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonSpeed()
        {
            object func = Program._engine.Evaluate("SetPersonSpeed;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
        [Test()]
        public void TestSetPersonSpeedXY()
        {
            object func = Program._engine.Evaluate("SetPersonSpeedXY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonSpeedX()
        {
            object func = Program._engine.Evaluate("GetPersonSpeedX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonSpeedY()
        {
            object func = Program._engine.Evaluate("GetPersonSpeedY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonMask()
        {
            object func = Program._engine.Evaluate("SetPersonMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
        [Test()]
        public void TestGetPersonMask()
        {
            object func = Program._engine.Evaluate("GetPersonMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonAngle()
        {
            object func = Program._engine.Evaluate("SetPersonAngle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonAngle()
        {
            object func = Program._engine.Evaluate("GetPersonAngle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonOffsetX()
        {
            object func = Program._engine.Evaluate("SetPersonOffsetX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonOffsetY()
        {
            object func = Program._engine.Evaluate("SetPersonOffsetY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonOffsetX()
        {
            object func = Program._engine.Evaluate("GetPersonOffsetX;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonOffsetY()
        {
            object func = Program._engine.Evaluate("GetPersonOffsetY;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonScaleFactor()
        {
            object func = Program._engine.Evaluate("SetPersonScaleFactor;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonScaleAbsolute()
        {
            object func = Program._engine.Evaluate("SetPersonScaleAbsolute;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsPersonVisible()
        {
            object func = Program._engine.Evaluate("IsPersonVisible;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonVisible()
        {
            object func = Program._engine.Evaluate("SetPersonVisible;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonBase()
        {
            object func = Program._engine.Evaluate("GetPersonBase;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsPersonObstructed()
        {
            object func = Program._engine.Evaluate("IsPersonObstructed;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIgnoreTileObstructions()
        {
            object func = Program._engine.Evaluate("IgnoreTileObstructions;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsIgnoringTileObstructions()
        {
            object func = Program._engine.Evaluate("IsIgnoringTileObstructions;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetObstructingTile()
        {
            object func = Program._engine.Evaluate("GetObstructingTile;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetObstructingPerson()
        {
            object func = Program._engine.Evaluate("GetObstructingPerson;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIgnorePersonObstructions()
        {
            object func = Program._engine.Evaluate("IgnorePersonObstructions;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestIsIgnoringPersonObstructions()
        {
            object func = Program._engine.Evaluate("IsIgnoringPersonObstructions;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetPersonIgnoreList()
        {
            object func = Program._engine.Evaluate("GetPersonIgnoreList;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetPersonIgnoreList()
        {
            object func = Program._engine.Evaluate("SetPersonIgnoreList;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetTalkDistance()
        {
            object func = Program._engine.Evaluate("SetTalkDistance;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetTalkDistance()
        {
            object func = Program._engine.Evaluate("GetTalkDistance;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetCurrentPerson()
        {
            object func = Program._engine.Evaluate("GetCurrentPerson;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

