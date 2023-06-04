using System.Runtime.CompilerServices;
using Urho3DNet;

namespace Urho3DNetTemplate
{
    /// <summary>
    /// Base class for unit tests.
    /// Await ToMainThreadAsync() before code that should be executed in application's main thread. 
    /// </summary>
    public class UnitTestBase
    {
        public Context Context => TestApplication.Instance.Context;

        public ConfiguredTaskAwaitable<bool> ToMainThreadAsync()
        {
            return TestApplication.Instance.ToMainThreadAsync();
        }
    }
}