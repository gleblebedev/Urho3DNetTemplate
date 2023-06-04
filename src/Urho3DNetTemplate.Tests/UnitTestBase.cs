using Urho3DNet;

namespace Urho3DNetTemplate;

public class UnitTestBase
{
    public Context Context => TestApplication.Instance.Context;

    public Task OnMainThread()
    {
        return TestApplication.Instance.OnMainThreadAsync();
    }
}