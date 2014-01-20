using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{
    public class Pathfinder
    {
        public class Node
        {
            public int x, y;
            public int f, g, h;
            public bool blocked;
            public Node next;

            public Node(int xpos, int ypos) { x = xpos; y = ypos; }
        }

        private class NodeComparer : IComparer<Node> {
            public int Compare(Node x, Node y)
            {
                return x.f - y.f;
            }
        }

        private TwoArray<bool> _closed;
        private SortedSet<Node> _open;
        private TwoArray<Node> _grid;
        private int _width, _height;

        public Pathfinder(int width, int height)
        {
            _width = width;
            _height = height;
            _closed = new TwoArray<bool>(width, height);
            _open = new SortedSet<Node>(new NodeComparer());
            _grid = new TwoArray<Node>(width, height);
            for (int y = 0, i = 0; y < height; ++y)
                for (int x = 0; x < width; ++x, ++i)
                    _grid.Array[i] = new Node(x, y);
        }

        public Node DoPath(int x1, int y1, int x2, int y2) {
            _open.Clear();
            _closed.Clear(false);

            int cx = x1, cy = y1;
            Node current = null, temp = null;

            _grid.Get(x1, y1, ref current);
            _open.Add(current);

            while (cx != x2 || cy != y2)
            {
                int w = cx + 2;
                int h = cy + 2;

                for (int x = cx - 1; x < w; ++x) {
                    if (x < 0 || x == _width) continue;

                    for (int y = cy - 1; y < h; ++y) {
                        if (y < 0 || y == _height) continue;

                        bool closed = false;
                        _closed.Get(x, y, ref closed);
                        if (closed) continue;

                        _grid.Get(x, y, ref temp);
                        if (temp.blocked) continue;

                        if (!_open.Contains(temp)) {
                            if (x == cx || y == cy)
                                temp.g = current.g + 10;
                            else
                                temp.g = current.g + 14;
                            temp.h = 10*(Math.Abs(x - x2) + Math.Abs(y - y2));
                            temp.next = current;
                            temp.f = temp.g + temp.h;
                            _open.Add(temp);
                        }
                        else if (current.g < temp.g) {
                            if (x == current.x || y == current.y)
                                temp.g = current.g + 10;
                            else
                                temp.g = current.g + 14;
                            temp.next = current;
                            temp.f = temp.g + temp.h;
                        }
                    }
                }
                if (_open.Count == 0) return null;
                foreach (Node n in _open)
                {
                    current = n;
                    break;
                }
                _open.Remove(current);
                _closed.Set(current.x, current.y, true);
            }

            return current;
        }
    }
}

