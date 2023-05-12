using NUnit.Framework;

namespace SignalPlot.UiTests
{
    public class Given_MainPage : TestBase
    {
        [Test]
        [Ignore("Manually start WASM platform without debugger then run this test.")]
        public void When_SmokeTest()
        {
            // Create a button with the `AutomationProperties.AutomationId="MyButton"`
            // then use the following code to search and tap the button
            //
            // Query MyButton = q => q.All().Marked("MyButton");
            // App.WaitForElement(MyButton);
            // App.Tap(MyButton);

            // Take a screenshot and add it to the test results
            TakeScreenshot("After tapped");
        }
    }
}