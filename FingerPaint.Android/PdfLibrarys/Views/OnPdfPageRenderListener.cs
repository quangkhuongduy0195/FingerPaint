using System;
using Android.Graphics;
using FingerPaint.Droid.PdfLibrarys.Pdfs;
using Java.Lang;

namespace FingerPaint.Droid.PdfLibrarys.Views
{
    public class OnPdfPageRenderListener : IOnPdfPageRenderListener
    {
        Action _invalidate;
        Action<float, float> _updateOrignBitmap;

        public OnPdfPageRenderListener(PdfView pdfView, bool isUpdateBitMap)
        {
            if(isUpdateBitMap)
            {
                _updateOrignBitmap = pdfView.UpateOrginBitMap;
            }
            else
            {
                _invalidate = pdfView.Invalidate;
            }
        }

        public void OnRendered(params Bitmap[] bitmaps)
        {
            _updateOrignBitmap?.Invoke(bitmaps[0].Width, bitmaps[0].Height);
            _invalidate?.Invoke();
        }
    }
}
