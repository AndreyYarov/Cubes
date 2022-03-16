public abstract class Presenter<M, V, P> : BasePresenter<V, P>
    where M : IModel
    where V : View<V, P>
    where P : BasePresenter<V, P>, new()
{
    private M _model;
    protected M model => _model;

    public void SetModel(M model)
    {
        _model = model;
        _model.AddUpdateListener(OnModelUpdate);
    }

    protected virtual void OnModelUpdate() { }
}