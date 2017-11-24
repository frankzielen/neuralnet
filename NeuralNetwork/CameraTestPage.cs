using System;
using Xamarin.Forms;
using Plugin.Media;

namespace NeuralNetwork
{
    public class CameraTestPage : ContentPage
    {
        public CameraTestPage()
        {
            // Define GUI
            Title = "Test Neural Net";

            BackgroundColor = Color.SteelBlue;

            Button button = new Button { Text = "Take a Picture" };
            Image image = new Image();

            Content = new StackLayout
            {
                Children = {button, image}
            };

            button.Clicked += async (sender, args) =>
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg",
                    SaveToAlbum = true,
                    AllowCropping = true
                });

                if (file == null)
                    return;

                await DisplayAlert("File Location", file.Path, "OK");

                image.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    file.Dispose();
                    return stream;
                });

                //or:
                //image.Source = ImageSource.FromFile(file.Path);
                //image.Dispose();
            };
        }
    }
}
