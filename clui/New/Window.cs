using System;
using System.Collections;
using System.Collections.Generic;

namespace clui.New
{
  public abstract class Window
  {
    public CluiApplication? Application { get; internal set; }

    public ControlCollection Controls { get; }

    protected Window()
    {
      Controls = new ControlCollection(this);
    }

    public void Close()
    {
      
    }

    public void BringToFront()
    {

    }

  }

  public class ControlCollection : IList<Control>
  {
    private readonly Window? _window;
    private readonly ContainerControl? _containerControl;

    private readonly List<Control> _innerList = new();

    public ControlCollection(Window window)
    {
      _window = window;
    }

    public ControlCollection(ContainerControl containerControl)
    {
      _containerControl = containerControl;
    }

    public IEnumerator<Control> GetEnumerator()
    {
      return _innerList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)_innerList).GetEnumerator();
    }

    public void Add(Control item)
    {
      SetWindowAndParent(item);
      _innerList.Add(item);
    }

    public void Clear()
    {
      _innerList.Clear();
    }

    public bool Contains(Control item)
    {
      return _innerList.Contains(item);
    }

    public void CopyTo(Control[] array, int arrayIndex)
    {
      _innerList.CopyTo(array, arrayIndex);
    }

    public bool Remove(Control item)
    {
      item.Window = null;
      item.Parent = null;

      var result = _innerList.Remove(item);
      return result;
    }

    public int Count => _innerList.Count;

    public bool IsReadOnly => ((ICollection<Control>)_innerList).IsReadOnly;

    public int IndexOf(Control item)
    {
      return _innerList.IndexOf(item);
    }

    public void Insert(int index, Control item)
    {
      SetWindowAndParent(item);
      _innerList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      Remove(this[index]);
    }

    public Control this[int index]
    {
      get => _innerList[index];
      set => throw new InvalidOperationException();
    }

    private void SetWindowAndParent(Control item)
    {
      item.Window = _window;
      item.Parent = _containerControl;
    }
  }
}
