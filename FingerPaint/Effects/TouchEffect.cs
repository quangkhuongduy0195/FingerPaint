using System;
using Xamarin.Forms;

namespace FingerPaint.Effects
{
    public class TouchEventArgs
    {
        public Point TouchedPosition { get; }
        public TouchEventArgs(Point touchedPoint)
        {
            TouchedPosition = touchedPoint;
        }
    }

    public delegate void TouchEventHandler(object sender, TouchEventArgs touchEventArgs);

    public class TouchEffect : RoutingEffect
    {
        public TouchEffect() : base($"{EffectConfig.MainResolutionName}.{nameof(TouchEffect)}")
        {
        }

        public event EventHandler TouchCancel;
        public event EventHandler TouchDown;
        public event TouchEventHandler TouchUpInside;

        public void InvokeTouchCancel(object sender, EventArgs e)
        {
            TouchCancel?.Invoke(sender, e);
        }

        public void InvokeTouchDown(object sender, EventArgs e)
        {
            TouchDown?.Invoke(sender, e);
        }

        public void InvokeTouchUpInside(object sender, TouchEventArgs e)
        {
            TouchUpInside?.Invoke(sender, e);
        }
    }
}
