using System;
using Xamarin.Forms;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetwork
{
    public class ResultsSinglePage : ContentPage
    {
        public ResultsSinglePage(Vector<double> result)
        {
            // Define GUI

            // Page properties
            Title = "Results";
            BackgroundColor = Color.SteelBlue;
            Padding = new Thickness(Application.Current.MainPage.Width * 0.05, Application.Current.MainPage.Height * 0.05);


            // Define style for labels in grid
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

            // Define 11x3 grid
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

            Content = grid;
        }
    }
}
