using System;
using Xamarin.Forms;

namespace NeuralNetwork
{
    // NeuralNetRunType is for selecting if net is trained or tested
    public enum NeuralNetRunType { train, test };

    public class MenuPage : ContentPage
    {
        // Setup neural net
        // Input vector must hold a 28x28 pixel bitmap
        // Output vector mus capture a result 0 to 9
        public NeuralNet neuralnet = new NeuralNet(28 * 28, 100, 10, 0.3);

        // Setup MNIST data manager for train and test data
        public MNISTDataManager mnisttraindata = new MNISTDataManager();
        public MNISTDataManager mnisttestdata = new MNISTDataManager();

        public MenuPage()
        {
            // Define GUI
            Title = "Neural Net for Handwritten Digits";
            BackgroundColor = Color.SteelBlue;

            Label description = new Label
            {
                Text = StatusTextNeuralNet(),
            };

            Button buttonhelp = new Button { Text = "Introduction" };
            Button buttonresetnet = new Button { Text = "Reset Neural Net" };
            Button buttontrainnet = new Button { Text = "Train Neural Net" };
            Button buttontestnet = new Button { Text = "Test Neural Net" };

            Content = new StackLayout
            {
                Children =
                {
                    description,
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
                this.Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);

                description.Margin = new Thickness(0, 0, 0, Application.Current.MainPage.Height * 0.05);

                // Set button width acc. to page width
                double width = Application.Current.MainPage.Width * 0.5;
                buttonhelp.WidthRequest = width;
                buttonresetnet.WidthRequest = width;
                buttontrainnet.WidthRequest = width;
                buttontestnet.WidthRequest = width;

                // Set button height acc to page height
                double height = Application.Current.MainPage.Height * 0.10;
                buttonhelp.HeightRequest = height;
                buttonresetnet.HeightRequest = height;
                buttontrainnet.HeightRequest = height;
                buttontestnet.HeightRequest = height;

                // Renew text
                description.Text = StatusTextNeuralNet();
            };

            // Introduction
            buttonhelp.Clicked += (s, e) =>
            {
                Navigation.PushAsync(new IntroductionPage());
            };

            // Reset neural net
            buttonresetnet.Clicked += (s, e) =>
            {
                neuralnet.Reset();

                // Update text
                description.Text = StatusTextNeuralNet();
            };

            // Train neural net with MNIST data (which is embedded in the code)
            buttontrainnet.Clicked += async (s, e) =>
            {
                // Read train data (if not already read)
                if (mnisttraindata.CountData==0)
                {
                    await DisplayAlert("Information", "The app needs to load MNIST trainig data to memory first. Please wait.","OK");
                    mnisttraindata.ReadEmbeddedText(@"NeuralNetwork.MNISTDatasets.mnist_train.csv");                   
                }

                await Navigation.PushAsync(new TrainAndTestPage(NeuralNetRunType.train, neuralnet, mnisttraindata));
            };

            // Test neural net with MNIST data (which is embedded in the code)
            buttontestnet.Clicked += async (s, e) =>
            {
                if (neuralnet.TrainingDataCounter == 0)
                    await DisplayAlert("Information", "You have not trained the net so far. The performance will be very poor.", "OK");

                string[] menuitems = { "MNIST data base", "My own handwriting" };
                var answer = await DisplayActionSheet("Select source of test data","Cancel",null, menuitems);

                // Use MNIST data base
                if (answer == menuitems[0])
                {
                    // Read test data (if not already read)
                    if (mnisttestdata.CountData == 0)
                    {
                        await DisplayAlert("Information", "The app needs to load MNIST test data sets to memory first. Please wait.", "OK");
                        mnisttestdata.ReadEmbeddedText(@"NeuralNetwork.MNISTDatasets.mnist_test.csv");
                    }

                    await Navigation.PushAsync(new TrainAndTestPage(NeuralNetRunType.test, neuralnet, mnisttestdata));
                }

                // Use Camera and own digits
                if (answer == menuitems[1])
                {
                    await Navigation.PushAsync(new CameraTestPage(neuralnet));
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

