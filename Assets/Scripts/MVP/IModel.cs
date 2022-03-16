using UnityEngine.Events;

public interface IModel
{
    void AddUpdateListener(UnityAction listener);
    void RemoveUpdateListener(UnityAction listener);
}
