using System;
using System.Diagnostics;
using Android.Content;
using Android.Views;
using Android.Widget;
using static Android.Views.GestureDetector;
namespace FingerPaint.Droid.PdfLibrarys.Gestures
{
    public class GestureManager : View, IOnGestureListener
    {
        private IOnScrollListener _onScrollListener;
        private OverScroller _overScroller;

        public GestureManager(Context context, IOnScrollListener onScrollListener) : base(context)
        {
            _overScroller = new OverScroller(context);
            _onScrollListener = onScrollListener;
        }

        public bool OnDown(MotionEvent e)
        {
            if (!_overScroller.IsFinished)
            {
                _overScroller.ForceFinished(true);
                _onScrollListener.OnFlingPaused();
            }
            return true;
        }

        public void OnShowPress(MotionEvent e)
        {
            //do not thing
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return true;
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            _onScrollListener.OnDrag(distanceX, distanceY);
            return true;
        }

        public void OnLongPress(MotionEvent e)
        {
            //do not thing
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            _overScroller.ForceFinished(true);

            int startX = _onScrollListener.FlingStartX;
            int startY = _onScrollListener.FlingStartY;
            int minX = _onScrollListener.FlingMinX;
            int maxX = _onScrollListener.FlingMaxX;
            int minY = _onScrollListener.FlingMinY;
            int maxY = _onScrollListener.FlingMaxY;

            _overScroller.Fling(startX, startY, (int)-velocityX, (int)-velocityY, minX, maxX, minY, maxY);
            ComputeScroll();

            return true;
        }

        public override void ComputeScroll()
        {
            if (_overScroller.ComputeScrollOffset())
            {
                _onScrollListener.OnFling(_overScroller.CurrX, _overScroller.CurrY, _overScroller.FinalX, _overScroller.FinalY);
            }
        }
    }
}
