using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using CarouselView.FormsPlugin.Abstractions;

namespace NeuralNetwork
{
    // Shows detailed results of testing with MNIST
    // User can swipe through data set to see images of digits and sees answer of neural net
    public class ResultsDetailsPage : ContentPage
    {
        // Class for capturing data of carousel items (here: Imagesources of pictures)
        public class CarouselItem
        {
            public ImageSource Picture { get; set; }
        }

        // Collection of pictures for carousel view (for the digits)
        ObservableCollection<CarouselItem> pictures;

        // Number of images to be loaded (at the beginning and every time user scrolls to end of carousel)
        int buffersize = 20;

        // Labels (including dynamic data)
        Label labelheadline;
        Label labeldigitcorrect;
        Label labeldigitanswernet;

        // Store neural net and mnistdata
        NeuralNet neuralnet;
        MNISTDataManager mnistdata;

        public ResultsDetailsPage(NeuralNet neuralnet, MNISTDataManager mnistdata)
        {
            // Save parameters
            this.neuralnet = neuralnet;
            this.mnistdata = mnistdata;

            // Define GUI

            // Page properties
            Title = "Details";
            BackgroundColor = Color.SteelBlue;
            Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);

            // Step 1: Generate initial collection of pictures from MNIST database (buffersize elements)
            pictures = new ObservableCollection<CarouselItem>();
            AddPictures(0);

            // Step 2: Generate data template (just a Image with data binding to CarouselItem class)
            DataTemplate template = new DataTemplate(() =>
            {
                Image image = new Image();
                image.SetBinding(Image.SourceProperty, "Picture");
                return image;
            });

            // Step 3: Generate carousel view
            var myCarousel = new CarouselViewControl();

            myCarousel.Position = 0; //default
            myCarousel.ItemsSource = pictures;
            myCarousel.ItemTemplate = template;
            myCarousel.InterPageSpacing = 2;
            myCarousel.Orientation = CarouselViewOrientation.Horizontal;
            myCarousel.ShowIndicators = false;
            myCarousel.HorizontalOptions = LayoutOptions.FillAndExpand;
            myCarousel.VerticalOptions = LayoutOptions.FillAndExpand;

            // Define labels
            labelheadline = new Label();
            labeldigitcorrect = new Label();
            labeldigitanswernet = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Application.Current.MainPage.Height * 0.1,
                VerticalTextAlignment = TextAlignment.Center
            };
            SetLabels(0);

            // Step 4: Build page
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Children = { labelheadline, labeldigitcorrect, myCarousel, labeldigitanswernet },
                    Spacing = Application.Current.MainPage.Height * 0.05
                }
            };

            // Update page when image is swiped
            myCarousel.PositionSelected += (s, e) =>
            {
                // Add new pictures if user has reached end of carousel
                // KNOWN ERROR: When end is reached the wrong picture is displayed (first picture of uploaded data)
                // Warning: stepping through all MINST data may yield to memory problems
                if (myCarousel.Position == pictures.Count - 1)
                    AddPictures(myCarousel.Position + 1);

                // Update label
                SetLabels(myCarousel.Position);
            };
        }

        // Add <buffersize> pictures to collection "pictures", starting at mnist data position <datasetstart>
        void AddPictures(int datasetstart)
        {
            // Define the upper index
            int upperindex = Math.Min(datasetstart + buffersize, mnistdata.UsedDataSets);

            // Step through MNIST data, read images add to pictures
            for (int i = datasetstart; i < upperindex; i++)
                pictures.Add(new CarouselItem { Picture = GetImage(i) });
        }

        // Return Imagesource of digit <datasetnumber> in MNIST data base
        ImageSource GetImage(int datasetnumber)
        {
            // Generate an empty bitmap
            SKBitmap bitmap = new SKBitmap(28, 28, SKColorType.Rgba8888, SKAlphaType.Premul);

            // Generate pixel data from neural net input data which is already normalized to [0.01,1]
            Vector<double> pixel = 255 - mnistdata.Input(datasetnumber).Subtract(0.01) / 0.99 * 255;

            // Copy pixel data to bitmap
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int rgb = (int)pixel[y * 28 + x];
                    bitmap.SetPixel(x, y, Color.FromRgb(rgb, rgb, rgb).ToSKColor());
                }
            }

            // Comvert bitmap to data object (to access it as stream)
            SKData data = SKImage.FromBitmap(bitmap).Encode();

            // Read image from stream and return
            return ImageSource.FromStream(data.AsStream);
        }

        // Update content of labels for data set <datasetnumber>
        void SetLabels(int datasetnumber)
        {
            int correct = mnistdata.Number(datasetnumber);
            int answer = neuralnet.Query(mnistdata.Input(datasetnumber)).AbsoluteMaximumIndex();

            labelheadline.Text = string.Format("Data set {0:N0} / {1:N0}", datasetnumber + 1, mnistdata.UsedDataSets);
            labeldigitcorrect.Text = string.Format("Correct digit is {0}", correct);
            labeldigitanswernet.Text = string.Format("Net's answer is {0}", answer);

            if (correct == answer)
                labeldigitanswernet.BackgroundColor = Color.Green;
            else
                labeldigitanswernet.BackgroundColor = Color.Red;
        }
    }
}

