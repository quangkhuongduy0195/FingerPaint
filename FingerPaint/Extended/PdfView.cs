using System;
using Xamarin.Forms;

namespace FingerPaint.Extended
{
    public class PdfView : View
    {
        #region Base64Data

        public static BindableProperty Base64DataProperty = BindableProperty.Create(nameof(Base64Data), typeof(string), typeof(PdfView), default(string), BindingMode.Default);

        public string Base64Data
        {
            get => (string)GetValue(Base64DataProperty);
            set => SetValue(Base64DataProperty, value);
        }

        #endregion
    }
}
