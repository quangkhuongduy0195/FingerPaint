using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace FingerPaint.Pages
{
    public partial class PDFListPage : ContentPage
    {
        public PDFListPage()
        {
            InitializeComponent();
        }

        void ListView_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (!(sender is ListView listView))
                return;

            listView.SelectedItem = null;
        }
    }
}
