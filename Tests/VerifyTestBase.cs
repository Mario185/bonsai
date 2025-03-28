using Argon;

namespace Tests
{
  public abstract class VerifyTestBase
  {
    protected VerifySettings VerifySettings { get; }

    protected VerifyTestBase()
    {
      VerifySettings = new VerifySettings();
      VerifySettings.AddExtraSettings(s => s.TypeNameHandling = TypeNameHandling.All);
      VerifySettings.DisableDiff();
      VerifySettings.UseDirectory(Path.Combine("verify_snapshots", GetType().Name));
    }
  }
}