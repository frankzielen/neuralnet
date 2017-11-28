using System;
using Xamarin.Forms;

namespace NeuralNetwork
{
    // This page holds the introduction. It's plane text only.
    public class IntroductionPage : ContentPage
    {
        public IntroductionPage()
        {
            // Page properties
            Title = "Introduction";
            BackgroundColor = Color.SteelBlue;
            Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);

            // Define label
            Label description = new Label
            {
                Text = "This app wants to inspire you for neural networks!\n\n"+
                    "It provides a very simple but powerful tool for recognizing handwritten digits. It is based on the example given by Tariq Rashid in his book "+
                    "\"Neuronale Netze selbst programmieren\" from O'REILLY (title of the english original: Make Your Own Neural Network).\n\n"+
                    "Core is a 3-layer neural net wherby the first layer is fed with the pixel data of a grayscaled bitmap showing a digit. The resolution is 28 x 28 pixels so there are 784 knots capturing the input data. The hidden layer is set to 100 knots and the output layer has 10 knots representing the possible results (figure is zero, one, ..., nine).\n\n"+
                    "How to use this app? It comes with the MNIST data base including pixel data of 70.000 handwritten digits. We use 60.000 data sets for training and 10.000 data sets for testing purpose. So you just have to do the following two steps:\n\n"+
                    "Step 1: Train the neural net\n\n"+
                    "Just touch the according menu item and select the number of data sets you want to use for training. The more data you use, the better your net will perform.\n\n"+
                    "Step 2: Test the neural net\n\n"+
                    "Select the test menu. You may either do a mass testing with the MNIST test data or you prove the net with your own handwritten figures! Just take a picture of a digit and the net will hopefully give the right answer. It's also possible to use your photographed figures for training so you can teach the net your handwriting!\n\n"+
                    "I have done this app for fun because Tariq's book inspired me to deal with this very interesting topic. Furthermore, an app makes it very easy to use own handwritten numbers for testing. The code is cross plattform and on GitHub if someone is interested.\n\n" +
                    "(c) 2017 Frank Zielen"
            };

            Content = new ScrollView
            {
                Content = description
            };
        }
    }
}
