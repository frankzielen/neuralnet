using System;
using Xamarin.Forms;

namespace NeuralNetwork
{
    // NeuralNetRunType is for selecting if net is trained or tested
    public enum NeuralNetRunType { train, test };

    // The menue page as starting point of app
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
            Button buttonresetnet = new Button { Text = "Reset Net" };
            Button buttontrainnet = new Button { Text = "Train Net with MNIST Data" };
            Button buttontestnet = new Button { Text = "Test Net with MNIST Data" };
            Button buttonhandnet = new Button { Text = "Try Net with Handwriting" };

            Content = new StackLayout
            {
                Children =
                {
                    description,
                    buttonhelp,
                    buttontrainnet,
                    buttontestnet,
                    buttonhandnet,
                    buttonresetnet
                }
            };

            // Adjust layout when page width and height are set
            this.SizeChanged += (s, e) =>
            {
                // Set view dimensions and locations according to page dimensions
                this.Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);

                description.Margin = new Thickness(0, 0, 0, Application.Current.MainPage.Height * 0.05);

                // Set button width acc. to page width
                double width = Application.Current.MainPage.Width * 0.8;
                buttonhelp.WidthRequest = width;
                buttonresetnet.WidthRequest = width;
                buttontrainnet.WidthRequest = width;
                buttontestnet.WidthRequest = width;
                buttonhandnet.WidthRequest = width;

                // Set button height acc to page height
                double height = Application.Current.MainPage.Height * 0.10;
                buttonhelp.HeightRequest = height;
                buttonresetnet.HeightRequest = height;
                buttontrainnet.HeightRequest = height;
                buttontestnet.HeightRequest = height;
                buttonhandnet.HeightRequest = height;
            };

            // Renew status text when page reappers (after training / testing text may change) 
            this.Appearing +=(s,e)=>
            {
                description.Text = StatusTextNeuralNet();
            };

            // Show introduction
            buttonhelp.Clicked += (s, e) =>
            {
                Navigation.PushAsync(new IntroductionPage());
            };

            // Reset neural net
            buttonresetnet.Clicked += (s, e) =>
            {
                neuralnet.Reset();

                // Update status text
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
                    await DisplayAlert("Information", "You have not trained the net so far. The initial performance will be very poor.", "OK");
                
                // Read test data (if not already read)
                if (mnisttestdata.CountData == 0)
                {
                    await DisplayAlert("Information", "The app needs to load MNIST test data sets to memory first. Please wait.", "OK");
                    mnisttestdata.ReadEmbeddedText(@"NeuralNetwork.MNISTDatasets.mnist_test.csv");
                }

                await Navigation.PushAsync(new TrainAndTestPage(NeuralNetRunType.test, neuralnet, mnisttestdata));
            };

            // Test and train net with own handwriting
            buttonhandnet.Clicked += async (s, e) =>
            {
                if (neuralnet.TrainingDataCounter == 0)
                    await DisplayAlert("Information", "You have not trained the net so far. The initial performance will be very poor.", "OK");
                
                    await Navigation.PushAsync(new CameraTestPage(neuralnet));
            };
        }

        // Get status text for neural net
        string StatusTextNeuralNet()
        {
            return String.Format("Neural net trained with {0:N0} data set{2}\nBest performance in testing:\n{1:P3}", neuralnet.TrainingDataCounter, neuralnet.BestPerformance, (neuralnet.TrainingDataCounter == 1 ? "" : "s"));
        }
    }
}

