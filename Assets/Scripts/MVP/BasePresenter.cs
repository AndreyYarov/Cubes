public abstract class BasePresenter<V, P>
    where V : View<V, P>
    where P : BasePresenter<V, P>, new()
{
    protected V view;

    public BasePresenter() { }

    public void SetView(V view)
    {
        this.view = view;
    }

    public bool Visible
    {
        get => view.Visible;
        set => view.Visible = value;
    }

    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnShow() { }
    public virtual void OnHide() { }
    public virtual void OnDestroy() { }
}
