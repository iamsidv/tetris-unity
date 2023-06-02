using UnityEngine;

public class BaseView : MonoBehaviour
{
    public virtual void OnScreenEnter() { }
    public virtual void OnScreenExit() { }

    public void SetVisibility(bool state)
    {
        gameObject.SetActive(state);
    }
}
