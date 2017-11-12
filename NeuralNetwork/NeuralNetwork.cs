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
            NeuralNet neuralnet = new NeuralNet(28*28,100,10,0.3);

            // Setup MNIST data manager
            MNISTDataManager mnistdata = new MNISTDataManager();

            // Read train data
            mnistdata.ReadEmbeddedText(@"NeuralNetwork.MNISTDatasets.mnist_train_100.csv");

            // Train neural net
            for (int i = 0; i < mnistdata.CountData; i++)
                neuralnet.Train(mnistdata.Input(i), mnistdata.Output(i));
            
            // Read test data
            mnistdata.ReadEmbeddedText(@"NeuralNetwork.MNISTDatasets.mnist_test_10.csv");

            // Test neural net
            Vector<double> scorecard = Vector<double>.Build.Dense(mnistdata.CountData);
            string message = "Nr.\tData\tAnswer\tResult\n\n";

            for (int i = 0; i < mnistdata.CountData; i++)
            {
                Vector<double> answer = neuralnet.Query(mnistdata.Input(i));
                if (answer.AbsoluteMaximumIndex() == mnistdata.Number(i))
                    scorecard[i] = 1.0;

                message += string.Format("{0}\t{1}\t{2}\t{3}\n", i, mnistdata.Number(i), answer.AbsoluteMaximumIndex(), scorecard[i]);
            }

            message += string.Format("\nPerformance = {0}", scorecard.Sum() / scorecard.Count);

            // The root page of your application
            var content = new ContentPage
            {
                Title = "NeuralNetwork",
                Content = new ScrollView
                {
                    Content = new StackLayout
                    {
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label
                            {
                                HorizontalTextAlignment = TextAlignment.Center,
                                Text = message
                            }
                        }
                    }
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
