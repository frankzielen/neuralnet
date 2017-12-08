using System;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    // This class represents the result of testing a handwritten digit. It's also possible to use the pixel data for training.
    // Parameters are the neural net, the input data vector (=pixel data) and the result calculated from the neural net
    public class ResultsCamPage : ContentPage
    {
        public ResultsCamPage(NeuralNet neuralnet, Vector<double> input, Vector<double> result)
        {
            // Define GUI

            // Page properties
            Title = "Results";
            BackgroundColor = Color.SteelBlue;
            Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);


            // Define styles for labels in grid
            var labelgridStyle = new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter { Property = Label.TextColorProperty, Value = Color.GhostWhite },
                    new Setter { Property = Label.BackgroundColorProperty, Value = Color.SteelBlue },
                    new Setter { Property = Label.HorizontalOptionsProperty, Value = LayoutOptions.FillAndExpand },
                    new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
                    new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.FillAndExpand },
                    new Setter { Property = Label.VerticalTextAlignmentProperty, Value = TextAlignment.Center }
                }
            };

            var labelgridhighlightStyle = new Style(typeof(Label))
            {
                BasedOn = labelgridStyle,
                Setters =
                {
                    new Setter { Property = Label.TextColorProperty, Value = Color.GhostWhite },
                    new Setter { Property = Label.BackgroundColorProperty, Value = Color.DarkOrange },
                    new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold }
                }
            };

            var labelgridheadlineStyle = new Style(typeof(Label))
            {
                BasedOn = labelgridStyle,
                Setters =
                {
                    new Setter { Property = Label.TextColorProperty, Value = Color.SteelBlue },
                    new Setter { Property = Label.BackgroundColorProperty, Value = Color.GhostWhite },
                    new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold }
                }
            };

            // Define 11x3 grid for representing results 
            Grid grid = new Grid();

            grid.BackgroundColor = Color.GhostWhite;
            grid.ColumnSpacing = 1;
            grid.RowSpacing = 1;
            grid.Padding = new Thickness(1);
            grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            grid.VerticalOptions = LayoutOptions.FillAndExpand;

            for (int i = 0; i < 3; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });               
            for (int i = 0; i < 11; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Add headlines to grid
            grid.Children.Add(new Label { Text = "Digit", Style = labelgridheadlineStyle }, 0, 0);
            grid.Children.Add(new Label { Text = "Ouput", Style = labelgridheadlineStyle }, 1, 0);
            grid.Children.Add(new Label { Text = "Share", Style = labelgridheadlineStyle }, 2, 0);

            // Add content to grid
            for (int i = 0; i < 10; i++)
            {
                if (result.AbsoluteMaximumIndex() == i)
                {
                    grid.Children.Add(new Label { Text = i.ToString(), Style = labelgridhighlightStyle }, 0, i+1);
                    grid.Children.Add(new Label { Text = String.Format("{0:N3}", result[i]), Style = labelgridhighlightStyle }, 1, i+1);
                    grid.Children.Add(new Label { Text = String.Format("{0:P1}", result[i] / result.Sum()), Style = labelgridhighlightStyle }, 2, i+1);                     
                }
                else
                {
                    grid.Children.Add(new Label { Text = i.ToString(), Style = labelgridStyle }, 0, i+1);
                    grid.Children.Add(new Label { Text = String.Format("{0:N3}", result[i]), Style = labelgridStyle }, 1, i+1);
                    grid.Children.Add(new Label { Text = String.Format("{0:P1}", result[i] / result.Sum()), Style = labelgridStyle }, 2, i+1);                     
                }
               
            }

            // Define 2x3 grid for training net with own handwriting
            Grid grid2 = new Grid();

            grid2.BackgroundColor = Color.Transparent;
            grid2.ColumnSpacing = 0;
            grid2.RowSpacing = 0;
            grid2.HorizontalOptions = LayoutOptions.Center;

            grid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8, GridUnitType.Star) });
            grid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

            grid2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Place content
            grid2.Children.Add(new Label { Text = "Net's answer:", Style = labelgridStyle, HorizontalTextAlignment = TextAlignment.Start }, 0, 0);
            grid2.Children.Add(new Label { Text = "Correct answer:", Style = labelgridStyle, HorizontalTextAlignment = TextAlignment.Start }, 0, 1);
            grid2.Children.Add(new Label { Text = result.AbsoluteMaximumIndex().ToString(), Style = labelgridStyle }, 1, 0);

            // labelresult keeps correct answer in Text property
            Label labelresult = new Label { Text = result.AbsoluteMaximumIndex().ToString(), Style = labelgridStyle };
            grid2.Children.Add(labelresult, 1, 1);

            // Buttons for changing value of correct answer
            Button buttondown = new Button() { Text = "-" };
            grid2.Children.Add(buttondown, 3, 4, 0, 2);
            buttondown.Clicked += (s, e) =>
            {
                if (Convert.ToInt32(labelresult.Text) > 0)
                    labelresult.Text = (Convert.ToInt32(labelresult.Text) - 1).ToString();
                else
                    labelresult.Text = "9";
            };

            Button buttonup = new Button() { Text = "+" };
            grid2.Children.Add(buttonup, 5, 6, 0, 2);
            buttonup.Clicked += (s, e) =>
            {
                if (Convert.ToInt32(labelresult.Text) < 9)
                    labelresult.Text = (Convert.ToInt32(labelresult.Text) + 1).ToString();
                else
                    labelresult.Text = "0";
            };

            // Define button to train net
            Button button = new Button
            {
                Text = "Train this Digit",
                WidthRequest = Application.Current.MainPage.Width * 0.8,
                HeightRequest = Application.Current.MainPage.Height * 0.10,
                VerticalOptions = LayoutOptions.Center,
            };

            // Train net
            button.Clicked += (s, e) =>
            {
                // Determine output vector from correct answer
                Vector<double> output = Vector<double>.Build.Dense(10, 0.01);
                output[Convert.ToInt32(labelresult.Text)] = 0.99;

                // Train net with this data set
                neuralnet.Train(input, output);

                // Increase number of training data sets used so far
                neuralnet.TrainingDataCounter++;

                DisplayAlert("Training completed",string.Format("Net trained with {0} digit{1} so far.",neuralnet.TrainingDataCounter, (neuralnet.TrainingDataCounter == 1 ? "" : "s")), "OK");
            };

            // Define page
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Children = { grid, grid2, button },
                    Spacing = Application.Current.MainPage.Height * 0.05
                }
            };
        }
    }
}
