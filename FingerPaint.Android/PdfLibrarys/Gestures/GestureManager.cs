using System;
using System.Diagnostics;
using Android.Content;
using Android.Views;
using Android.Widget;
using FingerPaint.Droid.PdfLibrarys.Utilities;
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


        /// <summary>
        /// Notified when a tap occurs with the down MotionEvent that triggered it.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool OnDown(MotionEvent e)
        {
            LogUtils.Log(nameof(GestureManager), "OnDown");

            if (!_overScroller.IsFinished)
            {
                _overScroller.ForceFinished(true);
                _onScrollListener.OnFlingPaused();
            }
            return true;
        }


        /// <summary>
        /// Notified when a long press occurs with the initial on down MotionEvent that trigged it.
        /// </summary>
        /// <param name="e"></param>
        public void OnShowPress(MotionEvent e)
        {
            LogUtils.Log(nameof(GestureManager), "OnShowPress");
        }


        /// <summary>
        /// Notified when a tap occurs with the up MotionEvent that triggered it.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool OnSingleTapUp(MotionEvent e)
        {
            LogUtils.Log(nameof(GestureManager), "OnSingleTapUp");
            return true;
        }


        /// <summary>
        /// Notified when a scroll occurs with the initial on down MotionEvent and the current move MotionEvent.
        /// The distance in x and y is also supplied for convenience.
        /// </summary>
        /// <param name="e1">The first down motion event that started the scrolling.</param>
        /// <param name="e2">The move motion event that triggered the current onScroll.</param>
        /// <param name="distanceX">The distance along the X axis that has been scrolled since the last call to onScroll.
        /// This is NOT the distance between e1 and e2.</param>
        /// <param name="distanceY">The distance along the Y axis that has been scrolled since the last call to onScroll.
        /// This is NOT the distance between e1 and e2.</param>
        /// <returns>true if the event is consumed, else false</returns>
        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            LogUtils.Log(nameof(GestureManager), $"OnScroll : {distanceX}, {distanceY}");
            _onScrollListener.OnDrag(distanceX, distanceY);
            return true;
        }

        public void OnLongPress(MotionEvent e)
        {
            LogUtils.Log(nameof(GestureManager), "OnLongPress");
        }


        /// <summary>
        /// Notified of a fling event when it occurs with the initial on down MotionEvent and the matching up MotionEvent.
        /// The calculated velocity is supplied along the x and y axis in pixels per second.
        /// </summary>
        /// <param name="e1">The first down motion event that started the fling.</param>
        /// <param name="e2">The move motion event that triggered the current onFling.</param>
        /// <param name="velocityX">The velocity of this fling measured in pixels per second along the x axis.</param>
        /// <param name="velocityY">The velocity of this fling measured in pixels per second along the y axis.</param>
        /// <returns></returns>
        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            LogUtils.Log(nameof(GestureManager), "OnFling");

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
            LogUtils.Log(nameof(GestureManager), "ComputeScroll");
            if (_overScroller.ComputeScrollOffset())
            {
                _onScrollListener.OnFling(_overScroller.CurrX, _overScroller.CurrY, _overScroller.FinalX, _overScroller.FinalY);
            }
        }
    }
}
