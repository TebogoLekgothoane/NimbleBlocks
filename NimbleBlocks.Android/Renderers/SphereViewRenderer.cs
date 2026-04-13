using Android.Content;
using Android.Graphics;
using Android.Views;
using NimbleBlocks.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SphereView), typeof(SphereViewRenderer))]

namespace NimbleBlocks.Android.Renderers
{
    public class SphereViewRenderer : ViewRenderer<SphereView, Android.Views.View>
    {
        private Paint _paint;
        private Paint _highlightPaint;
        private float _centerX, _centerY;
        private float _radius;
        private bool _isPressed = false;

        public SphereViewRenderer(Context context) : base(context)
        {
            _paint = new Paint(PaintFlags.AntiAlias);
            _highlightPaint = new Paint(PaintFlags.AntiAlias);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SphereView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var view = new Android.Views.View(Context);
                SetNativeControl(view);
                Control.Touch += OnTouch;
                
                // Set the size based on the Element's Size property
                var size = (int)Element.Size;
                Control.LayoutParameters = new Android.Views.ViewGroup.LayoutParams(size, size);
            }
        }

        private void OnTouch(object sender, TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
            {
                _isPressed = true;
                Invalidate();
                Element.OnSphereTapped();
            }
            else if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel)
            {
                _isPressed = false;
                Invalidate();
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (Element == null) return;

            _centerX = Width / 2f;
            _centerY = Height / 2f;
            _radius = (float)Math.Min(Width, Height) / 2f - 10;


            // Draw simple circle
            _paint.SetShader(null);
            _paint.Color = Element.Color.ToAndroid();
            canvas.DrawCircle(_centerX, _centerY, _radius, _paint);

            // Draw selection ring if selected
            if (Element.IsSelected)
            {
                _highlightPaint.SetShader(null);
                _highlightPaint.Color = Color.Argb(200, 255, 255, 0);
                _highlightPaint.StrokeWidth = 4;
                _highlightPaint.SetStyle(Paint.Style.Stroke);
                canvas.DrawCircle(_centerX, _centerY, _radius + 5, _highlightPaint);
            }

            // Draw pressed effect
            if (_isPressed)
            {
                _highlightPaint.SetShader(null);
                _highlightPaint.Color = Color.Argb(100, 255, 255, 255);
                _highlightPaint.SetStyle(Paint.Style.Fill);
                canvas.DrawCircle(_centerX, _centerY, _radius * 0.8f, _highlightPaint);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == SphereView.ColorProperty.PropertyName ||
                e.PropertyName == SphereView.IsSelectedProperty.PropertyName ||
                e.PropertyName == SphereView.SizeProperty.PropertyName)
            {
                Invalidate();
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            Invalidate();
        }
    }
}