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
        SKBitmap bitmap;

        // Grayscaled pixel array of digit (to be used as input for neural net, generated from bitmap)
        byte[] MNISTpixelarray = new byte[28 * 28];

        // Contrast applied to grayscale transformation (to sharpen the lines in the picture)
        float contrast = 0.8f;

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
                Text = "Here you can test the neural net with your own handwriting!\n" +
                    "Just take a picture of a single digit (0..9) written on a white background. The figure should be centered and 80% of the vertical picture size.",
            };

            // Define button to take pictures of digits
            Button buttoncam = new Button
            {
                Text = "Take Picture of Digit",
                WidthRequest = Application.Current.MainPage.Width * 0.5,
                HeightRequest = Application.Current.MainPage.Height * 0.10,
                VerticalOptions = LayoutOptions.Center
            };
            buttoncam.Clicked += TakeAndShowPicture;

            // Define button to aks net to guess the digit
            Button buttonasknet = new Button
            {
                Text = "Ask Neural Net",
                WidthRequest = Application.Current.MainPage.Width * 0.5,
                HeightRequest = Application.Current.MainPage.Height * 0.10,
                VerticalOptions = LayoutOptions.Center,
                //Margin = new Thickness(0,0,0,Application.Current.MainPage.Height * 0.05)
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
                Children = { description, buttoncam, canvasview, buttonasknet },
                Spacing = Application.Current.MainPage.Height * 0.05
            };
        }

        // Take picture with camera, transform it to grayscale and show it on canvas
        async void TakeAndShowPicture(object sender, EventArgs args)
        {
            // Initialite cam handling
            await CrossMedia.Current.Initialize();

            // Check if cam is available
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Error", "No camera available.", "OK");
                return;
            }

            // Take a pic
            var photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                // Save pic automatically to album
                //SaveToAlbum = true,
                // Crop pic to square format (works on iOS only)
                AllowCropping = true,
                // Convert pic to 28x28 pic
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 28,
            });

            // Convert to grayscaled bitmap
            if (photo != null)
            {
                // Generate bitmap
                using (SKManagedStream skstream = new SKManagedStream(photo.GetStream()))
                {
                    bitmap = SKBitmap.Decode(skstream);
                }

                // Convert bitmap to grayscale and apply contrast to emphasize lines
                // For conversion SKColorFilter is used
                // However, SKColorFilter is usually set in SKPaint object and applied to canvas when drawn
                // To apply it to the bitmap, we have to convert bitmap to image because it's possible to apply filters to images
                SKImage image = SKImage.FromBitmap(bitmap);
                SKImageFilter imagefilter = SKImageFilter.CreateColorFilter(SKColorFilter.CreateHighContrast(true, SKHighContrastConfigInvertStyle.NoInvert, contrast));
                SKRectI rectout = new SKRectI();
                SKPoint pointout = new SKPoint();
                image = image.ApplyImageFilter(imagefilter, new SKRectI(0, 0, image.Width, image.Height), new SKRectI(0, 0, image.Width, image.Height), out rectout, out pointout);
                bitmap = SKBitmap.FromImage(image);
            }

            // Update canvas view
            canvasview.InvalidateSurface();
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
            Navigation.PushAsync(new ResultsSinglePage(neuralnet, input, answer));
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
                    MNISTpixelarray[y * 28 + x] = (byte)(255 - (bitmap.GetPixel(x, 27-y).Red + bitmap.GetPixel(x, 27-y).Green + bitmap.GetPixel(x, 27-y).Blue) / 3);
                }
            }
        }
    }
}
