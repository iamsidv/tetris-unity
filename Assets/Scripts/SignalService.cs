using System;
using System.Collections.Generic;

public class SignalService
{
    public static event Action<int> OnScoreUpdated;
    public static event Action<int> OnGameStateUpdate;
    public static event Action OnBlockPlacedEvent;
    public static event Action OnBlockTeleportEvent;

    public static void TriggerOnBlockPlacedEvent()
    {
        OnBlockPlacedEvent?.Invoke();
    }

    public static void TeleportCurrentBlock()
    {
        OnBlockTeleportEvent?.Invoke();
    }

    //    public Dictionary<System.Type, List<System.Action<ISignal>>> receivers = new Dictionary<System.Type, List<System.Action<ISignal>>>();
    //    public Dictionary<System.Type, List<object>> observers = new Dictionary<System.Type, List<object>>();

    //    private static SignalService _instance;

    //    private static SignalService Instance
    //    {
    //        get
    //        {
    //            if (_instance == null)
    //                _instance = new SignalService();

    //            return _instance;
    //        }
    //    }

    //    //public static void Subscribe<T>(System.Action<T> callback) where T : ISignal
    //    //{
    //    //    UnityEngine.Debug.Log("SignalService " + typeof(T));

    //    //    var localType = callback.GetType();

    //    //    if (Instance.receivers.ContainsKey(localType))
    //    //    {

    //    //             UnityEngine.Debug.Log("SignalService " + ((callback as System.Action<ISignal>)==null));
    //    //             UnityEngine.Debug.Log("SignalService " + (callback == null));

    //    //        Instance.receivers[localType].Add(callback as System.Action<ISignal>);
    //    //    }
    //    //    else
    //    //    {
    //    //        //Instance.receivers.Add(localType, new List<System.Action<Signal>> { callback });
    //    //    }

    //    //    UnityEngine.Debug.Log("SignalService " + Instance.receivers.Count);

    //    //}

    //    public void Subscribe<T>(System.Action<Signal<T>> callback)
    //    {
    //        var localtype = typeof(T);

    //        if (Instance.observers.ContainsKey(localtype))
    //        {
    //            Instance.observers[localtype].Add(callback);
    //        }
    //        else
    //        {
    //            Instance.observers.Add(localtype, new List<object> { callback });
    //        }
    //    }

    //    public static void Publish<T>(T signalObject = default) where T : ISignal
    //    {
    //        //if(signalObject== null)
    //        //{
    //        //    signalObject = new Si();
    //        //}

    //        var localType = signalObject.GetType();

    //        if (Instance.receivers.TryGetValue(localType, out var callbacks))
    //        {
    //            foreach (var item in callbacks)
    //            {
    //                item?.Invoke(signalObject);
    //            }
    //        }
    //    }
}
