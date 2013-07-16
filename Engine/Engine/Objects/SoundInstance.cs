using System;
using Jurassic.Library;
using SFML.Audio;

namespace Engine.Objects
{
    public class SoundInstance : ObjectInstance
    {
        string _filename = "";
        bool _isSound;
        Sound _sound = null;
        Music _music = null;
        int _pan, _volume = 255;

        public SoundInstance(ObjectInstance proto)
            : base (proto)
        {
            PopulateFunctions();
        }

        public SoundInstance(ObjectInstance proto, string filename)
            : base (proto)
        {
            PopulateFunctions();

            _filename = filename;
            if (filename.EndsWith(".wav")) // quick n' dirty
            {
                _sound = new Sound(new SoundBuffer(filename));
                _isSound = true;
            }
            else
            {
                _music = new Music(filename);
                _isSound = false;
            }
        }

        [JSFunction(Name = "play")]
        public void Play(bool repeat = false)
        {
            if (_isSound)
            {
                _sound.Loop = repeat;
                _sound.Play();
            }
            else
            {
                _music.Loop = repeat;
                _music.Play();
            }
        }

        [JSFunction(Name = "setVolume")]
        public void SetVolume(int volume)
        {
            _volume = volume;
            if (_isSound)
                _sound.Volume = ((float)_volume / 255) * 100f;
            else
                _music.Volume = ((float)_volume / 255) * 100f;
        }

        [JSFunction(Name = "getVolume")]
        public int GetVolume()
        {
            return _volume > 255 ? 255 : _volume;
        }

        [JSFunction(Name = "setPitch")]
        public void SetPitch(double value)
        {
            if (_isSound)
                _sound.Pitch = (float)value;
            else
                _music.Pitch = (float)value;
        }

        [JSFunction(Name = "getPitch")]
        public double GetPitch()
        {
            if (_isSound)
                return _sound.Pitch;
            else
                return _music.Pitch;
        }

        [JSFunction(Name = "setRepeat")]
        public void SetRepeat(bool val)
        {
            if (_isSound)
                _sound.Loop = val;
            else
                _music.Loop = val;
        }

        [JSFunction(Name = "getRepeat")]
        public bool GetRepeat()
        {
            if (_isSound)
                return _sound.Loop;
            else
                return _music.Loop;
        }

        [JSFunction(Name = "isPlaying")]
        public bool IsPlaying()
        {
            if (_isSound)
                return _sound.Status == SoundStatus.Playing;
            else
                return _music.Status == SoundStatus.Playing;
        }

        [JSFunction(Name = "isSeekable")]
        public bool IsSeekable()
        {
            return true; // I think all types in OpenAL are?
        }

        [JSFunction(Name = "setPan")]
        public void SetPan(int val)
        {
            _pan = val; // doesn't do anything
        }

        [JSFunction(Name = "getPan")]
        public int GetPan()
        {
            return _pan;
        }

        [JSFunction(Name = "stop")]
        public void Stop()
        {
            if (_isSound)
                _sound.Stop();
            else
                _music.Stop();
        }

        [JSFunction(Name = "pause")]
        public void Pause()
        {
            if (_isSound)
                _sound.Pause();
            else
                _music.Pause();
        }

        [JSFunction(Name = "reset")]
        public void Reset()
        {
            Stop();
            Play();
        }

        [JSFunction(Name = "getPosition")]
        public double GetPosition()
        {
            if (_isSound)
                return _sound.PlayingOffset.TotalMilliseconds;
            else
                return _music.PlayingOffset.TotalMilliseconds;
        }

        [JSFunction(Name = "setPosition")]
        public void SetPosition(double value)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(value);
            if (_isSound)
                _sound.PlayingOffset = t;
            else
                _music.PlayingOffset = t;
        }

        [JSFunction(Name = "getLength")]
        public double GetLength()
        {
            if (_isSound)
                return (double)_sound.SoundBuffer.Duration;
            else
                return _music.Duration.TotalMilliseconds;
        }

        [JSFunction(Name = "clone")]
        public SoundInstance Clone()
        {
            SoundInstance instance = new SoundInstance(Program._engine.Object.InstancePrototype);
            instance._pan = _pan;
            instance._isSound = _isSound;
            if (_isSound)
                instance._sound = new Sound(_sound);
            else
                instance._music = new Music(_filename);
            return instance;
        }

        [JSFunction(Name = "toString")]
        public override string ToString()
        {
            return "[object sound]";
        }
    }
}

