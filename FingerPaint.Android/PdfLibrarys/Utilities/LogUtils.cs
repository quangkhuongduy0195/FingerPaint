using System;
using System.Diagnostics;

namespace FingerPaint.Droid.PdfLibrarys.Utilities
{
    public static class LogUtils
    {
        public static void Log(string tag ,string message)
        {
            Debug.WriteLine(message, tag);
        }
    }
}
