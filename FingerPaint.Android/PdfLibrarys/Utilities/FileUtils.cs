using System;
using Java.IO;
using Java.Lang;
using Java.Nio.Channels;
namespace FingerPaint.Droid.PdfLibrarys.Utilities
{
    public static class FileUtils
    {
        public static void Copy(File source, File dest)
        {
            try
            {
                FileChannel inputChannel = null;
                FileChannel outputChannel = null;
                try
                {
                    inputChannel = new FileInputStream(source).Channel;
                    outputChannel = new FileOutputStream(dest).Channel;
                    outputChannel.TransferFrom(inputChannel, 0, inputChannel.Size());
                }
                finally
                {
                    inputChannel.Close();
                    outputChannel.Close();

                }
            }
            catch (IOException ex)
            {
                throw ex;
            }


        }

        public static void CloseAutoCloseable(IAutoCloseable autoCloseable)
        {
            if (autoCloseable != null)
            {
                try
                {
                    autoCloseable.Close();
                }
                catch (Java.Lang.Exception ex)
                {

                }
            }
        }
    }
}
