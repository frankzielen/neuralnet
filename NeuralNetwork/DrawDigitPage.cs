using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace NeuralNetwork
{
    public class DrawDigitPage : ContentPage
    {
        // Point of currently drawn path (finger still on screen)
        Dictionary<long, SKPath> temporaryPaths = new Dictionary<long, SKPath>();

        // Points of finalized pahs
        List<SKPath> paths = new List<SKPath>();

        // Flag for resetting canvas (initialized with "true" to be setup with initial lunch of page)
        bool resetcanvas = true;

        // Flag for calculating bitmap
        bool savetobitmap = false;

        public DrawDigitPage()
        {
            // Page properties
            Title = "Draw Digit";
            BackgroundColor = Color.SteelBlue;
            Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);

            // Ask for squared canvas for drawing digit
            var canvasview = new SKCanvasView
            {
                WidthRequest = Application.Current.MainPage.Width * 0.85,
                HeightRequest = Application.Current.MainPage.Width * 0.85,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            canvasview.PaintSurface += OnCanvasViewPaintSurface;

            // Enable and initialize touch event for canvas
            canvasview.Touch += OnTouch;
            canvasview.EnableTouchEvents = true;

            // Define button to clear canvas
            Button buttonclear = new Button
            {
                Text = "Clear",
                WidthRequest = Application.Current.MainPage.Width * 0.3,
                HeightRequest = Application.Current.MainPage.Height * 0.10,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            buttonclear.Clicked += (s, e) =>
            {
                // Reset canvas in OnCanvasViewPaintSurface (called via InvalidateSurface) 
                resetcanvas = true;
                canvasview.InvalidateSurface();
            };

            // Define button OK
            Button buttonok = new Button
            {
                Text = "OK",
                WidthRequest = Application.Current.MainPage.Width * 0.3,
                HeightRequest = Application.Current.MainPage.Height * 0.10,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            buttonok.Clicked += (s, e) =>
            {
                // Calculate bitmap in OnCanvasViewPaintSurface (called via InvalidateSurface) 
                savetobitmap = true;
                canvasview.InvalidateSurface();

                // Remove page from stack
                Navigation.PopAsync();
            };

            // Define button stack
            var buttons = new StackLayout
            {
                Children = { buttonclear, buttonok },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = Application.Current.MainPage.Width * 0.1
            };

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Children = { canvasview, buttons },
                    Spacing = Application.Current.MainPage.Height * 0.05
                }
            };
        }

        // Touch event for canvas to generate paths
        void OnTouch(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                // Start drawing path
                case SKTouchAction.Pressed:
                    var p = new SKPath();
                    p.MoveTo(e.Location);
                    temporaryPaths[e.Id] = p;
                    break;

                // Drawing path
                case SKTouchAction.Moved:
                    if (e.InContact)
                        temporaryPaths[e.Id].LineTo(e.Location);
                    break;

                // End drawing path
                case SKTouchAction.Released:
                    paths.Add(temporaryPaths[e.Id]);
                    temporaryPaths.Remove(e.Id);
                    break;

                // Cancel drawing path
                case SKTouchAction.Cancelled:
                    temporaryPaths.Remove(e.Id);
                    break;
            }

            // Important: track also further events belonging to this event (e.g. "Moved"-actions after "Pressed")
            e.Handled = true;

            // Update canvas
            ((SKCanvasView)sender).InvalidateSurface();
        }

        // Draw on canvas
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            // Initialize canvas if resetcanvas is true
            if (resetcanvas)
            {
                // Delete paths
                while (paths.Count > 0)
                    paths.RemoveAt(0);

                // Clear canvas in white
                canvas.Clear(SKColors.White);

                // Reset flag for erasing canvas
                resetcanvas = false;
            }
            // Draw all paths (if resetcanvas is not set)
            else
            {
                // Define paint brush
                var touchPathStroke = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Black,
                    StrokeWidth = 0.05f * Math.Min(info.Width, info.Height),
                    StrokeCap = SKStrokeCap.Round
                };

                // Draw paths (currently "in drawing" and finalized)
                foreach (var touchPath in temporaryPaths)
                {
                    canvas.DrawPath(touchPath.Value, touchPathStroke);
                }
                foreach (var touchPath in paths)
                {
                    canvas.DrawPath(touchPath, touchPathStroke);
                }
            }

            // Save canvas to bitmap in page "CameraTestPage"
            // This coding is not elegant / robust but it works
            if (savetobitmap)
            {
                Application app = Application.Current;
                CameraTestPage page = app.MainPage.Navigation.NavigationStack[1] as CameraTestPage;

                var image = surface.Snapshot();
                page.bitmap = SKBitmap.FromImage(image);

                savetobitmap = false;
            }
        }
    }
}

