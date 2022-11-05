using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ASP_MVC_lesson2
{
    //1. Используя знания о пуле потоков, напишите свой микро пул с использованием новых структур данных.
    //2. Добавьте в ваш пул настройку для максимального количества регистрируемых потоков или кидайте ошибку

    public sealed class ObjectPool<TPullItem> where TPullItem : PullItem, new()
    {
        private int _MaxCountThreads;
        private readonly ConcurrentQueue<TPullItem> _queue = new ConcurrentQueue<TPullItem>();

        public ObjectPool(int maxCountThreads)
        {
            _MaxCountThreads = maxCountThreads;
        }

        public TPullItem Get()
        {
            if (_queue.Count > 0)
            {
                var item = new TPullItem();
                _queue.TryDequeue(out item);
                return item;
            }
            return new TPullItem();
        }

        public void AddItem(TPullItem item)
        {
            if (_queue.Count < _MaxCountThreads)
            {
                _queue.Enqueue(item);
            }
            else
            {
                throw new ArgumentOutOfRangeException(_queue.Count.ToString(), _MaxCountThreads.ToString(), $"Количество потоков не должно превышать максимальное -  {_MaxCountThreads}");
            }
        }

        public void Release(TPullItem item)
        {
            if (item is null)
            {
                return;
            }
            item.Reset();
            AddItem(item);
        }

    }

    public abstract class PullItem
    {
        public abstract void Reset();
    }
    public sealed class ClassicPullItem : PullItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override void Reset()
        {
            Id = 0;
            Name = string.Empty;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            ObjectPool<ClassicPullItem> _pool = new ObjectPool<ClassicPullItem>(3);
            ClassicPullItem item = _pool.Get();
            item.Id = 100;
            item.Name = "Just4Fun";
            _pool.Release(item);
            ClassicPullItem itemSecond = _pool.Get();

            _pool.AddItem(new ClassicPullItem());
            _pool.AddItem(new ClassicPullItem());
            _pool.AddItem(new ClassicPullItem());
            _pool.AddItem(new ClassicPullItem()); // Exception

            Console.ReadKey();
        }
    }
}
