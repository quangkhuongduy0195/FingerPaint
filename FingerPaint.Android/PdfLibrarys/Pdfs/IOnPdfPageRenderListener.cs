using System;
using Android.Graphics;

namespace FingerPaint.Droid.PdfLibrarys.Pdfs
{
    public interface IOnPdfPageRenderListener
    {
        void OnRendered(params Bitmap[] bitmaps);
    }
}
