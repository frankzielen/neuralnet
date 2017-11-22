using System;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    public class TrainAndTestPage : ContentPage
    {
        // Page for training or testing the net
        // This is given by runtype which can be "Train" or "Test"
        // mnistdata must hold the appropriate data set
        public TrainAndTestPage(string runtype, NeuralNet neuralnet, MNISTDataManager mnistdata)
        {
            // Define GUI
            Title = string.Format("{0} Neural Net",runtype);
            
            BackgroundColor = Color.SteelBlue;

            // Label for description
            Label label_description1 = new Label
            {
                Text = string.Format("Please define the number of data sets from the MNIST {0} data base to be used for {0}ing. The {0} data base holds about {1:N0} data sets in total.", runtype.ToLower(), mnistdata.CountData),
                TextColor = Color.GhostWhite,
                HorizontalOptions = LayoutOptions.Center
            };

            // Label for number of training / testing sets used
            Label label_useddatasets = new Label
            {
                Text = mnistdata.UsedDataSets.ToString("N0") + " data sets selected",
                TextColor = Color.GhostWhite,
                HorizontalOptions = LayoutOptions.Center
            };

            // Slider for number of training / testing sets used
            Slider slider_useddatasets = new Slider(0,mnistdata.CountData,mnistdata.UsedDataSets);
            slider_useddatasets.ValueChanged += (s, e) =>
            {
                mnistdata.UsedDataSets = (int)e.NewValue;
                label_useddatasets.Text = mnistdata.UsedDataSets.ToString("N0") + " data sets selected";
            };

            // Button to start training / testing
            Button button = new Button();

            button.Text = string.Format("Start {0}ing",runtype);

            button.Clicked += (s, e) =>
            {
                if (runtype == "Train")
                {
                    // Train neural net
                    for (int k = 0; k < mnistdata.Epochs; k++)
                        for (int i = 0; i < mnistdata.UsedDataSets; i++)
                        {
                            neuralnet.Train(mnistdata.Input(i), mnistdata.Output(i));
                        }
                }
                else
                {
                    // Test neural net
                    // Setup scorecard for capturing results
                    Vector<double> scorecard = Vector<double>.Build.Dense(mnistdata.UsedDataSets);

                    // Test neural net
                    for (int i = 0; i < mnistdata.UsedDataSets; i++)
                    {
                        Vector<double> answer = neuralnet.Query(mnistdata.Input(i));
                        if (answer.AbsoluteMaximumIndex() == mnistdata.Number(i))
                            scorecard[i] = 1.0;
                    }

                    // Output of results
                    DisplayAlert("Result", string.Format("{0} out of {1} data sets have been identified correctly.\nThis is a {2:P3} performance.", scorecard.Sum(), scorecard.Count, scorecard.Sum() / scorecard.Count), "OK");
                }
            };

            Content = new StackLayout
            {

                Children =
                {
                    label_description1,
                    slider_useddatasets,
                    label_useddatasets,
                    button
                }
            };

            this.Appearing += (s, e) =>
            {
                // Set view dimensions and locations according to page dimensions
                this.Padding = new Thickness(this.Width / 20, this.Height / 8);

                double width = this.Width * 0.5;
                button.WidthRequest = width;

                double height = this.Height * 0.75 / 4 * 0.75;
                button.HeightRequest = height;
            };
        }
    }
}

