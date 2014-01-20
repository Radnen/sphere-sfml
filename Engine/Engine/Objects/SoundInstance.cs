using System;
using Jurassic;
using Jurassic.Library;
using SFML.Audio;

namespace Engine.Objects
{
    public class SoundInstance : ObjectInstance
    {
        private enum SoundType {
            None,
            Sound,
            Music
        }

        string _filename = "";
        SoundType _soundType = SoundType.None;
        Sound _sound = null;
        Music _music = null;
        int _pan, _volume = 255;
        bool _looping = false, _playing = false;

        public SoundInstance(ScriptEngine parent)
            : base (parent)
        {
            PopulateFunctions();
        }

        public SoundInstance(ScriptEngine parent, string filename)
            : base (parent)
        {
            PopulateFunctions();

            string[] sounds = { ".wav", ".flac" };
            string[] music = { ".ogg" };

            if (!System.IO.File.Exists(filename)) {
                _soundType = SoundType.None;
                return;
            }

            _filename = filename;
            string ending = System.IO.Path.GetExtension(filename);
            if (Array.Exists(sounds, x => x == ending))
            {
                _sound = new Sound(new SoundBuffer(filename));
                _soundType = SoundType.Sound;
            }
            else if (Array.Exists(music, x => x == ending))
            {
                _music = new Music(filename);
                _soundType = SoundType.Music;
            }
        }

        [JSFunction(Name = "play")]
        public void Play([DefaultParameterValue(false)] bool repeat = false)
        {
            _looping = repeat;
            if (_soundType == SoundType.Sound)
            {
                _sound.Loop = repeat;
                _sound.Play();
            }
            else if (_soundType == SoundType.Music)
            {
                _music.Loop = repeat;
                _music.Play();
            }
        }

        [JSFunction(Name = "setVolume")]
        public void SetVolume(int volume)
        {
            _volume = volume;
            if (_soundType == SoundType.Sound)
                _sound.Volume = ((float)_volume / 255) * 100f;
            else if (_soundType == SoundType.Music)
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
            if (_soundType == SoundType.Sound)
                _sound.Pitch = (float)value;
            else if (_soundType == SoundType.Music)
                _music.Pitch = (float)value;
        }

        [JSFunction(Name = "getPitch")]
        public double GetPitch()
        {
            if (_soundType == SoundType.Sound)
                return _sound.Pitch;
            else if (_soundType == SoundType.Music)
                return _music.Pitch;
            else
                return 1;
        }

        [JSFunction(Name = "setRepeat")]
        public void SetRepeat(bool val)
        {
            _looping = val;
            if (_soundType == SoundType.Sound)
                _sound.Loop = val;
            else if (_soundType == SoundType.Music)
                _music.Loop = val;
        }

        [JSFunction(Name = "getRepeat")]
        public bool GetRepeat()
        {
            return _looping;
        }

        [JSFunction(Name = "isPlaying")]
        public bool IsPlaying()
        {
            return _playing;
        }

        [JSFunction(Name = "isSeekable")]
        public bool IsSeekable()
        {
            return (_soundType != SoundType.None);
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
            _playing = false;
            if (_soundType == SoundType.Sound)
                _sound.Stop();
            else if (_soundType == SoundType.Music)
                _music.Stop();
        }

        [JSFunction(Name = "pause")]
        public void Pause()
        {
            if (_soundType == SoundType.Sound)
                _sound.Pause();
            else if (_soundType == SoundType.Music)
                _music.Pause();
        }

        [JSFunction(Name = "reset")]
        public void Reset()
        {
            if (_soundType == SoundType.None) return;
            Stop();
            Play(_looping);
        }

        [JSFunction(Name = "getPosition")]
        public double GetPosition()
        {
            if (_soundType == SoundType.Sound)
                return _sound.PlayingOffset.TotalMilliseconds;
            else if (_soundType == SoundType.Music)
                return _music.PlayingOffset.TotalMilliseconds;
            else
                return 0;
        }

        [JSFunction(Name = "setPosition")]
        public void SetPosition(double value)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(value);
            if (_soundType == SoundType.Sound)
                _sound.PlayingOffset = t;
            else if (_soundType == SoundType.Music)
                _music.PlayingOffset = t;
        }

        [JSFunction(Name = "getLength")]
        public double GetLength()
        {
            if (_soundType == SoundType.Sound)
                return (double)_sound.SoundBuffer.Duration;
            else if (_soundType == SoundType.Music) {
                return _music.Duration.TotalMilliseconds;
            }
            else
                return -1;
        }

        [JSFunction(Name = "clone")]
        public SoundInstance Clone()
        {
            SoundInstance instance = new SoundInstance(Engine);
            instance._pan = _pan;
            instance._soundType = _soundType;
            if (_soundType == SoundType.Sound)
                instance._sound = new Sound(_sound);
            else if (_soundType == SoundType.Music)
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

