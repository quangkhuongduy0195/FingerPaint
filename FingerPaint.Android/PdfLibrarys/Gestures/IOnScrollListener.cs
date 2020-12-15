using System;
namespace FingerPaint.Droid.PdfLibrarys.Gestures
{
    public interface IOnScrollListener
    {
        int FlingStartX { get; }
        int FlingStartY { get; }
        int FlingMinX { get; }
        int FlingMaxX { get; }
        int FlingMinY { get; }
        int FlingMaxY { get; }
        void OnDrag(float deltaX, float deltaY);
        void OnFling(float currentX, float currentY, float finalX, float finalY);
        void OnFlingPaused();
    }
}
