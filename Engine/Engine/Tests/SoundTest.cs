using System;
using Jurassic.Library;
using NUnit.Framework;
using Engine.Objects;

namespace Engine
{
    public class SoundTest
    {
        [TestFixtureSetUp()]
        public void Init()
        {
            Program.SetupTestEnvironment();
            Program._engine.Evaluate("var sound = LoadSound(\"test.ogg\");");
        }

        [Test()]
        public void TestLoadSound()
        {
            object func = Program._engine.Evaluate("sound;");
            Assert.IsInstanceOf<SoundInstance>(func);
        }

        [Test()]
        public void TestSoundPlay()
        {
            object func = Program._engine.Evaluate("sound.play;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundIsPlaying()
        {
            object func = Program._engine.Evaluate("sound.isPlaying;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundPause()
        {
            object func = Program._engine.Evaluate("sound.pause;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundStop()
        {
            object func = Program._engine.Evaluate("sound.stop;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundGetVolume()
        {
            object func = Program._engine.Evaluate("sound.getVolume;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundSetVolume()
        {
            object func = Program._engine.Evaluate("sound.setVolume;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundGetPitch()
        {
            object func = Program._engine.Evaluate("sound.getPitch;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundSetPitch()
        {
            object func = Program._engine.Evaluate("sound.setPitch;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundGetLength()
        {
            object func = Program._engine.Evaluate("sound.getLength;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundGetPosition()
        {
            object func = Program._engine.Evaluate("sound.getPosition;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundSetPosition()
        {
            object func = Program._engine.Evaluate("sound.setPosition;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundReset()
        {
            object func = Program._engine.Evaluate("sound.reset;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundGetRepeat()
        {
            object func = Program._engine.Evaluate("sound.getRepeat;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundSetRepeat()
        {
            object func = Program._engine.Evaluate("sound.setRepeat;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundIsSeekable()
        {
            object func = Program._engine.Evaluate("sound.isSeekable;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundSetPan()
        {
            object func = Program._engine.Evaluate("sound.setPan;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundGetPan()
        {
            object func = Program._engine.Evaluate("sound.getPan;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundClone()
        {
            object func = Program._engine.Evaluate("sound.clone;");
            Assert.IsInstanceOf<FunctionInstance>(func);
        }

        [Test()]
        public void TestSoundToString()
        {
            object func = Program._engine.Evaluate("sound.toString;");
            Assert.IsInstanceOf<FunctionInstance>(func);

            object value = Program._engine.Evaluate("sound.toString();");
            Assert.IsInstanceOf<string>(value);
            Assert.AreEqual(value, "[object sound]");
        }
    }
}

