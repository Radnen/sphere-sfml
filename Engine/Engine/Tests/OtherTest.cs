using NUnit.Framework;
using System;
using Jurassic.Library;
using Engine.Objects;

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
        public void TestRequireScript()
        {
            object func = Program._engine.Evaluate("RequireScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRequireSystemScript()
        {
            object func = Program._engine.Evaluate("RequireSystemScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestEvaluateScript()
        {
            object func = Program._engine.Evaluate("EvaluateScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestEvaluateSystemScript()
        {
            object func = Program._engine.Evaluate("EvaluateSystemScript;");
            Assert.IsInstanceOf<FunctionInstance>(func);
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
        public void TestRestartGame()
        {
            object func = Program._engine.Evaluate("RestartGame;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetClippingRectangle()
        {
            object func = Program._engine.Evaluate("GetClippingRectangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSetClippingRectangle()
        {
            object func = Program._engine.Evaluate("SetClippingRectangle;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGarbageCollect()
        {
            object func = Program._engine.Evaluate("GarbageCollect;");
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
        public void TestGetGameList()
        {
            object func = Program._engine.Evaluate("GetGameList;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestExecuteGame()
        {
            object func = Program._engine.Evaluate("ExecuteGame;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestApplyColorMask()
        {
            object func = Program._engine.Evaluate("ApplyColorMask;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestCreateByteArray()
        {
            object func = Program._engine.Evaluate("CreateByteArray;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object array = Program._engine.Evaluate("CreateByteArray(5);");
            Assert.IsInstanceOf<ByteArrayInstance>(array);
        }

        [Test()]
        public void TestCreateByteArrayFromString()
        {
            object func = Program._engine.Evaluate("CreateByteArrayFromString;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object array = Program._engine.Evaluate("CreateByteArrayFromString(\"hi\");");
            Assert.IsInstanceOf<ByteArrayInstance>(array);
        }

        [Test()]
        public void TestCreateStringFromByteArray()
        {
            object func = Program._engine.Evaluate("CreateStringFromByteArray;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object str = Program._engine.Evaluate("CreateStringFromByteArray(CreateByteArray(5));");
            Assert.IsInstanceOf<string>(str);
        }

        [Test()]
        public void TestCreateStringFromCode()
        {
            object func = Program._engine.Evaluate("CreateStringFromCode;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestByteArrayConcat()
        {
            object size = Program._engine.Evaluate("var bytea = CreateByteArray(5);" +
                "var byteb = CreateByteArray(5);" +
                "bytea.concat(byteb); bytea.getSize();");
            Assert.IsInstanceOf<int>(size);
            Assert.AreEqual(size, 10);
        }

        [Test()]
        public void TestByteArraySplice()
        {
            object size = Program._engine.Evaluate("var bytea = CreateByteArray(5);" +
                "var byteb = bytea.splice(1, 3); byteb.getSize();");
            Assert.IsInstanceOf<int>(size);
            Assert.AreEqual(size, 2);
        }

        [Test()]
        public void TestByteArrayToString()
        {
            object str = Program._engine.Evaluate("CreateByteArray(5).toString();");
            Assert.IsInstanceOf<string>(str);
            Assert.AreEqual(str, "[object bytearray]");
        }

        [Test()]
        public void TestHashByteArray()
        {
            object func = Program._engine.Evaluate("HashByteArray;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

