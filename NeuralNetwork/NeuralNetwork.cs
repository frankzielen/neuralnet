using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace NeuralNetwork
{
    public class App : Application
    {
        public App()
        {
            // Define global button style
            var buttonStyle = new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = Button.TextColorProperty, Value = Color.SteelBlue },
                    new Setter { Property = Button.BackgroundColorProperty, Value = Color.GhostWhite },
                    new Setter { Property = Button.BorderColorProperty, Value = Color.LightSteelBlue },
                    new Setter { Property = Button.BorderWidthProperty, Value = 1 },
                    new Setter { Property = Button.BorderRadiusProperty, Value = 10 },
                    new Setter { Property = Button.FontSizeProperty, Value = Device.GetNamedSize(NamedSize.Medium, typeof(Button)) },
                    new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },
                    new Setter { Property = Button.HorizontalOptionsProperty, Value = LayoutOptions.Center },
                    new Setter { Property = Button.VerticalOptionsProperty, Value = LayoutOptions.CenterAndExpand }
                }
            };

            // Define global label style
            var labelStyle = new Style(typeof(Label))
            {
                Setters =
                {
                    new Setter { Property = Label.TextColorProperty, Value = Color.GhostWhite },
                    new Setter { Property = Label.HorizontalOptionsProperty, Value = LayoutOptions.Center },
                    new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center }
                }
            };

            // Add styles to ResourceDictionary
            Resources = new ResourceDictionary();
            Resources.Add(buttonStyle);
            Resources.Add(labelStyle);

            // Call MenuPage
            MainPage = new NavigationPage(new MenuPage());

            // Set color for navigation bar
            (MainPage as NavigationPage).BarBackgroundColor = Color.SteelBlue;
            (MainPage as NavigationPage).BarTextColor = Color.GhostWhite;
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
