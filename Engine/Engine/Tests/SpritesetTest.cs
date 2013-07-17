using System;
using Jurassic.Library;
using NUnit.Framework;
using Engine.Objects;

namespace Engine
{
    [TestFixture()]
    public class SpritesetTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTestEnvironment();
            Program._engine.Evaluate("var ss = LoadSpriteset(\"test.rss\");");
        }

        [Test()]
        public void TestLoadSpriteset()
        {
            object ss = Program._engine.Evaluate("ss;");
            Assert.IsInstanceOf<SpritesetInstance>(ss);
        }

        [Test()]
        public void TestCreateSpriteset()
        {
            object func = Program._engine.Evaluate("CreateSpriteset;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object ss = Program._engine.Evaluate("CreateSpriteset(16, 16, 1, 1, 1);");
            Assert.IsInstanceOf<SpritesetInstance>(ss);
        }

        [Test()]
        public void TestSpritesetSave()
        {
            object func = Program._engine.Evaluate("ss.save;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSpritesetImages()
        {
            object array = Program._engine.Evaluate("ss.images;");
            Assert.IsInstanceOf<ArrayInstance>(array);

            object image = Program._engine.Evaluate("ss.images[0];");
            Assert.IsInstanceOf<ImageInstance>(image);
        }

        [Test()]
        public void TestSpritesetDirections()
        {
            object array = Program._engine.Evaluate("ss.directions;");
            Assert.IsInstanceOf<ArrayInstance>(array);

            object value = Program._engine.Evaluate("ss.directions[0];");
            Assert.IsInstanceOf<ObjectInstance>(value);
        }

        [Test()]
        public void TestSpritesetDirectionName()
        {
            object value = Program._engine.Evaluate("ss.directions[0].name;");
            Assert.IsInstanceOf<string>(value);
        }

        [Test()]
        public void TestSpritesetFrames()
        {
            object array = Program._engine.Evaluate("ss.directions[0].frames;");
            Assert.IsInstanceOf<ArrayInstance>(array);

            object value = Program._engine.Evaluate("ss.directions[0].frames[0];");
            Assert.IsInstanceOf<ObjectInstance>(value);
        }

        [Test()]
        public void TestSpritesetFrameIndex()
        {
            object value = Program._engine.Evaluate("ss.directions[0].frames[0].index;");
            Assert.IsInstanceOf<int>(value);
        }

        [Test()]
        public void TestSpritesetFrameDelay()
        {
            object value = Program._engine.Evaluate("ss.directions[0].frames[0].delay;");
            Assert.IsInstanceOf<int>(value);
        }

        [Test()]
        public void TestSpritesetClone()
        {
            object func = Program._engine.Evaluate("ss.clone;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object ss = Program._engine.Evaluate("ss.clone();");
            Assert.IsInstanceOf<SpritesetInstance>(ss);
        }

        [Test()]
        public void TestSpritesetToString()
        {
            object func = Program._engine.Evaluate("ss.toString;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object ss = Program._engine.Evaluate("ss.toString();");
            Assert.IsInstanceOf<string>(ss);
            Assert.AreEqual(ss, "[object spriteset]");
        }
    }
}

