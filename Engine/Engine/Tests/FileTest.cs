using NUnit.Framework;
using System;
using Jurassic.Library;
using Engine.Objects;

namespace Engine
{
    [TestFixture()]
    public class FileTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTestEnvironment();
            Program._engine.Evaluate("var file = OpenFile(\"test.sav\");");
            Program._engine.Evaluate("var raw = OpenRawFile(\"raw.dat\");");
        }

        [TestFixtureTearDown()]
        public void Destroy()
        {
            Program._engine.Evaluate("file.close();");
            Program._engine.Evaluate("raw.close();");
        }

        [Test()]
        public void TestGetFileList()
        {
            object func = Program._engine.Evaluate("GetFileList;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestGetDirectoryList()
        {
            object func = Program._engine.Evaluate("GetDirectoryList;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRemoveFile()
        {
            object func = Program._engine.Evaluate("RemoveFile;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRename()
        {
            object func = Program._engine.Evaluate("Rename;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestCreateDirectory()
        {
            object func = Program._engine.Evaluate("CreateDirectory;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRemoveDirectory()
        {
            object func = Program._engine.Evaluate("RemoveDirectory;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestOpenFile()
        {
            object func = Program._engine.Evaluate("OpenFile;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object file = Program._engine.Evaluate("file;");
            Assert.IsInstanceOf<FileInstance>(file);
        }
        [Test()]

        public void TestFileRead()
        {
            object func = Program._engine.Evaluate("file.read;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFileWrite()
        {
            object func = Program._engine.Evaluate("file.write;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFileClose()
        {
            object func = Program._engine.Evaluate("file.close;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFileFlush()
        {
            object func = Program._engine.Evaluate("file.flush;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestFileToString()
        {
            object func = Program._engine.Evaluate("file.toString;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object str = Program._engine.Evaluate("file.toString();");
            Assert.IsInstanceOf<string>(str);
            Assert.AreEqual(str, "[object file]");
        }

        [Test()]
        public void TestOpenRawFile()
        {
            object func = Program._engine.Evaluate("OpenRawFile;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRawFileRead()
        {
            object func = Program._engine.Evaluate("raw.read;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRawFileWrite()
        {
            object func = Program._engine.Evaluate("raw.write;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRawFileClose()
        {
            object func = Program._engine.Evaluate("raw.close;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRawFileGetSize()
        {
            object func = Program._engine.Evaluate("raw.getSize;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRawFileGetPosition()
        {
            object func = Program._engine.Evaluate("raw.getPosition;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRawFileSetPosition()
        {
            object func = Program._engine.Evaluate("raw.setPosition;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestRawFileToString()
        {
            object func = Program._engine.Evaluate("raw.toString;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object str = Program._engine.Evaluate("raw.toString();");
            Assert.IsInstanceOf<string>(str);
            Assert.AreEqual(str, "[object rawfile]");
        }

        [Test()]
        public void TestHashFromFile()
        {
            object func = Program._engine.Evaluate("HashFromFile;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }
    }
}

