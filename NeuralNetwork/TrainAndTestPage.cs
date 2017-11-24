using System;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    public class TrainAndTestPage : ContentPage
    {
        // Page for training or testing the net
        // This is given by enum runtype which can be "train" or "test"
        // mnistdata must hold the appropriate data set

        // Indicates if training / testing is currently running
        bool activerun = false; 

        public TrainAndTestPage(NeuralNetRunType runtype, NeuralNet neuralnet, MNISTDataManager mnistdata)
        {
            // Define GUI
            Title = string.Format("{0} Neural Net", runtype.ToString().ToUpperFirstOnly());

            BackgroundColor = Color.SteelBlue;

            // Label for description
            Label label_description1 = new Label
            {
                Text = string.Format("Please define the number of data sets from the MNIST {0} data base to be used for {0}ing. The {0} data base holds about {1:N0} data sets in total.", runtype.ToString(), mnistdata.CountData),
            };

            // Label for number of training / testing sets used
            Label label_useddatasets = new Label
            {
                Text = mnistdata.UsedDataSets.ToString("N0") + " data sets selected",
            };

            // Slider for number of training / testing sets used
            Slider slider_useddatasets = new Slider(0, mnistdata.CountData, mnistdata.UsedDataSets);
            slider_useddatasets.ValueChanged += (s, e) =>
            {
                mnistdata.UsedDataSets = (int)e.NewValue;
                label_useddatasets.Text = mnistdata.UsedDataSets.ToString("N0") + " data sets selected";
            };

            // Progress bar
            ProgressBar progressbar = new ProgressBar
            {
                Progress = 0
            };

            // Label for showing progress
            Label label_progress = new Label
            {
                Text = "Currently no calculation running",
            };

            // Button to start training / testing
            Button button = new Button();
            button.Text = string.Format("Start {0}ing",runtype.ToString().ToUpperFirstOnly());

            // Button event
            // Here training and test sessions are executed
            button.Clicked += async (s, e) =>
            {
                // Check if another calculation is currently running
                if (activerun == true)
                {
                    await DisplayAlert("Warning", "Please wait until end of current calculation.", "OK");
                    return;
                }

                // Check if selected data set is big 
                if (mnistdata.UsedDataSets > mnistdata.UsedDataSetsWarning)
                {
                    bool answer = await DisplayAlert("Warning", "Please be aware that big data sets effect run time. Continue?", "Yes", "No");
                    if (!answer)
                        return;
                }

                // Set activerun flag
                activerun = true;

                // Define parameters for progress bar calculation
                int progresssteps = 20;
                int progressspan = (int)Math.Ceiling((double)mnistdata.UsedDataSets / progresssteps);

                // Train neural net
                if (runtype == NeuralNetRunType.train)
                {
                    // Training loop
                    for (int i = 0; i < mnistdata.UsedDataSets; i++)
                    {
                        neuralnet.Train(mnistdata.Input(i), mnistdata.Output(i));

                        // Update progress bar
                        if (i % progressspan == 0)
                        {
                            label_progress.Text = i.ToString() + " / " + mnistdata.UsedDataSets.ToString();
                            await progressbar.ProgressTo((double)i / mnistdata.UsedDataSets, 1, Easing.Linear);
                        }
                    }
                    label_progress.Text = mnistdata.UsedDataSets.ToString() + " / " + mnistdata.UsedDataSets.ToString();
                    await progressbar.ProgressTo(1.0, 1, Easing.Linear);

                    // Show mesage
                    await DisplayAlert("Result", string.Format("Neural net trained with {0:N0} data sets",mnistdata.UsedDataSets), "OK");

                    // Remeber number of training data sets
                    neuralnet.TrainingDataCounter += mnistdata.UsedDataSets;
                }

                if (runtype == NeuralNetRunType.test)
                {
                    // Setup scorecard for capturing test results
                    Vector<double> scorecard = Vector<double>.Build.Dense(mnistdata.UsedDataSets);

                    // Testing loop
                    for (int i = 0; i < mnistdata.UsedDataSets; i++)
                    {
                        Vector<double> answer = neuralnet.Query(mnistdata.Input(i));
                        if (answer.AbsoluteMaximumIndex() == mnistdata.Number(i))
                            scorecard[i] = 1.0;

                        // Update progress bar
                        if (i % progressspan == 0)
                        {
                            label_progress.Text = i.ToString() + " / " + mnistdata.UsedDataSets.ToString();
                            await progressbar.ProgressTo((double)i / mnistdata.UsedDataSets, 1, Easing.Linear);
                        }
                    }
                    label_progress.Text = mnistdata.UsedDataSets.ToString() + " / " + mnistdata.UsedDataSets.ToString();
                    await progressbar.ProgressTo(1.0, 1, Easing.Linear);

                    // Show of test results
                    await DisplayAlert("Result", string.Format("{0} out of {1} data sets have been identified correctly.\nThis is a {2:P3} performance.", scorecard.Sum(), scorecard.Count, scorecard.Sum() / scorecard.Count), "OK");

                    // Remeber performance result
                    neuralnet.Performance.Add(scorecard.Sum() / scorecard.Count);
                }

                // Reset progress bar
                label_progress.Text = "Currently no calculation running";
                await progressbar.ProgressTo(0.0, 250, Easing.Linear);

                // Cancel activerun flag
                activerun = false;
            };

            // Define page
            Content = new StackLayout
            {

                Children =
                {
                    label_description1,
                    slider_useddatasets,
                    label_useddatasets,
                    button,
                    progressbar,
                    label_progress
                }
            };

            // Adjust/update layout when page appers
            this.Appearing += (s, e) =>
            {
                // Set view dimensions and locations according to page dimensions
                this.Padding = new Thickness(this.Width * 0.05, this.Height * 0.1);

                // Set button width to half page width
                double width = this.Width * 0.5;
                button.WidthRequest = width;

                // Set button height
                double height = this.Height * 0.8 * 0.75 / 4;
                button.HeightRequest = height;
            };
        }
    }
}

