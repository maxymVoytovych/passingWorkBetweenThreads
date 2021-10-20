using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lab5
{
    class WorkerWithQueue<T> : IDisposable where T : class
    {
        readonly object _locker = new object();
        private Thread _worker;
        readonly Queue<T> _taskQueue = new Queue<T>();
        readonly Action<T> _dequeueAction;
        private WorkerWithQueue<T> _nextWorker;

        public WorkerWithQueue(Action<T> dequeueAction)
        {
            _dequeueAction = dequeueAction;
        }
        public void StartExecuting(WorkerWithQueue<T> nextWorker)
        {
            _nextWorker = nextWorker;
            _worker = new Thread(Consume) { IsBackground = true };
            _worker.Start();
        }
        public void EnqueueTask(T task)
        {
            lock (_locker)
            {
                _taskQueue.Enqueue(task);
                Monitor.PulseAll(_locker);
            }
        }
        void PassTheWork(T task)
        {
            _nextWorker.EnqueueTask(task);
        }
        void Consume()
        {
            while (true)
            {
                T item;
                lock (_locker)
                {
                    while (_taskQueue.Count == 0) Monitor.Wait(_locker);
                    item = _taskQueue.Dequeue();
                }
                if (item == null) return;
                _dequeueAction(item);
                PassTheWork(item);
            }
        }
        public void Dispose()
        {
            EnqueueTask(null);
        }
    }
}
