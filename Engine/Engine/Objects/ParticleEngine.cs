using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using Jurassic.Library;
using SFML.Graphics;

namespace Engine.Objects
{
    public static class ParticleEngine
    {
        static Particle[] _particles;

        static ParticleEngine()
        {
            Setup(500);
        }

        public static void BindToEngine(ScriptEngine engine)
        {
            engine.SetGlobalFunction("CreateEmitter", new Func<ImageInstance, ColorInstance, double, double, EmitterInstance>(CreateEmitter));
            engine.SetGlobalFunction("UpdateParticles", new Action<double>(Update));
            engine.SetGlobalFunction("RenderParticles", new Action<double, double>(Render));
        }

        public static EmitterInstance CreateEmitter(ImageInstance image, ColorInstance color, double life, double speed)
        {
            return new EmitterInstance(Program._engine, image, color, life, speed);
        }

        public static void Setup(int num)
        {
            _particles = new Particle[num];
            for (int i = 0; i < num; ++i) _particles[i] = new Particle();
        }

        public static void Update(double time)
        {
            for (int i = 0; i < _particles.Length; ++i)
            {
                if (_particles[i].Life > 0) _particles[i].Update(time);
            }
        }

        public static void Render(double x, double y)
        {
            for (int i = 0; i < _particles.Length; ++i)
            {
                if (_particles[i].Life > 0) _particles[i].Render(x, y);
            }
        }

        public static void Emit(EmitterInstance emitter, int times)
        {
            for (int i = 0; i < times; ++i)
            {
                for (int n = 0; n < _particles.Length; ++n)
                {
                    if (_particles[n].Life <= 0) { _particles[n].Setup(emitter); break; }
                }
            }
        }
    }

    public class EmitterInstance : ObjectInstance
    {
        public ImageInstance Image { get; set; }
        public ColorInstance Color { get; set; }
        public int ImgWidth { get; set; }
        public int ImgHeight { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public double Life { get; set; }
        public double Speed { get; set; }
        public double Angle { get; set; }
        public bool Fade { get; set; }

        public EmitterInstance(ScriptEngine engine, ImageInstance image, ColorInstance color, double life, double speed)
            : base(engine)
        {
            Image = image;
            ImgWidth = (int)image.Texture.Size.X;
            ImgHeight = (int)image.Texture.Size.Y;
            StartX = 0;
            StartY = 0;
            Life = life;
            Speed = speed;
            Color = color;

            PopulateFunctions();
        }

        [JSFunction(Name = "setAngle")]
        public void SetAngle(double v)
        {
            Angle = v;
        }

        [JSFunction(Name = "emit")]
        public void Emit(int x, int y, int amount)
        {
            StartX = x;
            StartY = y;
            ParticleEngine.Emit(this, amount);
        }
    }

    public class Particle
    {
        public double Life { get; set; }

        Texture _img;
        double _x, _y;
        double _vx, _vy;
        //bool _fade = true;
        double _total;
        Color _color;

        public void Setup(EmitterInstance parent)
        {
            _color = parent.Color.Color;
            _x = parent.StartX;
            _vx = parent.Speed * Math.Cos(parent.Angle);
            _y = parent.StartY;
            _vy = parent.Speed * Math.Sin(parent.Angle);
            _img = parent.Image.Texture;
            _total = parent.Life;
            Life = parent.Life;
        }

        public void Render(double ox, double oy)
        {
            double x = _x + ox;
            double y = _y + oy;
            Program.Batch.Add(_img, (float)x, (float)y, _color);
        }

        public void Update(double time)
        {
            _x += _vx * time * 60;
            _y += _vy * time * 60;
            Life -= time * 1000;
            _color.A = (byte)((Life / _total) * 255);
        }
    }
}
