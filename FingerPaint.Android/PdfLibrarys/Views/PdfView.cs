using Android;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content.Res;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using FingerPaint.Droid.PdfLibrarys.Gestures;
using FingerPaint.Droid.PdfLibrarys.Pdfs;
using FingerPaint.Droid.PdfLibrarys.Utilities;
using Java.Lang;
using static Android.Graphics.Paint;
using FormView = FingerPaint.Controls.Renderers;

namespace FingerPaint.Droid.PdfLibrarys.Views
{
    public class PdfView : View, IOnScrollListener, IOnScaleListener, IOnTapListener
    {
        //define const
        private const float ORIGINAL_SCALE = 1.0f;
        private const float MAX_SCALE = 4.0f;

        public GestureDetector.IOnDoubleTapListener OnDoubleTapListener { get; private set; }
        public IOnPdfLoadListener OnPdfLoadListener { get; set; }
        public IOnPdfPageChangeListener OnPdfPageChangeListener { get;  set; }
        private GestureManager _gestureManager;
        private GestureDetectorCompat _gestureDetectorCompat;
        private IOnPdfPageRenderListener _onPdfPageRenderListener;
        private AsyncTask _renderTask;
        private ScaleGestureDetector _scaleDetector;

        private PdfFile _pdfFile;
        private FormView.PDFView _formView;
        private Size _canvasSize;
        private PointF _offSet = new PointF(0f,0f);
        public SizeF _originalBitmap  = new SizeF(0, 0);
        public SizeF _currentBitmap = new SizeF(0, 0);
        private float _minScale;
        private float _maxScale;
        private float _currentScale;
        private Rect _sourceRect = new Rect();
        private Rect _destinationRect = new Rect();
        private Paint _pagePaint;
        private Paint _separatorPaint;
        private Paint _backgroundPaint;
        private bool _flinging = false;
        private PdfViewMode _viewMode = PdfViewMode.FIT_WIDTH;

        public int FlingStartX => (int)_offSet.X;
        public int FlingStartY => (int)_offSet.Y;
        public int FlingMinX
        {
            get
            {
                if (_currentBitmap.Width <= _canvasSize.Width)
                    return FlingStartX;
                return 0;
            }
        }
        public int FlingMaxX
        {
            get
            {
                if (_currentBitmap.Width <= _canvasSize.Width)
                    return FlingStartX;
                return (int)(_currentBitmap.Width - _canvasSize.Width);
            }
        }
        public int FlingMinY
        {
            get
            {
                if (_currentBitmap.Height * _pdfFile.PageCount <= _canvasSize.Height)
                {
                    return FlingStartY;
                }
                return 0;
            }
        }
        public int FlingMaxY
        {
            get
            {
                if (_currentBitmap.Height * _pdfFile.PageCount <= _canvasSize.Height)
                {
                    return FlingStartY;
                }
                return (int)(_currentBitmap.Height * _pdfFile.PageCount - _canvasSize.Height);
            }
        }

        public PdfView(Context context, FormView.PDFView formView) : base(context)
        {
            SetupPaint();

            _formView = formView;
            OnDoubleTapListener = new TapManager(Context, this);
            _gestureManager = new GestureManager(Context, this);
            _gestureDetectorCompat = new GestureDetectorCompat(Context, _gestureManager);
            _gestureDetectorCompat.SetOnDoubleTapListener(OnDoubleTapListener);
            _scaleDetector = new ScaleGestureDetector(Context, new ScaleManager(this));
            _onPdfPageRenderListener = new OnPdfPageRenderListener(this, false);
        }


        private void SetupPaint()
        {
            //Paint Page;
            _pagePaint = new Paint();

            //Separator Paint
            _separatorPaint = new Paint();
            _separatorPaint.SetStyle(Style.FillAndStroke);
            _separatorPaint.StrokeWidth = 10;
            _separatorPaint.Color = GetBackgroundColor();

            //Setup background Paint()
            _backgroundPaint = new Paint();
            _separatorPaint.SetStyle(Style.FillAndStroke);
            _backgroundPaint.Color = GetBackgroundColor();
        }


        public Color GetBackgroundColor()
        {
            var intColor = ResourcesCompat.GetColor(Context.Resources, Resource.Color.pdf_page_background, null);
            return Color.ParseColor("#" + Integer.ToHexString(intColor));
        }

        public void OnScale(float scaleFactor, float focusX, float focusY)
        {
            float previousScale = _currentScale;
            float newScale = scaleFactor * _currentScale;

            if (newScale >= _maxScale)
                SetCurrentScale(_maxScale);
            else if (newScale <= _minScale)
                SetCurrentScale(_minScale);
            else SetCurrentScale(newScale);

            if (_currentBitmap.Width <= _canvasSize.Width || _currentBitmap.Height * _pdfFile.PageCount <= _canvasSize.Height)
            {
                focusX = _canvasSize.Width / 2.0F;
                focusY = _canvasSize.Height / 2.0F;
            }
            float newOffsetX = (_offSet.X + focusX) * _currentScale / previousScale - focusX;
            float newOffsetY = (_offSet.Y + focusY) * _currentScale / previousScale - focusY;
            SetOffset(newOffsetX, newOffsetY);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            LogUtils.Log(nameof(PdfView), "OnTouchEvent");
            PerformClick();
            _scaleDetector.OnTouchEvent(e);

            if (_gestureDetectorCompat.OnTouchEvent(e))
                return true;

            return base.OnTouchEvent(e);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            LogUtils.Log(nameof(PdfView), $"OnTouchEvent : Width {MeasuredWidth} Height {MeasuredHeight}");
            //measure
            _canvasSize = new Size(MeasuredWidth, MeasuredHeight);
        }

        public override void ComputeScroll()
        {
            LogUtils.Log(nameof(PdfView), $"ComputeScroll");
            base.ComputeScroll();
            _gestureManager.ComputeScroll();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            LogUtils.Log(nameof(PdfView), $"OnDraw");

            if (_originalBitmap.Width <= 0 || _originalBitmap.Height <= 0)
                return;

            int pageFrom = GetPageFromInclusive(_offSet.Y);
            int pageTo = GetPageToExclusive(_offSet.Y);

            float x = -_offSet.X;
            float y = -(_offSet.Y - pageFrom * _currentBitmap.Height);


            for (int i = pageFrom; i < pageTo; ++i)
            {
                // draw PDF page
                Bitmap bitmap = _pdfFile.GetPage(i, !_flinging);
                if (bitmap != null)
                {
                    Rect dst = SetAndGetDestinationRect(x, y, _currentBitmap.Width + x, _currentBitmap.Height + y);
                    canvas.DrawBitmap(bitmap, SetAndGetSourceRect(), dst, _pagePaint);
                }

                //draw left and right side of background
                if (x > 0)
                {
                    canvas.DrawRect(0, Math.Max(0, y), x, _currentBitmap.Height + y, _backgroundPaint);
                    canvas.DrawRect(_canvasSize.Width - x, Math.Max(0, y), _canvasSize.Width, _currentBitmap.Height + y, _backgroundPaint);
                }

                //draw separator line
                if (i != 0)
                {
                    canvas.DrawLine(0, y, _canvasSize.Width, y, _separatorPaint);
                }

                if (i != 0)
                {
                    canvas.DrawLine(0, y, _canvasSize.Width, y, _separatorPaint);
                }
                y += _currentBitmap.Height;
            }

            //draw bottom of background
            if (y < _canvasSize.Height)
            {
                canvas.DrawRect(0, y, _canvasSize.Width, _canvasSize.Height, _backgroundPaint);
            }
        }

        private int GetPageFromInclusive(float offsetY)
        {
            var result = (int)System.Math.Floor(offsetY / _currentBitmap.Height);
            return result;
        }

        private int GetPageToExclusive(float offsetY)
        {
            var result = Math.Min(_pdfFile.PageCount, (int)Math.Ceil((offsetY + _canvasSize.Height) / _currentBitmap.Height));
            return result;
        }

        private int GetCurrentPageNumber => (int)System.Math.Floor((_offSet.Y + _canvasSize.Height / 2.0F) / _currentBitmap.Height);


        private void NotifyPageNumber()
        {
            if (OnPdfPageChangeListener != null)
            {
                OnPdfPageChangeListener.AtPage(GetCurrentPageNumber, _pdfFile.PageCount);
            }
        }

        private void NotifyPdfLoaded()
        {
            if (OnPdfLoadListener != null)
            {
                OnPdfLoadListener.OnLoaded();
            }
        }

        public void OnDoubleTap(float x, float y)
        {
            float newScale = (_currentScale == _maxScale ? _minScale : _maxScale);
            OnScale(newScale / _currentScale, x, y);
        }

        public void OnDrag(float deltaX, float deltaY)
        {
            LogUtils.Log(nameof(PdfView), $"OnDrag : {deltaX}/{deltaY}");
            AdjustOffset(deltaX, deltaY);
            NotifyPageNumber();
        }

        public void OnFling(float currentX, float currentY, float finalX, float finalY)
        {
            bool finished = (currentX == finalX && currentY == finalY);
            if (finished && !_flinging)
            {
                return;
            }
            _flinging = !finished;
            SetOffset(currentX, currentY);
            if (_flinging)
            {
                NotifyPageNumber();
            }
        }

        public void OnFlingPaused()
        {
            LogUtils.Log(nameof(PdfView), $"OnFlingPaused ");
            _flinging = false;
            Redraw(true);
        }

        public void SetPdfFile(PdfFile pdfFile)
        {
            _pdfFile = pdfFile;
            SetupOriginalBitmap();
        }

        public void JumpToPage(int pageNumber)
        {
            LogUtils.Log(nameof(PdfView), $"JumpToPage : {pageNumber} ");
            if (pageNumber < 0 || pageNumber >= _pdfFile.PageCount)
            {
                return;
            }
            SetOffsetY(pageNumber * _currentBitmap.Height);
            Redraw(true);
        }

        public void Refresh()
        {
            var handler = new Handler(Context.MainLooper);
            handler.PostDelayed(new Runnable(() =>
            {
               SetupInitialBitmap();
            }), 500);
        }

        private void ResetScaleRange()
        {
            _minScale = ORIGINAL_SCALE;
            _maxScale = MAX_SCALE;
        }

        private void SetupOriginalBitmap()
        {
            LogUtils.Log(nameof(PdfView), $"SetupOriginalBitmap ");
            _pdfFile.GetPages(0, 1, new OnPdfPageRenderListener(this, true));
        }

        private float GetFitWidthScale()
        {
            return _canvasSize.Width / _originalBitmap.Width;
        }

        private float GetFitWidthAndHeightScale()
        {
            return Math.Min(_canvasSize.Width / _originalBitmap.Width, _canvasSize.Height / _originalBitmap.Height);
        }

        public void SetupInitialBitmap()
        {
            ResetScaleRange();
            switch (_viewMode)
            {
                case PdfViewMode.ORIGINAL:
                    SetCurrentScale(ORIGINAL_SCALE);
                    break;
                case PdfViewMode.FIT_WIDTH:
                    SetCurrentScale(GetFitWidthScale());
                    break;
                case PdfViewMode.FIT_WIDTH_AND_HEIGHT:
                    SetCurrentScale(GetFitWidthAndHeightScale());
                    break;
            }

            float fitWidthScale = GetFitWidthScale();
            float fitWidthAndHeightScale = GetFitWidthAndHeightScale();
            float minScale = Math.Min(ORIGINAL_SCALE, Math.Min(fitWidthScale, fitWidthAndHeightScale));
            float maxScale = Math.Max(ORIGINAL_SCALE, Math.Max(fitWidthScale, fitWidthAndHeightScale));
            _minScale = Math.Min(_minScale, minScale);
            _maxScale = Math.Max(_maxScale, maxScale);

            SetOffsetX(0);
            SetOffsetY(0);

            NotifyPdfLoaded();
            NotifyPageNumber();
            Redraw(true);
        }

        private void SetCurrentScale(float maxScale)
        {
            _currentScale = maxScale;
            _currentBitmap = new SizeF(_currentScale * _originalBitmap.Width, _currentScale * _originalBitmap.Height);
        }

        private Rect SetAndGetSourceRect()
        {
            _sourceRect.Set(0, 0, (int)_originalBitmap.Width, (int)_originalBitmap.Height);
            return _sourceRect;
        }


        private Rect SetAndGetDestinationRect(float left, float top, float right, float bottom)
        {
            _destinationRect.Set((int)left, (int)top, (int)right, (int)bottom);
            return _destinationRect;
        }

        private void SetOffsetX(float offsetX)
        {
            if (_currentBitmap.Width <= _canvasSize.Width)
            {
                _offSet.X = (_currentBitmap.Width - _canvasSize.Width) / 2.0F;
            }
            else
            {
                float min = 0;
                float max = _currentBitmap.Width - _canvasSize.Width;
                _offSet.X = Math.Max(min, Math.Min(max, offsetX));
            }

            UpdateFromScrollChanged(_offSet.X, _offSet.Y);
        }

        private void SetOffsetY(float offsetY)
        {
            if (_currentBitmap.Height * _pdfFile.PageCount <= _canvasSize.Height)
            {
                _offSet.Y = 0;
            }
            else
            {
                float min = 0;
                float max = _currentBitmap.Height * _pdfFile.PageCount - _canvasSize.Height;
                _offSet.Y = Math.Max(min, Math.Min(max, offsetY));
            }

            UpdateFromScrollChanged(_offSet.X, _offSet.Y);
        }

        private void SetOffset(float newOffsetX, float newOffsetY)
        {
            SetOffsetX(newOffsetX);
            SetOffsetY(newOffsetY);
            Redraw(false);
        }

        private void AdjustOffset(float deltaX, float deltaY)
        {
            //Adjust Offset X, Y
            SetOffsetY(_offSet.Y + deltaY);
            SetOffsetX(_offSet.X + deltaX);
            Redraw(true);
        }


        private void Redraw(bool forceDraw)
        {
            if (forceDraw || _flinging)
            {
                 Invalidate();
            }
            else
            {
                _renderTask?.Cancel(true);
                _renderTask = _pdfFile.GetPages(GetPageFromInclusive(_offSet.Y), GetPageToExclusive(_offSet.Y), _onPdfPageRenderListener);
            }
        }

        public void UpateOrginBitMap(float originalBitmapWidth, float originalBitmapHeight)
        {
            _originalBitmap = new SizeF(originalBitmapWidth, originalBitmapHeight);
            SetupInitialBitmap();
        }

        public void EndScale()
        {
            UpdateFromScale(_currentScale);
        }

        public override void Invalidate()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                base.Invalidate();
            });
        }


        #region Update Form Control
        private void UpdateFromScale(float newScale)
        {
            _formView.CurrentScale = newScale;
            _formView.RaiseScaleChanged(newScale);
        }

        private void UpdateFromScrollChanged(float offsetX, float offsetY)
        {
            var offSetNew = new Xamarin.Forms.Point(offsetX, offsetY);
            _formView.ScrollOffset = offSetNew;
            _formView.RaiseScrollChanged(offSetNew);
        }
        #endregion
    }
}
