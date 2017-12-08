using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    // Page for training or testing the net
    // This is given by enum runtype which can be "train" or "test"
    // mnistdata must hold the appropriate data set
    public class TrainAndTestPage : ContentPage
    {
        // Flag if training / testing is currently running
        static bool activerun = false;

        // Flag if user wants to end calculation
        static bool endrun = false;

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
                Text = String.Format("{0:N0} data set{1} selected", mnistdata.UsedDataSets, mnistdata.UsedDataSets == 0 ? "" : "s"),
                Margin = new Thickness(0, 0, 0, Application.Current.MainPage.Height * 0.075)
            };

            // Slider for number of data sets used for training / testing 
            Slider slider_useddatasets = new Slider(1, mnistdata.CountData, mnistdata.UsedDataSets);
            slider_useddatasets.ValueChanged += (s, e) =>
            {
                mnistdata.UsedDataSets = (int)e.NewValue;
                label_useddatasets.Text = String.Format("{0:N0} data set{1} selected", mnistdata.UsedDataSets, mnistdata.UsedDataSets == 0 ? "" : "s");
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
            Content = new ScrollView
            {
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
                }
            };

            // Button event
            // Here training and test sessions are executed
            button.Clicked += async (s, e) =>
            {
                // Check if another calculation is currently running
                if (activerun == true)
                {
                    var answer = await DisplayAlert("Warning", "Do you want to stop current calculation?", "Yes", "No");
                    if (answer)
                        endrun = true;
                    return;
                }

                // Check if selected data set is big 
                if (mnistdata.UsedDataSets > mnistdata.UsedDataSetsWarning)
                {
                    bool answer = await DisplayAlert("Warning", "Please be aware that big data sets effect run time. Continue?", "Yes", "No");
                    if (!answer)
                        return;
                }

                // Set activerun and endrun flags
                activerun = true;
                endrun = false;

                // Disable Back button
                NavigationPage.SetHasBackButton(this, false);

                // Rename Button to Stop
                button.Text = string.Format("Stop {0}ing", runtype.ToString().ToUpperFirstOnly());

                // Define parameters for progress bar calculation (increase bar 100 times or each 20 runs)
                int progresssteps = Math.Max(100, mnistdata.UsedDataSets/20);
                int progressspan = (int)Math.Ceiling((double)mnistdata.UsedDataSets / progresssteps);

                // Counter for training / testing runs
                int i = 0;

                // Initialize progress bar
                label_progress.Text = i.ToString() + " / " + mnistdata.UsedDataSets.ToString();
                await progressbar.ProgressTo((double)i / mnistdata.UsedDataSets, 1, Easing.Linear);

                // Train neural net
                if (runtype == NeuralNetRunType.train)
                {
                    while (i < mnistdata.UsedDataSets && endrun == false)
                    {
                        // Counter for runs in progress step
                        int j = 0;

                        // Training loop for one progress step done as task
                        await Task.Run(() =>
                        {
                            while (j < progressspan && i < mnistdata.UsedDataSets)
                            {
                                // Train net with data set i
                                neuralnet.Train(mnistdata.Input(i), mnistdata.Output(i));

                                j++;
                                i++;
                            }

                            // Remember number of training data sets
                            neuralnet.TrainingDataCounter += j;
                        });

                        // Update progress bar
                        label_progress.Text = i.ToString() + " / " + mnistdata.UsedDataSets.ToString();
                        await progressbar.ProgressTo((double)i / mnistdata.UsedDataSets, 1, Easing.Linear);
                    }

                    // Show mesage
                    await DisplayAlert("Result", string.Format("Neural net trained with {0:N0} data sets", i), "OK");
                }

                if (runtype == NeuralNetRunType.test)
                {
                    // Setup scorecard for capturing test results
                    // digittotal counts all trained figures, digitcorrect the ones that are identified correctly
                    Vector<double> digittotal = Vector<double>.Build.Dense(10);
                    Vector<double> digitcorrect = Vector<double>.Build.Dense(10);

                    // Testing loop
                    while (i < mnistdata.UsedDataSets && endrun == false)
                    {
                        // Counter for runs in progress step
                        int j = 0;

                        // Testing loop for one progress step done as task
                        await Task.Run(() =>
                        {
                            while (j < progressspan && i < mnistdata.UsedDataSets)
                            {
                                // Increase counter for testet digit (0..9)
                                int number = mnistdata.Number(i);
                                digittotal[number]++;

                                // Ask net
                                Vector<double> answer = neuralnet.Query(mnistdata.Input(i));

                                // Check if it's correct
                                if (answer.AbsoluteMaximumIndex() == number)
                                    digitcorrect[number]++;

                                j++;
                                i++;
                            }

                            // Remember number of training data sets
                            neuralnet.TrainingDataCounter += j;
                        });

                        // Update progress bar
                        label_progress.Text = i.ToString() + " / " + mnistdata.UsedDataSets.ToString();
                        await progressbar.ProgressTo((double)i / mnistdata.UsedDataSets, 1, Easing.Linear);
                    }

                    // Remeber performance result
                    neuralnet.Performance.Add(digitcorrect.Sum() / digittotal.Sum());

                    // Show test results
                    await Navigation.PushAsync(new ResultsMNISTPage(neuralnet, mnistdata, digittotal, digitcorrect));
                }

                // Reset progress bar
                label_progress.Text = "Currently no calculation running";
                await progressbar.ProgressTo(0.0, 250, Easing.Linear);

                // Rename Button to Start
                button.Text = string.Format("Start {0}ing", runtype.ToString().ToUpperFirstOnly());

                // Enable Back button
                NavigationPage.SetHasBackButton(this, true);

                // Cancel activerun flag
                activerun = false;
            };
        }

        // Prevent to go back via hardware back button if there is currently a task running
        protected override bool OnBackButtonPressed()
        {
            if (activerun)
            {
                DisplayAlert("Warning","Please wait until end of current calculation or stop calculation.","OK");
                return true;
            }
            else
                return base.OnBackButtonPressed();
        }
    }
}

