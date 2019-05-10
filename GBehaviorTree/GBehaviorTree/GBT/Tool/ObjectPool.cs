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
        public static T Get<T>()
           where T: class , new ()
        {
#if GUCCANG_OBJ_POOL
            var pool = Instance.getPool<T>();
            return pool.Get();
#else 
            return new T();
#endif

        }

        public static void Free(object obj)
        {
#if GUCCANG_OBJ_POOL
            foreach(var pool in Instance._objectPools)
            {
                if(pool.Key == obj.GetType())
                {
                    pool.Value.Free(obj);
                    break;
                }
            }
#endif
        }

        public static void Test()
        {
#if GUCCANG_OBJ_POOL
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
#endif
        }

        private ObjectPool<T> getPool<T>()
            where T : class, new()
        {
            IObjectPool pool = null;
            _objectPools.TryGetValue(typeof(T), out pool);
            if (null == pool)
            {
                pool = new ObjectPool<T>(INC_POOL,CAPICITY);
                _objectPools.Add(typeof(T), pool);
            }
            var p = pool as ObjectPool<T>;
            return p;
        }
    }

    interface IObjectPool
    {
        void Free(object obj);
        int FreeCnt();
        int UsedCnt();
    }
    class ObjectPool<T> : IObjectPool
       where T : class, new()
    {
        private LinkedList<T> _freeObjs;
        private int _usedCnt;
        private int _inc;
        private int _capicity;

        public ObjectPool(int inc,int capicity)
        {
            _inc = inc;
            if (_inc <= 0)
                _inc = 10;

            _capicity = capicity;
            if (_capicity <= _inc)
                _capicity = _inc * 2;

            _freeObjs = new LinkedList<T>();
            _usedCnt = 0;
        }
        public void Free(object obj)
        {
            if (null == obj)
                return;
            _usedCnt--;
            _freeObjs.AddLast((T)obj);
            resize();
        }
        public T Get()
        {
            if (_freeObjs.Count <= 0)
                inc();

            object obj = null;
            if (_freeObjs.Count > 0)
            {
                obj =  _freeObjs.First.Value;
                _freeObjs.RemoveFirst();
                _usedCnt++;
            }
            resize();
            return (T)obj;
        }

        public int UsedCnt()
        {
            return _usedCnt;
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

            for(int i=0;i< inc; ++i)
            {
                _freeObjs.AddLast(new T());
            }
        }
        private void resize()
        {
            return;
            int threshold = (int)(_freeObjs.Count * 0.5f);
            if (_usedCnt < threshold)
            {
                int rmCnt = (int)(threshold * 0.2f); // 0.1 of taotal
                for (int i = 0; i < rmCnt; ++i)
                    _freeObjs.RemoveFirst();
            }
        }
    }
}
