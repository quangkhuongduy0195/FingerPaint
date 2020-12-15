using System;
namespace FingerPaint.Droid.PdfLibrarys.Gestures
{
    public interface IOnScaleListener
    {
        void OnScale(float scaleFactor, float focusX, float focusY);
        void EndScale();
    }
}
