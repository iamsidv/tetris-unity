using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public List<BaseView> views;

    private static MenuManager _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }


    public static T ShowMenu<T>() where T : BaseView
    {
        var type = typeof(T);

        var menu = _instance.views.Find(t => t.GetType().Equals(type));
        menu.SetVisibility(true);
        menu.OnScreenEnter();
        
        return menu as T;
    }

    public static void HideMenu<T>() where T : BaseView
    {
        var type = typeof(T);

        var menu = _instance.views.Find(t => t.GetType().Equals(type));
        menu.OnScreenExit();
        menu.SetVisibility(false);
    }
}
