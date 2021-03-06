﻿using System;
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

            // Define label
            Label description = new Label
            {
                Text = "This app wants to inspire you for neural networks!\n\n"+
                    "It provides a very simple but powerful tool for recognizing handwritten digits. It is based on the example given by Tariq Rashid in his book "+
                    "\"Make Your Own Neural Network\" (german title: Neuronale Netze selbst programmieren).\n\n"+
                    "Core is a 3-layer neural net wherby the first layer is fed with the pixel data of a grayscaled bitmap showing a digit. The resolution is 28 x 28 pixels so there are 784 knots capturing the input data. The hidden layer is set to 100 knots and the output layer has 10 knots representing the possible results (figure is zero, one, ..., nine).\n\n"+
                    "How to use this app? It comes with the MNIST data base including pixel data of handwritten digits for training and testing purpose. Furthemore, it's possible to use your own handwritten figures. You just have to do the following:\n\n"+
                    "Step 1: Teach the neural net\n\n"+
                    "Touch train menu and select the number of MNIST data sets you want to use for training. The more digits you teach, the better your net will perform. It's also possible to teach the net your own handwriting (see below).\n\n"+
                    "Step 2: Test the neural net\n\n"+
                    "You may either do a mass testing with the MNIST test data or you prove the net with your own handwritten figures. Just take a picture of a digit or write it directly on the screen and the net will hopefully give the right answer. If not, teach the net the correct figure.\n\n"+
                    "It's very interesting to see, how the net learns. For example, you can start writing two digits only and teaching them alternately.\n\n" +
                    "I have done this app for fun because Tariq's book inspired me to deal with this very interesting topic. Furthermore, an app makes it very easy to use own handwritten numbers for testing and teaching. The code is cross platform and on GitHub.\n\n" +
                    "(c) 2017 Frank Zielen",
                Margin = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05)
            };

            Content = new ScrollView
            {
                Content = description
            };
        }
    }
}
