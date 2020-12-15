using System;
using System.Linq;
using CoreGraphics;
using FingerPaint.Effects;
using FingerPaint.iOS.Effects;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName(nameof(EffectConfig.MainResolutionName))]
[assembly: ExportEffect(typeof(TouchPlatformEffect), nameof(TouchEffect))]
namespace FingerPaint.iOS.Effects
{
    public class TouchPlatformEffect : PlatformEffect
    {
        private UIView _view;
        private TouchEffect _effect;
        private TouchGestureRecognizer _gestureRecognizer;
        private TouchManager _touchManager;

        protected override void OnAttached()
        {
            _effect = Element.Effects.FirstOrDefault(e => e is TouchEffect) as TouchEffect;
            _touchManager = new TouchManager(_effect, Element);
            if (Control is UIButton uIButton)
            {
                uIButton.TouchDown += _touchManager.TouchDown;
                uIButton.TouchCancel += _touchManager.TouchCancel;
                //uIButton.TouchUpInside += _touchManager.TouchUpInside;
            }
            else
            {
                _view = Container ?? Control;
                _gestureRecognizer = new TouchGestureRecognizer(_touchManager);
                _gestureRecognizer.RequiresExclusiveTouchType = true;
                _view.AddGestureRecognizer(_gestureRecognizer);
                _view.UserInteractionEnabled = true;
                _view.ExclusiveTouch = true;
            }
        }

        protected override void OnDetached()
        {
            if (Control is UIButton uIButton)
            {
                uIButton.TouchDown -= _touchManager.TouchDown;
                uIButton.TouchCancel -= _touchManager.TouchCancel;
                //uIButton.TouchUpInside -= _touchManager.TouchUpInside;
            }
            else
            {
                _view?.RemoveGestureRecognizer(_gestureRecognizer);
            }
        }
    }

    public class TouchGestureRecognizer : UIGestureRecognizer, IUIGestureRecognizerDelegate
    {
        readonly TouchManager _touchManager;
        readonly nfloat _outsideDistance = 80;

        public TouchGestureRecognizer(TouchManager touchManager)
        {
            _touchManager = touchManager;
            Delegate = this;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            _touchManager?.TouchDown(null, null);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            _touchManager?.TouchCancel(null, null);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            if (IsTouchInView(touches))
                _touchManager?.TouchUpInside(touches, evt, View);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            if (!IsTouchInView(touches))
                _touchManager?.TouchDragOutside(null, null);
        }

        bool IsTouchInView(NSSet touches)
        {
            var touch = touches.First() as UITouch;
            var point = touch.LocationInView(View);
            var outsideFrame = new CGRect(
                -_outsideDistance,
                -_outsideDistance,
                View.Frame.Width + _outsideDistance * 2,
                View.Frame.Height + _outsideDistance * 2);
            return outsideFrame.Contains(point);
        }
    }

    public class TouchManager
    {
        public Element Element { get; }

        private readonly TouchEffect _effect;

        public TouchManager(TouchEffect effect, Element element)
        {
            Element = element;
            _effect = effect;
        }

        public void TouchUpInside(NSSet touches, UIEvent evt, UIView view)
        {
            var touch = touches.First() as UITouch;
            var point = touch.LocationInView(view);
            Point touchedPoint = point.ToPoint();
            TouchEventArgs touchEventArgs = new TouchEventArgs(touchedPoint);

            _effect.InvokeTouchUpInside(Element, touchEventArgs);
        }

        public void TouchDragOutside(object sender, EventArgs e)
        {
            _effect.InvokeTouchCancel(Element, EventArgs.Empty);
        }

        public void TouchCancel(object sender, EventArgs e)
        {
            _effect.InvokeTouchCancel(Element, EventArgs.Empty);
        }

        public void TouchDown(object sender, EventArgs e)
        {
            _effect.InvokeTouchDown(Element, EventArgs.Empty);
        }
    }
}
