using App.View.Common;

namespace App.Controller;

public abstract class BaseController
{
    private IView view;

    public BaseController(IView view)
    {
        this.view = view;
    }

    public bool Execute()
    {
        return HandleChoice(view.Show());
    }

    protected abstract bool HandleChoice(int choice);
}