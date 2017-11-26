using System;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    // Select if net is trained or tested
    public enum NeuralNetRunType { train, test };

    public class MenuPage : ContentPage
    {
        // Setup neural net
        public NeuralNet neuralnet = new NeuralNet(28 * 28, 100, 10, 0.3);

        // Setup MNIST data manager for train and test data
        public MNISTDataManager mnisttraindata = new MNISTDataManager();
        public MNISTDataManager mnisttestdata = new MNISTDataManager();

        public MenuPage()
        {
            // Define GUI
            Title = "Neural Network";
            BackgroundColor = Color.SteelBlue;

            Label description1 = new Label
            {
                Text = StatusTextNeuralNet(),
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            Button buttonhelp = new Button { Text = "Introduction" };
            Button buttonresetnet = new Button { Text = "Reset Neural Net" };
            Button buttontrainnet = new Button { Text = "Train Neural Net" };
            Button buttontestnet = new Button { Text = "Test Neural Net" };

            Content = new StackLayout
            {
                Children =
                {
                    description1,
                    buttonhelp,
                    buttonresetnet,
                    buttontrainnet,
                    buttontestnet
                }
            };

            // Adjust/update layout when page appers
            this.Appearing += (s, e) =>
            {
                // Set view dimensions and locations according to page dimensions
                this.Padding = new Thickness(this.Width * 0.05, this.Height * 0.05);

                // Set button width to half page width
                double width = this.Width * 0.5;
                buttonhelp.WidthRequest = width;
                buttonresetnet.WidthRequest = width;
                buttontrainnet.WidthRequest = width;
                buttontestnet.WidthRequest = width;

                // Set button height to 15% of page height
                double height = this.Height * 0.15;
                buttonhelp.HeightRequest = height;
                buttonresetnet.HeightRequest = height;
                buttontrainnet.HeightRequest = height;
                buttontestnet.HeightRequest = height;

                // Renew text
                description1.Text = StatusTextNeuralNet();
            };

            // Reset neural net
            buttonresetnet.Clicked += (s, e) =>
            {
                neuralnet.Reset();

                // Update text
                description1.Text = StatusTextNeuralNet();
            };

            // Train neural net with MNIST data
            buttontrainnet.Clicked += async (s, e) =>
            {
                // Read train data (if not already read)
                if (mnisttraindata.CountData==0)
                {
                    await DisplayAlert("Information", "The app needs to load training data sets to memory first. Please wait.","OK");
                    mnisttraindata.ReadEmbeddedText(@"NeuralNetwork.MNISTDatasets.mnist_train.csv");                   
                }

                await Navigation.PushAsync(new TrainAndTestPage(NeuralNetRunType.train, neuralnet, mnisttraindata));
            };

            // Test neural net with MNIST data
            buttontestnet.Clicked += async (s, e) =>
            {
                string[] menuitems = { "Digits from MNIST data base", "Take pictures of digits using phone cam" };
                var answer = await DisplayActionSheet("Select test data source",null,null, menuitems);

                // Use MNIST data base
                if (answer == menuitems[0])
                {
                    // Read test data (if not already read)
                    if (mnisttestdata.CountData == 0)
                    {
                        await DisplayAlert("Information", "The app needs to load testing data sets to memory first. Please wait.", "OK");
                        mnisttestdata.ReadEmbeddedText(@"NeuralNetwork.MNISTDatasets.mnist_test.csv");
                    }

                    await Navigation.PushAsync(new TrainAndTestPage(NeuralNetRunType.test, neuralnet, mnisttestdata));
                }

                // Use Camera and own digits
                if (answer == menuitems[1])
                {
                    await Navigation.PushAsync(new CameraTestPage());
                }
            };
        }

        // Get status text for neural net
        string StatusTextNeuralNet()
        {
            return String.Format("Neural net trained with {0:N0} data sets\nBest performance in testing:\n{1:P3}", neuralnet.TrainingDataCounter, neuralnet.BestPerformance);
        }
    }
}

