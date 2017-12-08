using System;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    // Show results of testing with MNIST (table of performance by digit)
    public class ResultsMNISTPage : ContentPage
    {
        // Flag if popup message when calling Datails page has been shown
        static bool messageflag = false;

        public ResultsMNISTPage(NeuralNet neuralnet, MNISTDataManager mnistdata, Vector<double> digittotal, Vector<double> digitcorrect)
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

            for (int i = 0; i < 4; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            for (int i = 0; i < 12; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Add headlines to grid
            grid.Children.Add(new Label { Text = "Digit", Style = labelgridheadlineStyle }, 0, 0);
            grid.Children.Add(new Label { Text = "Total", Style = labelgridheadlineStyle }, 1, 0);
            grid.Children.Add(new Label { Text = "Correct", Style = labelgridheadlineStyle }, 2, 0);
            grid.Children.Add(new Label { Text = "in %", Style = labelgridheadlineStyle }, 3, 0);

            // Add results for each digit to grid
            for (int i = 0; i < 10; i++)
            {
                grid.Children.Add(new Label { Text = i.ToString(), Style = labelgridStyle }, 0, i + 1);
                grid.Children.Add(new Label { Text = String.Format("{0:N0}", digittotal[i]), Style = labelgridStyle }, 1, i + 1);
                grid.Children.Add(new Label { Text = String.Format("{0:N0}", digitcorrect[i]), Style = labelgridStyle }, 2, i + 1);
                grid.Children.Add(new Label { Text = String.Format("{0:P0}", digittotal[i] > 0 ? digitcorrect[i] / digittotal[i] : 0), Style = labelgridStyle }, 3, i + 1);
            }

            // Add sum to grid
            grid.Children.Add(new Label { Text = "Sum", Style = labelgridhighlightStyle }, 0, 11);
            grid.Children.Add(new Label { Text = String.Format("{0:N0}", digittotal.Sum()), Style = labelgridhighlightStyle }, 1, 11);
            grid.Children.Add(new Label { Text = String.Format("{0:N0}", digitcorrect.Sum()), Style = labelgridhighlightStyle }, 2, 11);
            grid.Children.Add(new Label { Text = String.Format("{0:P0}", digitcorrect.Sum() / digittotal.Sum()), Style = labelgridhighlightStyle }, 3, 11);

            // Define button for details
            Button button = new Button
            {
                Text = "Details",
                WidthRequest = Application.Current.MainPage.Width * 0.8,
                HeightRequest = Application.Current.MainPage.Height * 0.10,
                VerticalOptions = LayoutOptions.Center,
            };

            // Show page with detailed results for each digit
            button.Clicked += (s, e) =>
            {
                if (messageflag == false)
                {
                    DisplayAlert("Information","Look at the details by swiping left and right.","OK");
                    messageflag = true;
                }

                Navigation.PushAsync(new ResultsDetailsPage(neuralnet, mnistdata));
            };

            // Define page
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Children = { grid, button },
                    Spacing = Application.Current.MainPage.Height * 0.05
                }
            };
        }
    }
}
