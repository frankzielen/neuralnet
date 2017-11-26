using System;
using Xamarin.Forms;
using Plugin.Media;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace NeuralNetwork
{
    public class CameraTestPage : ContentPage
    {
        SKCanvasView canvasview;
        SKBitmap bitmap;

        public CameraTestPage()
        {
            // Define GUI
            Title = "Test Neural Net";

            BackgroundColor = Color.SteelBlue;

            Button button = new Button { Text = "Take a Picture" };

            Image image = new Image();
            image.HorizontalOptions = LayoutOptions.FillAndExpand;
            image.VerticalOptions = LayoutOptions.FillAndExpand;

            canvasview = new SKCanvasView();
            canvasview.HorizontalOptions = LayoutOptions.FillAndExpand;
            canvasview.VerticalOptions = LayoutOptions.FillAndExpand;
            canvasview.PaintSurface += OnCanvasViewPaintSurface;

            Content = new StackLayout
            {
                Children = {button, canvasview}
            };

            button.Clicked += async (sender, args) =>
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
                    SaveToAlbum = true,
                    // Crop pic to square format
                    AllowCropping = true,
                    // Convert pic to 28x28 pic
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 28,
                });

                if (photo != null)
                {
                    // Get image source
                    //image.Source = ImageSource.FromStream(photo.GetStream);

                    // Get bitmap
                    using (SKManagedStream skstream = new SKManagedStream(photo.GetStream()))
                    {
                        bitmap = SKBitmap.Decode(skstream);
                    }
                }
            };

            this.Appearing += (s, e) =>
            {
                // Set view dimensions and locations according to page dimensions
                this.Padding = new Thickness(this.Width * 0.05, this.Height * 0.05);

                // Set button width to half page width
                double width = this.Width * 0.5;
                  button.WidthRequest = width;

                // Set button height to 15% of page height
                double height = this.Height * 0.15;
                  button.HeightRequest = height;
            };
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (bitmap != null)
            {
                float x = (info.Width - bitmap.Width) / 2;
                float y = (info.Height / 3 - bitmap.Height) / 2;
                //canvas.DrawBitmap(bitmap,new SKRect(0, 0, info.Width, info.Height));
                canvas.DrawBitmap(bitmap,0,0);
            }
            else
            {
                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = SKColors.Blue;
                    paint.TextAlign = SKTextAlign.Center;
                    paint.TextSize = 48;

                    canvas.DrawText("No pic shot",
                        info.Width / 2, 5 * info.Height / 6, paint);
                }
            }
        }

    }
}
