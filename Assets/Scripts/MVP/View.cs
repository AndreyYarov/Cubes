using UnityEngine;

public abstract class View<V, P> : MonoBehaviour 
    where V : View<V, P>
    where P : BasePresenter<V, P>, new()
{
    protected P presenter;

    public virtual bool Visible
    {
        get => gameObject.activeSelf;
        set
        {
            if (value)
                presenter.OnShow();
            else
                presenter.OnHide();
        }
    }

    protected virtual void Start()
    {
        presenter = new P();
        presenter.SetView((V)this);
        presenter.OnStart();
    }

    protected virtual void Update() => presenter.OnUpdate();

    protected virtual void OnDestroy() => presenter.OnDestroy();
}
