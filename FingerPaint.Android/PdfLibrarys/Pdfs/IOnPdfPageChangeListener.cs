using System;
namespace FingerPaint.Droid.PdfLibrarys.Pdfs
{
    public interface IOnPdfPageChangeListener
    {
        void AtPage(int currentPageNumber, int totalPageNumber);
    }
}
