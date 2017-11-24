using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;

namespace NeuralNetwork
{
    public class NeuralNet
    {
        // Number of nodes in input, hidden and output layer
        int inodes;
        int hnodes;
        int onodes;

        // Link weight matrices
        // Weights inside the arrays are w_i_j, where link is from node i to node j in the next layer
        // w11 w21
        // w12 w22 etc
        Matrix<double> wih;
        Matrix<double> who;

        // Learning rate
        double lr;

        // Counter for training data sets applied to neural net
        public int TrainingDataCounter { get; set; }

        // Remember performence of neural net
        // Performance is given as percentage of correctly identified data sets in a test session
        // Performance is captured for each test session
        public ObservableCollection<double> Performance { get; set; }

        // Initialize neural net
        public NeuralNet(int inputnodes=3, int hiddennodes=3, int outputnodes=3, double learningrate=0.3)
        {
            // Set nodes
            inodes = inputnodes;
            hnodes = hiddennodes;
            onodes = outputnodes;

            // Set weight matrices randomly by normal distribution given by mean 0 and input node depending std. derivation  
            var distribution = Normal.WithMeanStdDev(0, Math.Pow(inodes, -0.5));
            wih = Matrix<double>.Build.Dense(hnodes, inodes, (i, j) => distribution.Sample());

            distribution = Normal.WithMeanStdDev(0, Math.Pow(hnodes, -0.5));
            who = Matrix<double>.Build.Dense(onodes, hnodes, (i, j) => distribution.Sample());

            // Set learning rate
            lr = learningrate;

            // Set counter for training data sets
            TrainingDataCounter = 0;

            // Set performance measuring
            Performance = new ObservableCollection<double>();
        }

        // Reset neural net by resetting weight matrices
        public void Reset(double learningrate = 0.3)
        {
            // Reset weight matrices randomly by normal distribution given by mean 0 and input node depending std. derivation  
            var distribution = Normal.WithMeanStdDev(0, Math.Pow(inodes, -0.5));
            wih = Matrix<double>.Build.Dense(hnodes, inodes, (i, j) => distribution.Sample());

            distribution = Normal.WithMeanStdDev(0, Math.Pow(hnodes, -0.5));
            who = Matrix<double>.Build.Dense(onodes, hnodes, (i, j) => distribution.Sample());

            // (Re-)set learning rate
            lr = learningrate;

            // Reset counter for training data sets
            TrainingDataCounter = 0;

            // Reset performance measuring
            while (Performance.Count>0)
                Performance.RemoveAt(0);
        }

        // LearningRate property
        public double LearningRate
        {
            get { return lr; }
            set { lr = value; }
        }

        // Get best performance achieved
        public double BestPerformance
        {
            get
            {
                double best = 0;
                foreach (double result in Performance)
                    best = result > best ? result : best;
                return best;
            }
        }

        // Definition of activationb function
        // We use sigmoid function f(x)=1/(1+e(-x))
        static double ActivationFunction(double x)
        {
            return 1.0 / (1.0 + Math.Pow(Math.E, -x));
        }

        // Query the neural network
        // Trigger input value vector at input nodes and request output at output nodes
        public Vector<double> Query(Vector<double> inputs)
        {
            // Calculate signals into hidden layer (=summarize weighted signals)
            Vector<double> hidden_outputs = wih * inputs;

            // Calculate the signals emerging from hidden layer (=apply activation function)
            hidden_outputs.MapInplace(ActivationFunction, Zeros.Include);

            // Calculate signals into final output layer (=summarize weighted signals)
            Vector<double> final_outputs = who * hidden_outputs;

            // Calculate the signals emerging from final output layer (=apply activation function)
            final_outputs.MapInplace(ActivationFunction, Zeros.Include);

            return final_outputs;
        }

        // Train the neural network
        public void Train(Vector<double> inputs, Vector<double> targets)
        {
            // Calculate signals into hidden layer (=summarize weighted signals)
            Vector<double> hidden_outputs = wih * inputs;

            // Calculate the signals emerging from hidden layer (=apply activation function)
            hidden_outputs.MapInplace(ActivationFunction, Zeros.Include);

            // Calculate signals into final output layer (=summarize weighted signals)
            Vector<double> final_outputs = who * hidden_outputs;

            // Calculate the signals emerging from final output layer (=apply activation function)
            final_outputs.MapInplace(ActivationFunction, Zeros.Include);

            // Output layer error is the (target - actual)
            Vector<double> output_errors = targets - final_outputs;

            // Hidden layer error is the output_errors, split by weights, recombined at hidden nodes
            Vector<double> hidden_errors = who.TransposeThisAndMultiply(output_errors);

            // Update the weights for the links between the hidden and output layers
            who += lr * output_errors.PointwiseMultiply(final_outputs.PointwiseMultiply(final_outputs.SubtractFrom(1.0))).ToColumnMatrix() * hidden_outputs.ToRowMatrix();

            // Update the weights for the links between the input and hidden layers
            wih += lr * hidden_errors.PointwiseMultiply(hidden_outputs.PointwiseMultiply(hidden_outputs.SubtractFrom(1.0))).ToColumnMatrix() * inputs.ToRowMatrix();     
        }
    }
}
