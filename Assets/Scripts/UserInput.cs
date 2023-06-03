using UnityEngine;

public class UserInput : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SignalService.Publish(new DirectionButtonPressedSignal { Value = 1 });
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SignalService.Publish(new DirectionButtonPressedSignal { Value = -1 });
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SignalService.Publish(new DownArrowPressedSignal { Value = true });
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            SignalService.Publish(new DownArrowPressedSignal { Value = false });
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SignalService.Publish<RotateBlockSignal>();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SignalService.Publish<SpaceBarPressedSignal>();
        }
    }
}