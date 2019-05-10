using System;
using System.Collections.Generic;
using System.Text;

namespace GBT
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _isnstance = null;

        public static T Instance
        {
            get
            {
                if (_isnstance == null)
                    _isnstance = new T();
                return _isnstance;
            }
        }
    }

    class RandomHelp
    {
        static System.Random _random;
        static RandomHelp()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }
        public static int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }
    }
}
