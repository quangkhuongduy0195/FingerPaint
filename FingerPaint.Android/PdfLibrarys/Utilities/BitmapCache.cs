using System;
using Android.Graphics;
using Android.Util;
using Java.Lang;
namespace FingerPaint.Droid.PdfLibrarys.Utilities
{
    public class BitmapCache
    {
        private readonly LruCache _memoryCache;
        private const int MB = 1024;
        public BitmapCache()
        {
            // Get max available VM memory, exceeding this amount will throw an
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / MB);

            // Use 1/8th of the available memory for this memory cache.
            int cacheSize = maxMemory / 8;
            _memoryCache = new PDFLruCache(cacheSize);
        }

        public void AddBitmapToMemoryCache(Java.Lang.String key, Bitmap bitmap)
        {
            if (bitmap != null && GetBitmapFromMemCache(key) == null)
            {
                _memoryCache.Put(key, bitmap);
            }
        }

        public Bitmap GetBitmapFromMemCache(Java.Lang.String key)
        {
            return _memoryCache.Get(key) as Bitmap;
        }
    }

    public class PDFLruCache : LruCache
    {
        public PDFLruCache(int maxSize) : base(maxSize)
        {

        }

        protected override int SizeOf(Java.Lang.Object key, Java.Lang.Object value)
        {
            if (value is Bitmap bitmap)
            {
                return bitmap.ByteCount / 1024;
            }
            throw new ArgumentException("Value must is Bitmap type and not null!");
        }
    }
}
