using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayUI
{
    private static GameObject _view;
    private static GameObject view
    {
        get
        {
            if (!_view)
            {
                var canvas = Object.FindObjectOfType<Canvas>();
                _view = canvas.transform.Find("GameplayUI").gameObject;
            }
            return _view;
        }
    }

    public static void Show()
    {
        view.SetActive(true);
    }
    public static void Hide()
    {
        view.SetActive(false);
    }
}
