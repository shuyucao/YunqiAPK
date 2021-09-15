using System;
using System.Collections.Generic;
using System.Text;
namespace Assets.Scripts.Tool
{
    public class Singleton<T> where T : new()
    {
        private static T _Instance;
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new T();
                }
                return _Instance;
            }
        }

        public Singleton()
        {

        }
    }
}
