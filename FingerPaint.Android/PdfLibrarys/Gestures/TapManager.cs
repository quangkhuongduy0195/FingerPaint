using Android.Content;
using Android.Views;
using FingerPaint.Droid.PdfLibrarys.Utilities;
using static Android.Views.GestureDetector;
namespace FingerPaint.Droid.PdfLibrarys.Gestures
{
    public class TapManager : View, IOnDoubleTapListener
    {
        private IOnTapListener _onTapListener;
        public TapManager(Context context, IOnTapListener onTapListener) : base(context)
        {
            _onTapListener = onTapListener;
        }


        /// <summary>
        /// Notified when a single-tap occurs.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool OnSingleTapConfirmed(MotionEvent e)
        {
            LogUtils.Log(nameof(TapManager), "OnSingleTapConfirmed");
            return true;
        }


        /// <summary>
        /// Notified when a double-tap occurs.
        /// </summary>
        /// <param name="e">The down motion event of the first tap of the double-tap.</param>
        /// <returns>true if the event is consumed, else false</returns>
        public bool OnDoubleTap(MotionEvent e)
        {
            LogUtils.Log(nameof(TapManager), $"OnDoubleTap : {e.GetX()} , {e.GetY()} ");
            _onTapListener.OnDoubleTap(e.GetX(), e.GetY());
            return true;
        }


        /// <summary>
        /// Notified when an event within a double-tap gesture occurs, including the down, move, and up events.
        /// </summary>
        /// <param name="e">The motion event that occurred during the double-tap gesture.</param>
        /// <returns>true if the event is consumed, else false</returns>
        public bool OnDoubleTapEvent(MotionEvent e)
        {
            LogUtils.Log(nameof(TapManager), $"OnDoubleTapEvent");
            return true;
        }
    }
}
