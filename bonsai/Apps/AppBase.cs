using System;
using System.Collections.Generic;

namespace bonsai.Apps
{
  internal static class BonsaiContext
  {
    private static readonly Stack<IBonsaiContext> s_appStack = new();

    public static IBonsaiContext? Current { get; private set; }

    public static void Push (IBonsaiContext app)
    {
      s_appStack.Push (app);
      Current = s_appStack.Peek();
    }

    public static void Pop ()
    {
      s_appStack.Pop();
      Current = s_appStack.Count > 0 ? s_appStack.Peek() : null;
    }
  }

  internal abstract class AppBase
  {
    protected abstract IBonsaiContext Context { get; }

    public string? Run ()
    {
      try
      {
        BonsaiContext.Push (Context);
        return RunInternal();
      }
      finally
      {
        BonsaiContext.Pop();
      }
    }

    protected abstract string? RunInternal ();
  }

  internal interface IBonsaiContext
  {
    bool IsFilteringActive { get; }
  }
}