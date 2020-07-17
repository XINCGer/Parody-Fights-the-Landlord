//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections.Generic;

namespace ColaFramework
{
    /// <summary>
    /// 管理所有的Timer对象的更新、创建与销毁
    /// 与旧版中的TimeHelper所不同的是，新版的TimeManager不是面向具体的业务逻辑使用者，它不包含创建一个计时、终止一个计时等功能,仅仅是管理Timer的声明周期
    /// 具体的计时、Timeout、Wait等功能下放到Timer中实现，符合现实生活中对闹钟(Timer)的定义 ，同时也避免了使用诸如Dictionary 或者 HashTable 之类的结构存储<id,Timer>的映射关系
    /// 避免使用Dictionary的实现，同时使用对象池，减少频繁内存的申请和GC的产生
    /// </summary>
    public class TimerManager : IManager
    {
        private readonly static int capacity = 10;
        private readonly List<Timer> _timers = new List<Timer>();
        private readonly MiniObjectPool<Timer> _objectPool = new MiniObjectPool<Timer>(capacity);
        private static TimerManager timerManager;

        public static TimerManager Instance
        {
            get
            {
                if (null == timerManager)
                {
                    timerManager = new TimerManager();
                }
                return timerManager;
            }
        }

        public float TimeSinceUpdate
        {
            get;
            set;
        }

        public Timer Get()
        {
            var tiemr = _objectPool.Get();
            _timers.Add(tiemr);

            return tiemr;
        }


        public void Update(float deltaTime)
        {
            RemoveTimer();

            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                _timers[i].OnUpdate(deltaTime);
            }

            RemoveTimer();
        }

        public void RemoveTimer()
        {
            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
                if (timer.IsCancelled)
                {
                    timer.Reset();
                    _timers.RemoveAt(i);
                    _objectPool.Release(timer);
                }
            }
        }

        public bool TryGet(long useId, ref Timer timer)
        {
            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                if (_timers[i].UseId != useId) continue;

                timer = _timers[i];
                return true;
            }

            return false;
        }

        public void Init()
        {
        }

        public void Dispose()
        {
        }
    }
}