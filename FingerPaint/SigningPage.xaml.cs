using System;
using System.Collections.Generic;
using System.IO;
using FingerPaint.Effects;
//using System.Drawing;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FingerPaint
{
    public partial class SigningPage : ContentPage
    {
        public SigningPage()
        {
            InitializeComponent();
            /*
             * var assembly = IntrospectionExtensions.GetTypeInfo(GetType()).Assembly;
            var path = "sbicfd.Logics.licenses.json";// no "-"
             */
        }

        public string PdfData { get; internal set; }

        public byte[] ImageBinary { get; internal set; }

        //private Image _image = new Image();
        //private Point _imagePosition = new Point();
        private TouchEffect _touchEffect = new TouchEffect();

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Viewer.Base64Data = PdfData;
            //_image = GetImageFromBinary(_image, ImageBinary);
            _touchEffect.TouchUpInside += TouchEffect_TouchUpInside;
            Viewer.Effects.Add(_touchEffect);
        }

        private void TouchEffect_TouchUpInside(object sender, TouchEventArgs e)
        {
            //_touchEffect.TouchedPosition.X += 10;
            //_touchEffect.TouchedPosition.Y += 10;
            var image = new Image();
            image = GetImageFromBinary(image, ImageBinary);
            image.BackgroundColor = Color.Aqua;
            //AbsoluteLayout.SetLayoutBounds(image, new Rectangle(_touchEffect.TouchedPosition.X, _touchEffect.TouchedPosition.Y, 150, 75));
            AbsoluteLayout.SetLayoutBounds(image, new Rectangle(e.TouchedPosition.X, e.TouchedPosition.Y, 150, 75));
            AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.None);
            ParentView.Children.Add(image);
        }

        private Image GetImageFromBinary(Image image, byte[] imageBinary)
        {
            var rootDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(rootDir, "test.png");
            var existed = File.Exists(path);
            if (existed)
                File.Delete(path);
            using (MemoryStream ms = new MemoryStream(imageBinary))
            {
                File.WriteAllBytes(path, ms.ToArray());
            }
            image.Source = path;
            return image;
        }
    }
}
