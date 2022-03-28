using UnityEngine;

public static class Tutorial
{
    private static GameObject _view;
    private static GameObject view
    {
        get
        {
            if (!_view)
            {
                var canvas = Object.FindObjectOfType<Canvas>();
                _view = canvas.transform.Find("Tutorial").gameObject;
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
