using Urho3DNet;
using Xunit;

namespace Urho3DNetTemplate
{
    public class SampleUnitTestFixture: UnitTestBase
    {
        [Fact]
        public async Task SampeUnitTest()
        {
            await OnMainThread();

            SharedPtr<Scene> scene = new Scene(Context);

            Assert.Equal(0u, scene.Ptr.GetNumChildren());
        }

        [Fact]
        public async Task SampeUnitTest2()
        {
            await OnMainThread();

            SharedPtr<Scene> scene = new Scene(Context);
            scene.Ptr.SubscribeToEvent("TestEvent", scene.Ptr, map => { });
            scene.Ptr.SendEvent("TestEvent");
        }
    }
}
