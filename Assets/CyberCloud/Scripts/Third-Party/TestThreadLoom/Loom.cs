using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

/// <summary>
/// Unity多线程（Thread）和主线程（MainThread）交互使用类；
/// 由于unity不支持多线程内访问：UnityEngine的API + Unity的Obj类及其子类等，Loom在执行多线程时也可以让一些必须在主线程内访问的代码在主线程执行：
/// 变形网格中操作大量的顶点；持续的要运行上传数据到服务器；二维码识别等图像处理
/// </summary>
public class Loom : MonoBehaviour
{
    public static int maxThreads = 8;
    private static int numThreads;

    private static Loom _current;
    private int _count;
    public static Loom Current
    {
        get
        {
            Initialize();
            return _current;
        }
    }

    void Awake()
    {
        _current = this;
        initialized = true;
    }

    static bool initialized;

    /// <summary>初始化Loom</summary>
    static void Initialize()
    {
        if (!initialized)
        {
            if (!Application.isPlaying)
                return;
            initialized = true;
            var g = new GameObject("Loom");
            _current = g.AddComponent<Loom>();
        }
    }

    private List<Action> _actions = new List<Action>();
    public struct DelayedQueueItem
    {
        public float time;
        public Action action;
    }
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

    /// <summary>
    /// 在主线程运行actions
    /// </summary>
    public static void QueueOnMainThread(Action action)
    {
        QueueOnMainThread(action, 0f);
    }
    /// <summary>
    /// 在主线程运行actions
    /// </summary>
    /// <param name="time">延迟时间</param>
    public static void QueueOnMainThread(Action action, float time)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }
    /// <summary>
    /// 在另一个线程运行actions
    /// </summary>
    public static Thread RunAsync(Action a)
    {
        Initialize();
        while (numThreads >= maxThreads)
        {
            Thread.Sleep(1);
        }
        Interlocked.Increment(ref numThreads);
        ThreadPool.QueueUserWorkItem(RunAction, a);
        return null;
    }

    private static void RunAction(object action)
    {
        try
        {
            ((Action)action)();
        }
        catch
        {
        }
        finally
        {
            Interlocked.Decrement(ref numThreads);
        }
    }

    private void OnDisable()
    {
        if (_current == this)
        {
            _current = null;
        }
    }

    List<Action> _currentActions = new List<Action>();

    // Update is called once per frame  
    void Update()
    {
        lock (_actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(_actions);
            _actions.Clear();
        }
        foreach (var a in _currentActions)
        {
            a();
        }
        lock (_delayed)
        {
            _currentDelayed.Clear();
            _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
            foreach (var item in _currentDelayed)
                _delayed.Remove(item);
        }
        foreach (var delayed in _currentDelayed)
        {
            delayed.action();
        }
    }
}