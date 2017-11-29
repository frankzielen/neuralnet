using System;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    // Show overall and detailed results of testing with MNIST data
    public class ResultsMNISTPage : ContentPage
    {
        public ResultsMNISTPage(MNISTDataManager mnistdata)
        {
            // Define GUI

            // Page properties
            Title = "Results";
            BackgroundColor = Color.SteelBlue;
            Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);

            // Results in Total
            Label labelresults = new Label { Text = "Results" };

            // Define page
            Content = new StackLayout
            {
                Children = { labelresults },
                Spacing = Application.Current.MainPage.Height * 0.05
            };

            // Change bitmap to MNISTdata
            //Vector<double> pixel = mnistdata.Input(number++).Subtract(0.01) / 0.99 * 255;
            //for (int y = 0; y < bitmap.Height; y++)
            //{
            //    for (int x = 0; x < bitmap.Width; x++)
            //    {
            //        int rgb = 255 - (int)pixel[y * 28 + x];
            //        System.Diagnostics.Debug.WriteLine(rgb);
            //        bitmap.SetPixel(x, y, Color.FromRgb(rgb, rgb, rgb).ToSKColor());
            //    }
            //}
            //canvasview.InvalidateSurface();
        }
    }
}
