using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;

namespace Engine.Objects
{
    class BinHeapConstructor : ClrFunction
    {
        public BinHeapConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "BinaryHeap", new BinHeapInstance(engine.Object.InstancePrototype))
        {
        }

        [JSConstructorFunction]
        public BinHeapInstance Construct(FunctionInstance func)
        {
            return new BinHeapInstance(InstancePrototype, func);
        }
    }

    class BinHeapInstance : ObjectInstance
    {
        private List<ObjectInstance> _heap;
        private FunctionInstance _func;

        public BinHeapInstance(ObjectInstance prototype)
            : base(prototype)
        {
            PopulateFunctions();
            _heap = new List<ObjectInstance>();
        }

        public BinHeapInstance(ObjectInstance prototype, FunctionInstance func)
            : base(prototype)
        {
            _heap = new List<ObjectInstance>();
            _func = func;
        }

        [JSFunction(Name = "add")]
        public void Add(ObjectInstance item)
        {
            _heap.Add(item);
            BubbleUp(_heap.Count - 1);
        }

        [JSFunction(Name = "pop")]
        public ObjectInstance Pop()
        {
            if (_heap.Count == 0) return null;
            var result = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);
            return result;
        }

        [JSFunction(Name = "shift")]
        public ObjectInstance Shift()
        {
            var result = _heap[0];
            var end = Pop();
            if (_heap.Count > 0)
            {
                _heap[0] = end;
                SinkDown(0);
            }
            return result;
        }

        [JSFunction(Name = "remove")]
        public void Remove(ObjectInstance node)
        {
            int length = _heap.Count;

            for (int i = 0; i < length; ++i)
            {
                if (!_heap[i].Equals(node)) continue;

                var end = Pop();
                if (end == null || i == length - 1) break;

                _heap[i] = end;
                BubbleUp(i);
                SinkDown(i);
                break;
            }
        }

        [JSFunction(Name = "clear")]
        public void Clear()
        {
            _heap.Clear();
        }

        [JSFunction(Name = "size")]
        public int Size()
        {
            return _heap.Count;
        }

        [JSFunction(Name = "resort")]
        public void Resort()
        {
            for (int i = _heap.Count - 1; i >= 0; i--)
            {
                BubbleUp(i);
            }
        }

        [JSFunction(Name = "update")]
        public void Update(ObjectInstance obj)
        {
            int length = _heap.Count;
            for (int i = 0; i < length; ++i)
            {
                if (!_heap[i].Equals(obj)) continue;
                BubbleUp(i);
                SinkDown(i);
                break;
            }
        }

        [JSFunction(Name = "get")]
        public ObjectInstance Get(int index)
        {
            return _heap[index];
        }

        [JSFunction(Name = "toArray")]
        public ArrayInstance ToArray()
        {
            return Engine.Array.New(_heap.ToArray());
        }

        private int CallComparator(ObjectInstance item)
        {
            return TypeConverter.ToInt32(_func.Call(null, item));
        }

        private void BubbleUp(int index)
        {
            var item = _heap[index];
            var score = CallComparator(item);

            while (index > 0)
            {
                int parent_idx = (int)((index + 1) / 2 - 1);
                var parent = _heap[parent_idx];

                if (score >= CallComparator(parent)) break;

                _heap[parent_idx] = item;
                _heap[index] = parent;
                index = parent_idx;
            }
        }

        private void SinkDown(int index)
        {
            int length = _heap.Count;
            var item = _heap[index];
            var score = CallComparator(item);
            while (true)
            {
                int child2N = (index + 1) * 2, child1N = child2N - 1;
                int swap = -1;
                ObjectInstance child1, child2;
                int ch1Score = 0, ch2Score = 0;

                if (child1N < length)
                {
                    child1 = _heap[child1N];
                    ch1Score = CallComparator(child1);
                    if (ch1Score < score) swap = child1N;
                }

                if (child2N < length)
                {
                    child2 = _heap[child2N];
                    ch2Score = CallComparator(child2);
                    if (ch2Score < ((swap == -1) ? score : ch1Score))
                        swap = child2N;
                }

                if (swap == -1) break;

                _heap[index] = _heap[swap];
                _heap[swap] = item;
                index = swap;
            }
        }
    }
}
