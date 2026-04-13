using Android.Views;
using Android.App;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("NimbleBlocks")]
[assembly: ExportEffect(typeof(NimbleBlocks.Android.ShadowEffectRenderer), nameof(NimbleBlocks.ShadowEffect))]

namespace NimbleBlocks.Android
{
    public class ShadowEffectRenderer : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Control is global::Android.Views.View view)
                {
                    var radius = NimbleBlocks.ShadowEffect.GetRadius(Element);
                    view.Elevation = radius;
                    view.TranslationZ = 0.5f * radius;
                    view.SetClipToOutline(false);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Cannot set shadow effect on Android. Error: {ex.Message}");
            }
        }

        protected override void OnDetached()
        {
            try
            {
                if (Control is global::Android.Views.View view)
                {
                    view.Elevation = 0;
                    view.TranslationZ = 0;
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Cannot remove shadow effect on Android. Error: {ex.Message}");
            }
        }
    }
}