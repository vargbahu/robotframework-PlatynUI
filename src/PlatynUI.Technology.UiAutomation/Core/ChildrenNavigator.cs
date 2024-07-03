namespace PlatynUI.Technology.UIAutomation.Core;

using System.Collections.Generic;

internal abstract class ChildrenNavigatorBase<TParent, TChild>
{
    protected ChildrenNavigatorBase(TParent? parent)
    {
        Parent = parent;
    }

    public TParent? Parent { get; protected set; }

    public int CurrentIndex { get; protected set; }

    protected abstract IReadOnlyList<TChild> Children { get; }

    public bool IsStarted { get; protected set; }

    public virtual TChild? Current
    {
        get
        {
            if (IsStarted && CurrentIndex < Children.Count)
            {
                return Children[CurrentIndex];
            }

            return default;
        }
    }

    public virtual bool MoveToFirst()
    {
        IsStarted = true;

        if (Children.Count <= 0)
        {
            return false;
        }

        CurrentIndex = 0;

        return true;
    }

    public bool MoveToNext()
    {
        if (IsStarted && CurrentIndex + 1 >= Children.Count)
        {
            return false;
        }

        CurrentIndex++;

        return true;
    }

    public bool MoveToPrevious()
    {
        if (IsStarted && CurrentIndex - 1 < 0)
        {
            return false;
        }

        CurrentIndex--;

        return true;
    }

    public abstract ChildrenNavigatorBase<TParent, TChild> Clone();
}
