using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UserInput : MonoBehaviour
{
    public static event Action<int> OnDirectionChangeEvent;
    public static event Action OnRotateEvent;
    public static event Action OnMoveDownEvent;

    // Update is called once per frame
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnMoveDownEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnRotateEvent?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {

        }
    }
}
