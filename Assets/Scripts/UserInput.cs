using System;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public static event Action<int> OnDirectionChangeEvent;

    public static event Action OnRotateEvent;

    public static event Action<bool> OnDownButtonPressed;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnDirectionChangeEvent?.Invoke(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnDirectionChangeEvent?.Invoke(-1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnDownButtonPressed?.Invoke(true);
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            OnDownButtonPressed?.Invoke(false);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //SignalService.Publish<TestSignal>(new TestSignal { Value = 1 });
            OnRotateEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SignalService.TriggerSpaceBarPressedEvent();
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    SignalService.Publish<ButtonPressedSignal>(new ButtonPressedSignal { Value = 100 });
        //}
    }
}