using System;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;
using Plugin.Media;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace NeuralNetwork
{
    // Opens a page that allows the user to take a photo of a handwritten figure and to test it with the neural net
    public class CameraTestPage : ContentPage
    {
        // Neural net
        NeuralNet neuralnet;

        // Canvasview needed for drawing 
        SKCanvasView canvasview;

        // Bitmap of the digit (recorded with the smartphone cam)
        public SKBitmap bitmap = null;

        // Grayscaled pixel array of digit (to be used as input for neural net, generated from bitmap)
        byte[] MNISTpixelarray = new byte[28 * 28];

        // Contrast applied to grayscale transformation (to sharpen the lines in the picture)
        const float contrast = 0.9f;

        // Flag if guidelines for photos have been shown 
        static bool flagguidelinesphoto = false;

        public CameraTestPage(NeuralNet neuralnet)
        {
            // Save neural net
            this.neuralnet = neuralnet;

            // Define GUI

            // Page properties
            Title = "Handwriting";
            BackgroundColor = Color.SteelBlue;
            Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);

            // Define label
            Label description = new Label
            {
                Text = "Take a picture of a single digit (0..9) with your phone camera or draw it directly with your finger on the screen"
            };

            // Define button to take pictures of digits
            Button buttoncam = new Button
            {
                Text = "Take a Picture",
                WidthRequest = Application.Current.MainPage.Width * 0.8,
                HeightRequest = Application.Current.MainPage.Height * 0.10,
                VerticalOptions = LayoutOptions.Center
            };
            buttoncam.Clicked += TakeAndShowPicture;

            // Define button to draw a digit
            Button buttondraw = new Button
            {
                Text = "Draw with Finger",
                WidthRequest = Application.Current.MainPage.Width * 0.8,
                HeightRequest = Application.Current.MainPage.Height * 0.10,
                VerticalOptions = LayoutOptions.Center
            };
            buttondraw.Clicked += async (s, e) =>
            {
                // Open page to draw digit
                // Note: DrawDigitPage writes result to "bitmap"
                await Navigation.PushAsync(new DrawDigitPage());
            };

            // Define button to ask net to guess the digit
            Button buttonasknet = new Button
            {
                Text = "Ask Neural Net",
                WidthRequest = Application.Current.MainPage.Width * 0.8,
                HeightRequest = Application.Current.MainPage.Height * 0.10,
                VerticalOptions = LayoutOptions.Center,
            };
            buttonasknet.Clicked += AskNeuralNet;

            // Define canvas for showing the picture of the digit
            canvasview = new SKCanvasView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            canvasview.PaintSurface += OnCanvasViewPaintSurface;

            Content = new StackLayout
            {
                Children = { description, buttoncam, buttondraw, canvasview, buttonasknet },
                Spacing = Application.Current.MainPage.Height * 0.05
            };

            // Convert bitmap and update canvas when page appears after drawing page
            Appearing += (s, e) =>
            {
                if (bitmap == null)
                    return;

                // Check if bitmap is not squared and not 28x28 px
                if ((bitmap.Width != bitmap.Height) || bitmap.Width!=28)
                {
                    // Convert bitmap to 28x28 pixel grayscale
                    bitmap = ConvertBitmap(bitmap);

                    // Update canvas
                    canvasview.InvalidateSurface();                    
                }
            };
        }

        // Take picture with camera, transform it to grayscale and show it on canvas
        async void TakeAndShowPicture(object sender, EventArgs args)
        {
            if (flagguidelinesphoto == false)
            {
                await DisplayAlert("To achieve suitable results...",
                                   "- write on white background\n" +
                                   "- use thick stroke\n" +
                                   "- center with camera zoom\n" +
                                   "- avoid shadows\n" +
                                   "- fill 80% of height",
                                   "OK");
                flagguidelinesphoto = true;
            }

            // Initialite cam handling
            await CrossMedia.Current.Initialize();

            // Check if cam is available
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Error", "No camera available.", "OK");
                return;
            }

            // Define options for camera plugin
            var options = new Plugin.Media.Abstractions.StoreCameraMediaOptions();

            // On iOS it's possible to take a squared pic rendered to 28x28 directly
            if (Device.RuntimePlatform == Device.iOS)
            {
                options.AllowCropping = true;
                options.PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight;
                options.MaxWidthHeight = 28;               
            }

            // Take picture
            var photo = await CrossMedia.Current.TakePhotoAsync(options);

            // Convert to resized (28x28) grayscaled bitmap
            if (photo != null)
            {
                // Generate bitmap from media object
                using (SKManagedStream skstream = new SKManagedStream(photo.GetStream()))
                {
                    bitmap = SKBitmap.Decode(skstream);
                }

                // Convert bitmap to 28x28 pixel grayscale
                bitmap = ConvertBitmap(bitmap);
            }

            // Update canvas view
            canvasview.InvalidateSurface();
        }     

        // Convert bitmap:
        // 1. Trim to square
        // 2. Resize to 28x28 pixels
        // 3. Grayscale with high contrast
        SKBitmap ConvertBitmap(SKBitmap bitmap, float contr = contrast)
        {
            // 0. bitmap initialized?
            if (bitmap == null)
                return null;

            // 1. Square bitmap (if not done by media plugin)
            if (bitmap.Width != bitmap.Height)
            {
                // Calculate size and start coordinates of square
                int size = Math.Min(bitmap.Width, bitmap.Height);
                int left = (bitmap.Width - size) / 2;
                int top = (bitmap.Height - size) / 2;

                // Cut centered square
                bitmap.ExtractSubset(bitmap, new SKRectI(left, top, left + size, top + size));
            }

            // 2. Resize to 28x28 pixels (if not done by media plugin)
            if (bitmap.Width != 28)
            {
                SKBitmap bitmap_copy = bitmap.Copy();
                bitmap = new SKBitmap(28, 28, bitmap.ColorType, bitmap.AlphaType);
                bitmap_copy.Resize(bitmap, SKBitmapResizeMethod.Box);
            }

            // 3. Convert bitmap to grayscale and apply contrast to emphasize lines
            // Second grayscale conversion to highlight pen color again which may be suppressed by resizing
            bitmap = ConvertBitmapToGray(bitmap, contr);

            return bitmap;
        }

        // Convert bitmap to grayscale and apply contrast to emphasize lines
        // For conversion SKColorFilter is used
        // However, SKColorFilter is usually set in SKPaint object and applied to canvas when drawn
        // To apply it to the bitmap, we have to convert bitmap to image because it's possible to apply filters to images
        SKBitmap ConvertBitmapToGray(SKBitmap bitmap, float contr = contrast)
        {
            SKImage image = SKImage.FromBitmap(bitmap);
            SKImageFilter imagefilter = SKImageFilter.CreateColorFilter(SKColorFilter.CreateHighContrast(true, SKHighContrastConfigInvertStyle.NoInvert, contrast));
            SKRectI rectout = new SKRectI();
            SKPoint pointout = new SKPoint();
            image = image.ApplyImageFilter(imagefilter, new SKRectI(0, 0, image.Width, image.Height), new SKRectI(0, 0, image.Width, image.Height), out rectout, out pointout);

            return SKBitmap.FromImage(image);            
        }

        // Draw bitmap
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            // Center picture on canvas without stretching
            int minside = Math.Min(info.Width, info.Height);
            float left = (info.Width - minside) * 0.5f;
            float top = (info.Height - minside) * 0.5f;
            float right = left + minside;
            float bottom = top + minside;

            using (SKPaint paint = new SKPaint())
            {
                if (bitmap != null)
                {   
                    canvas.DrawBitmap(bitmap, new SKRect(left, top, right, bottom), paint);
                }
                else
                {
                    // Draw placeholder if no bitmap available
                    // Paint box placeholder
                    paint.Color = Color.LightSteelBlue.ToSKColor();
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawRect(new SKRect(left, top, right, bottom), paint);

                    // Write text
                    string text = "No picture available";
                    paint.Color = Color.GhostWhite.ToSKColor();
                    paint.Style = SKPaintStyle.StrokeAndFill;
                    paint.TextAlign = SKTextAlign.Center;

                    // Adjust TextSize property so text is 80% of minside
                    float textWidth = paint.MeasureText(text);
                    paint.TextSize = 0.8f * minside * paint.TextSize / textWidth;

                    canvas.DrawText(text, left + minside / 2, top + minside / 2, paint);
                }
            }
        }

        // Ask neural net what digit is captured in the bitmap 
        void AskNeuralNet(object obj, EventArgs args)
        {
            // Check if bitmap is existent
            if (bitmap == null)
            {
                DisplayAlert("Information", "Please take a picture of a digit first.", "OK");
                return;
            }

            // Generate MNIST pixel data from bitmap (if existent)
            GenerateMNISTPixelData();

            // Generate input vector for neural net
            Vector<double> input = Vector<double>.Build.Dense(28 * 28, 0.01);
            for (int i = 0; i < MNISTpixelarray.Length; i++)
                input[i] += Convert.ToDouble(MNISTpixelarray[i]) / 255 * 0.99;

            // Ask neural net
            Vector<double> answer = neuralnet.Query(input);

            // Output
            Navigation.PushAsync(new ResultsCamPage(neuralnet, input, answer));
        }

        // Generate the MNIST pixel data array from (greyscaled) bitmap
        void GenerateMNISTPixelData()
        {
            if (bitmap == null)
                return;

            if (bitmap.Width != 28 || bitmap.Height != 28)
                return;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // Due to grayscale all pixels have same RGB values, however, we generate the average (we could also take the RED value, for example)
                    // Furthermore, in MNIST 255 is black and 0 is zero, so we have to "mirror" the pixel values
                    MNISTpixelarray[y * 28 + x] = (byte)(255 - (bitmap.GetPixel(x, y).Red + bitmap.GetPixel(x, y).Green + bitmap.GetPixel(x, y).Blue) / 3);
                }
            }
        }
    }
}
