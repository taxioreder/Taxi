using CoreAnimation;
using TaxiApp.iOS.NewRender;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(TextRender))]
namespace TaxiApp.iOS.NewRender
{
    public class TextRender : EntryRenderer
    {
        public TextRender()
        {
        }


        protected override async void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderStyle = UITextBorderStyle.None;

                var view = (Element as Entry);

                if (view != null)
                {
                    view.SizeChanged += Element_SizeChanged;
                }
            }
        }

        void DrawBorder(Entry view)
        {
            var borderLayer = new CALayer();
            borderLayer.MasksToBounds = true;
            borderLayer.Frame = new CoreGraphics.CGRect(0f, view.Height - 3, view.Width, 1f);
            borderLayer.BorderColor = Color.FromHex("#000000").ToCGColor();
            borderLayer.BorderWidth = 1.0f;
            Control.Layer.AddSublayer(borderLayer);
            Control.BorderStyle = UITextBorderStyle.None;
        }

        void Element_SizeChanged(object sender, System.EventArgs e)
        {
            var view = (Element as Entry);
            if (view != null)
            {
                DrawBorder(view);
            }
        }
    }
}   