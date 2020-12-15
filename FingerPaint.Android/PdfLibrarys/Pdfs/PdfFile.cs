
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using FingerPaint.Droid.PdfLibrarys.Utilities;
using Java.IO;
using Java.Lang;
using static Android.Graphics.Pdf.PdfRenderer;
namespace FingerPaint.Droid.PdfLibrarys.Pdfs
{
    public class PdfFile
    {
        private File _pdfFile;
        private BitmapCache _pageCache;
        public bool IsValid => PageCount > 0;
        public int PageCount { get; private set; }

        public PdfFile(File pdfFile)
        {
            try
            {
                _pdfFile = File.CreateTempFile("PDF", null);
                FileUtils.Copy(pdfFile, _pdfFile);
            }
            catch (IOException ex)
            {
                //handle if need
            }
            InitPageCount();
            _pageCache = new BitmapCache();
        }

        private void InitPageCount()
        {
            ParcelFileDescriptor parcelFileDescriptor = null;
            PdfRenderer pdfRenderer = null;
            try
            {
                parcelFileDescriptor = GetSeekableFileDescriptor();
                pdfRenderer = GetPdfRenderer(parcelFileDescriptor);
                PageCount = pdfRenderer.PageCount;
            }
            catch (IOException e)
            {
                PageCount = 0;
            }
            finally
            {
                parcelFileDescriptor.Close();
                pdfRenderer.Close();
            }
        }

        private PdfRenderer GetPdfRenderer(ParcelFileDescriptor parcelFileDescriptor)
        {
            return new PdfRenderer(parcelFileDescriptor);
        }

        private ParcelFileDescriptor GetSeekableFileDescriptor()
        {
            return ParcelFileDescriptor.Open(_pdfFile, ParcelFileMode.ReadOnly);
        }

        public AsyncTask GetPages(int pageFrom, int pageTo, IOnPdfPageRenderListener listener)
        {
            if (pageFrom == pageTo)
                return null;

            if (pageFrom > pageTo)
                return GetPages(pageTo, pageFrom, listener);

            if (pageFrom < 0 || pageFrom >= PageCount || pageTo > PageCount || !_pdfFile.Exists())
                return null;
            return new GetPageAsyncTask(GetPages, listener).Execute(pageFrom, pageTo);
        }

        public Bitmap GetPage(int pageNum, bool renderIfNotExist)
        {
            string cacheKey = GetCacheKey(pageNum);
            Bitmap bitmap = _pageCache.GetBitmapFromMemCache(new Java.Lang.String(cacheKey));
            if (bitmap != null || !renderIfNotExist)
                return bitmap;

            Bitmap[] bitmaps = GetPages(pageNum, pageNum + 1);
            return (bitmaps == null ? null : bitmaps[0]);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Bitmap[] GetPages(int pageFrom, int pageTo)
        {
            if (pageFrom == pageTo)
            {
                return null;
            }
            if (pageFrom > pageTo)
            {
                return GetPages(pageTo, pageFrom);
            }
            if (pageFrom < 0 || pageFrom >= PageCount || pageTo > PageCount)
            {
                return null;
            }

            Bitmap[] bitmaps = new Bitmap[pageTo - pageFrom];

            float quality = 2.0F;

            ParcelFileDescriptor parcelFileDescriptor = null;
            PdfRenderer pdfRenderer = null;

            try
            {
                parcelFileDescriptor = GetSeekableFileDescriptor();
                pdfRenderer = GetPdfRenderer(parcelFileDescriptor);

                for (int i = 0; i < bitmaps.Length; ++i)
                {
                    string cacheKey = GetCacheKey(i + pageFrom);
                    Bitmap bitmap = _pageCache.GetBitmapFromMemCache(new Java.Lang.String(cacheKey));

                    if (bitmap == null)
                    {
                        Page page = pdfRenderer.OpenPage(i + pageFrom);

                        int width = Java.Lang.Math.Round(page.Width * quality);
                        int height = Java.Lang.Math.Round(page.Height * quality);
                        bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

                        page.Render(bitmap, null, null, PdfRenderMode.ForDisplay);

                        page.Close();
                        _pageCache.AddBitmapToMemoryCache(new Java.Lang.String(cacheKey), bitmap);
                    }
                    bitmaps[i] = bitmap;
                }
            }
            catch (IOException ex)
            {
                //log
            }
            finally
            {
                parcelFileDescriptor.Close();
                pdfRenderer.Close();
            }
            return bitmaps;
        }

        private string GetCacheKey(int pageNum)
        {
            return Java.Lang.String.Format("PdfPage%04d", pageNum);
        }
    }

    public class GetPageAsyncTask : AsyncTask<Integer, Action, Bitmap[]>
    {
        Func<int, int, Bitmap[]> _funcGetPage;
        IOnPdfPageRenderListener _listener;
        public GetPageAsyncTask(Func<int, int, Bitmap[]> funcGetPage, IOnPdfPageRenderListener listener)
        {
            _funcGetPage = funcGetPage;
            _listener = listener;
        }

        protected override Bitmap[] RunInBackground(params Integer[] @params)
        {
            var result = _funcGetPage.Invoke(@params[0].IntValue(), @params[1].IntValue());
            _listener.OnRendered(result);
            return result;
        }

        protected override void OnPostExecute([AllowNull] Bitmap[] result)
        {
            if (_listener != null)
            {
                _listener.OnRendered(result);
            }
        }
    }
}
