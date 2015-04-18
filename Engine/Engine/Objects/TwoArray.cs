using System;

// Copyright Andrew Helenius, 2013

namespace Engine
{
    /// <summary>
    /// A Two array, manages a 1D array as if it were 2D.
    /// </summary>
    public class TwoArray<T>
    {
        T[] _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Engine.TwoArray`1"/> class.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public TwoArray(int width, int height)
        {
            _data = new T[width * height];
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the underlying array.
        /// </summary>
        /// <value>The array.</value>
        public T[] Array
        {
            get { return _data; }
        }

        /// <summary>
        /// Get the specified value from the x, y coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="value">The value if changed.</param>
        public void Get(int x, int y, ref T value)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
                value = _data[x + y * Width];
        }

        /// <summary>
        /// Set the specified data at the x, y coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="value">The data to set.</param>
        public void Set(int x, int y, T value)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
                _data[x + y * Width] = value;
        }

        /// <summary>
        /// Tries to set the value at the x, y coordinate.
        /// </summary>
        /// <returns><c>true</c>, if set was successful, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="value">Value.</param>
        public bool TrySet(int x, int y, T value)
        {
            if (x < 0 || y < 0 || x > Width || y > Height)
                return false;
            _data[x + y * Width] = value;
            return true;
        }

        /// <summary>
        /// Set the data at the linear i'th position.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <param name="data">The data to set.</param>
        public void Set(int i, T data)
        {
            if (i >= 0 && i < _data.Length)
                _data[i] = data;
        }

        /// <summary>
        /// Replace this array with the data array.
        /// </summary>
        /// <param name="lineardata">Lineardata.</param>
        public void Set(T[] lineardata)
        {
            if (lineardata.Length != _data.Length)
                System.Array.Resize(ref _data, lineardata.Length);
            System.Array.Copy(lineardata, _data, lineardata.Length);
        }

        /// <summary>
        /// Resize this 2D array to the specified newSize.
        /// </summary>
        /// <param name="newSize">New size.</param>
        public void Resize(int width, int height)
        {
            T[] copy = new T[width * height];

            int w = Math.Min(Width, width);
            int h = Math.Min(Height, height);

            for (int y = 0; y < h; ++y)
            {
                int scan1 = y * w, scan2 = y * Width;
                System.Array.Copy(_data, scan2, copy, scan1, Width);
            }

            Set(copy);
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a copy of this instance.
        /// </summary>
        public TwoArray<T> Clone()
        {
            TwoArray<T> array = new TwoArray<T>(Width, Height);
            System.Array.Copy(_data, array._data, _data.Length);
            return array;
        }

        /// <summary>
        /// Clear the data to the specified value.
        /// </summary>
        /// <param name="set_to">The value to clear the elements to.</param>
        public void Clear(T value = default(T))
        {
            for (int i = 0; i < _data.Length; ++i)
                _data[i] = value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Engine.TwoArray`1[[`0]]"/> is equal to the current <see cref="Engine.TwoArray`1"/>.
        /// </summary>
        /// <param name="array">The <see cref="Engine.TwoArray`1[[`0]]"/> to compare with the current <see cref="Engine.TwoArray`1"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Engine.TwoArray`1[[`0]]"/> is equal to the current
        /// <see cref="Engine.TwoArray`1"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(TwoArray<T> array)
        {
            if (array._data.Length < _data.Length)
                return false;
            for (int i = 0; i < _data.Length; ++i)
            {
                if (!_data[i].Equals(array._data[i]))
                    return false;
            }
            return true;
        }
    }
}

