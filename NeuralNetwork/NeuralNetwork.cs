using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    public class App : Application
    {
        public App()
        {
            // Setup neural net
            NeuralNet neuralnet = new NeuralNet(28 * 28, 100, 10, 0.3);

            // Setup MNIST data manager
            MNISTDataManager mnistdata = new MNISTDataManager();

            // Label to show results
            Label label = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = ""
            };

            // Button to train net
            Button buttontrain = new Button();
            buttontrain.Text = "Train net";
            buttontrain.VerticalOptions = LayoutOptions.End;
            buttontrain.Clicked += (s, e) =>
            {
                // Read train data
                mnistdata.ReadEmbeddedText(@"NeuralNetwork.MNISTDatasets.mnist_train.csv");

                // Train neural net
                for (int i = 0; i < mnistdata.CountData; i++)
                {
                    neuralnet.Train(mnistdata.Input(i), mnistdata.Output(i));

                    //if ((i + 1) % 5000 == 0)
                    //{
                    //    bool answer = await MainPage.DisplayAlert("Read Data", String.Format("Net trained with {0} numbers. Continue?", i+1), "Yes", "No");
                    //    if (!answer)
                    //        break;
                    //}
                }
            };

            // Button to test net
            Button buttontest = new Button();
            buttontest.Text = "Test net";
            buttontest.VerticalOptions = LayoutOptions.End;
            buttontest.Clicked += (s, e) =>
            {
                // Read test data
                mnistdata.ReadEmbeddedText(@"NeuralNetwork.MNISTDatasets.mnist_test.csv");

                // Test neural net
                Vector<double> scorecard = Vector<double>.Build.Dense(mnistdata.CountData);
                //String message = "Nr.\tData\tAnswer\tResult\n\n";

                for (int i = 0; i < mnistdata.CountData; i++)
                {
                    Vector<double> answer = neuralnet.Query(mnistdata.Input(i));
                    if (answer.AbsoluteMaximumIndex() == mnistdata.Number(i))
                        scorecard[i] = 1.0;

                    //message += string.Format("{0}\t{1}\t{2}\t{3}\n", i, mnistdata.Number(i), answer.AbsoluteMaximumIndex(), scorecard[i]);
                }

                //message += string.Format("\nPerformance = {0}", scorecard.Sum() / scorecard.Count);

                label.Text = string.Format("Performance = {0}", scorecard.Sum() / scorecard.Count);
            };

            // The root page of your application
            var content = new ContentPage
            {
                Title = "NeuralNetwork",
                Content = new StackLayout
                {
                    Children = { label, buttontrain, buttontest } 
                }
            };

            MainPage = new NavigationPage(content);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
