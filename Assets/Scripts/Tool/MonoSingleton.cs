using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Tool
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        #region 单例

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
                return instance;
            }
        }

        #endregion
        //可重写的Awake虚方法，用于实例化对象
        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
}
