using Prism.Mvvm;

namespace FingerPaint.Models
{
    public class FilesPDFModel : BindableBase
    {
        public string ItemName { get; set; }

        public string Url { get; set; }

        public FilesPDFModel()
        {
        }
    }
}
