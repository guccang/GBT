using System;
using System.Collections.Generic;
using log4net;

namespace GBT
{
    class ObjectPoolMgr : Singleton<ObjectPoolMgr>
    {
        private static ILog log = LogConfig.GetLog(typeof(ObjectPoolMgr));
        private Dictionary<Type, IObjectPool> _objectPools;
        public static int INC_POOL = 100;
        public static int CAPICITY = int.MaxValue;

        public ObjectPoolMgr()
        {
            _objectPools = new Dictionary<Type, IObjectPool>();
        }
        public static T Pop<T>()
           where T : class, new()
        {
#if GUCCANG_OBJ_POOL
            var pool = Instance.getPool<T>();
            return pool.Pop();
#else 
            return new T();
#endif
        }

        [System.Diagnostics.Conditional("GUCCANG_OBJ_POOL")]
        public static void Push(object obj)
        {
            IObjectPool pool = null;
            Instance._objectPools.TryGetValue(obj.GetType(), out pool);
            if (null != pool)
                pool.Push(obj);
        }

        [System.Diagnostics.Conditional("GUCCANG_OBJ_POOL")]
        public static void Test()
        {
            foreach (var pool in Instance._objectPools)
            {
                string info = $"{pool.Key}- usedCnt:{pool.Value.UsedCnt()} freeCnt:{pool.Value.FreeCnt()}";
                if (pool.Value.UsedCnt() != 0)
                {
                    log.Error(info);
                }
                else
                {
                    log.Warn(info);
                }
            }
        }

        private ObjectPool<T> getPool<T>()
            where T : class, new()
        {
            IObjectPool pool = null;
            _objectPools.TryGetValue(typeof(T), out pool);
            if (null == pool)
            {
                pool = new ObjectPool<T>(INC_POOL, CAPICITY);
                _objectPools.Add(typeof(T), pool);
            }
            var p = pool as ObjectPool<T>;
            return p;
        }
    }

    interface IObjectPool
    {
        void Push(object obj);
        int FreeCnt();
        int UsedCnt();
    }


    class ObjectPool<T> : IObjectPool
       where T : class, new()
    {
        private static ILog log = LogConfig.GetLog(typeof(ObjectPool<T>));
        private LinkedList<T> _freeObjs;
        private HashSet<T> _usedObjs;
        private int _inc;
        private int _capicity;

        public ObjectPool(int inc, int capicity)
        {
            _inc = inc;
            if (_inc <= 0)
                _inc = 10;

            _capicity = capicity;
            if (_capicity <= _inc)
                _capicity = _inc * 2;

            _freeObjs = new LinkedList<T>();
            _usedObjs = new HashSet<T>();
        }

        public void Push(object obj)
        {
            if (null == obj)
                return;
            var tObj = (T)obj;
            if (false == _usedObjs.Contains(tObj))
            {
                log.Error($"objpool_ error Free.{tObj}");
                return;
            }
            _usedObjs.Remove(tObj);
            _freeObjs.AddLast(tObj);
            resize();
        }

        public T Pop()
        {
            if (_freeObjs.Count <= 0)
                inc();

            T obj = null;
            if (_freeObjs.Count > 0)
            {
                obj = _freeObjs.First.Value;
                _freeObjs.RemoveFirst();
                _usedObjs.Add(obj);
            }
            resize();
            return obj;
        }
        public int UsedCnt()
        {
            return _usedObjs.Count;
        }
        public int FreeCnt()
        {
            return _freeObjs.Count;
        }
        /// /////////////////////////////////////////////////
        private void inc()
        {
            int inc = _inc;
            if (_freeObjs.Count > _capicity)
                inc = 1;

            for (int i = 0; i < inc; ++i)
            {
                _freeObjs.AddLast(new T());
            }
        }
        private void resize()
        {
            int threshold = (int)(_freeObjs.Count * 0.5f);
            if (_usedObjs.Count < threshold)
            {
                int rmCnt = (int)(threshold * 0.2f); // 0.1 of total
                for (int i = 0; i < rmCnt; ++i)
                    _freeObjs.RemoveFirst();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method ,  AllowMultiple = false)]
    public class FreeUseLimitAttribute : System.Attribute
    {
        public List<Type> canUseClassType;
        public FreeUseLimitAttribute()
        {
            canUseClassType = new List<Type>();
        }
    }
}
