using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    public class MNISTDataManager
    {
        // Data given as string collection
        ObservableCollection<string> dataset;

        // Number of data sets used (for training / testing)
        int useddatasets;

        // Initial default value for useddatasets
        public int UsedDataSetsDefault { get { return 5000; } }

        // Epoches used for training
        int epochs;

        public MNISTDataManager()
        {
            dataset = new ObservableCollection<string>();
            useddatasets = 0;
            epochs = 0;
        }

        // Read embedded data given as text file
        // URL format is "namespace.filename"
        // Set trainingsets to all read data and epochs to 1
        public void ReadEmbeddedText(string URL)
        {
            var assembly = typeof(MNISTDataManager).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(URL);

            using (var reader = new StreamReader(stream))
            {
                // Clear dataset before adding new values
                ClearData();

                while (!reader.EndOfStream)
                    dataset.Add(reader.ReadLine());
            }

            useddatasets = dataset.Count > UsedDataSetsDefault ? UsedDataSetsDefault : dataset.Count;
            epochs = 1;
        }

        // Clear dataset
        void ClearData()
        {
            while (dataset.Count > 0)
                dataset.RemoveAt(0);
            useddatasets = 0;
            epochs = 0;
        }

        // Count data (readonly property)
        public int CountData
        {
            get
            {
                return dataset.Count;
            }
        }

        // UsedDataSets (number of data sets used for training)
        public int UsedDataSets
        {
            get
            {
                return useddatasets;
            }
            set
            {
                if (value > dataset.Count)
                    useddatasets = dataset.Count;
                if (value < 0)
                    useddatasets = 0;
                if (value >= 0 && value <= dataset.Count)
                    useddatasets = value;
            }
        }

        // Epochs (number of training loops)
        public int Epochs
        {
            get
            {
                return epochs;
            }
            set
            {
                if (value < 0)
                    epochs = 0;
                else
                    epochs = value;
            }
        }

        // Get real number (0,...,9) of data given by index
        public int Number(int index)
        {
            if (index < 0 || index >= dataset.Count)
                return -1;
            else
            {
                return Convert.ToInt32(dataset[index].Split(',')[0]);
            }
        }

        // Generate output data vector of data given by index
        // It's a 10d vector where n-th dimension is showing the weight of number n (=0,...,9)
        // Example: if target number is 3, the output vector is (0.01, 0.01, 0.01, 0.99, 0.01, 0.01, ..., 0.01)
        public Vector<double> Output(int index)
        {
            Vector<double> output = Vector<double>.Build.Dense(10, 0.01);

            if (index >= 0 && index < dataset.Count)
                output[Number(index)] = 0.99;

            return output;
        }

        // Generate input data vector of data given by index
        // It's a 28x28d vector based on the pixel values in byte grey scaling (values from 0 to 255 in original dataset)
        // Note that the first entry of a line shows the number, so the pixel array starts at index 1
        // Input data vector is rescaled from 0.01 to 1.0
        public Vector<double> Input(int index)
        {
            Vector<double> input = Vector<double>.Build.Dense(28 * 28, 0.01);

            if (index >= 0 && index < dataset.Count)
            {
                string[] rawdata = dataset[index].Split(',');
                double[] data = new double[28 * 28];

                for (int i = 0; i < data.Length; i++)
                    data[i]=Convert.ToDouble(rawdata[i+1])/255*0.99;

                input += Vector<double>.Build.DenseOfArray(data);
            }

            return input;
        }
    }
}
