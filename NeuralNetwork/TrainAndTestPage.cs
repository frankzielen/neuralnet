using System;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    // Page for training or testing the net
    // This is given by enum runtype which can be "train" or "test"
    // mnistdata must hold the appropriate data set
    public class TrainAndTestPage : ContentPage
    {
        // Indicates if training / testing is currently running
        bool activerun = false; 

        public TrainAndTestPage(NeuralNetRunType runtype, NeuralNet neuralnet, MNISTDataManager mnistdata)
        {
            // Define GUI

            // Page properties
            Title = string.Format("{0} Net", runtype.ToString().ToUpperFirstOnly());
            BackgroundColor = Color.SteelBlue;
            Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);

            // Label for description
            Label description = new Label
            {
                Text = string.Format("Please define the number of data sets from the MNIST {0} data base to be used for {0}ing. Each data set represents one digit. The {0} data base holds about {1:N0} data sets in total.", runtype.ToString(), mnistdata.CountData),
                Margin = new Thickness(0, 0, 0, Application.Current.MainPage.Height * 0.075)
            };

            // Label for number of training / testing sets used
            Label label_useddatasets = new Label
            {
                Text = mnistdata.UsedDataSets.ToString("N0") + " data sets selected",
                Margin = new Thickness(0, 0, 0, Application.Current.MainPage.Height * 0.075)
            };

            // Slider for number of data sets used for training / testing 
            Slider slider_useddatasets = new Slider(0, mnistdata.CountData, mnistdata.UsedDataSets);
            slider_useddatasets.ValueChanged += (s, e) =>
            {
                mnistdata.UsedDataSets = (int)e.NewValue;
                label_useddatasets.Text = mnistdata.UsedDataSets.ToString("N0") + " data sets selected";
            };

            // Progress bar
            ProgressBar progressbar = new ProgressBar { Progress = 0 };

            // Label for showing progress
            Label label_progress = new Label
            {
                Text = "Currently no calculation running",
            };

            // Button to start training / testing
            Button button = new Button
            {
                Text = string.Format("Start {0}ing", runtype.ToString().ToUpperFirstOnly()),
                WidthRequest = Application.Current.MainPage.Width * 0.5,
                HeightRequest = Application.Current.MainPage.Height * 0.10
            };

            // Define page
            Content = new StackLayout
            {

                Children =
                {
                    description,
                    label_useddatasets,
                    slider_useddatasets,
                    button,
                    progressbar,
                    label_progress
                }
            };

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

                    // Remember number of training data sets
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
                    await DisplayAlert("Result", string.Format("{0} out of {1} data sets have been identified correctly.\nPerformance: {2:P3}", scorecard.Sum(), scorecard.Count, scorecard.Sum() / scorecard.Count), "OK");

                    // Remeber performance result
                    neuralnet.Performance.Add(scorecard.Sum() / scorecard.Count);
                }

                // Reset progress bar
                label_progress.Text = "Currently no calculation running";
                await progressbar.ProgressTo(0.0, 250, Easing.Linear);

                // Cancel activerun flag
                activerun = false;
            };
        }
    }
}

