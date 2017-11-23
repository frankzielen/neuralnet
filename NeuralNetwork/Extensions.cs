using System;
namespace NeuralNetwork
{
    public static class Extensions
    {
        // Return string with first char upper and reminder chars lower
        public static string ToUpperFirstOnly(this String text)
        {
            switch(text.Length)
            {
                case 0:
                    return text;
                case 1:
                    return text.ToUpper();
                default:
                    return text.ToUpper().Substring(0, 1) + text.ToLower().Substring(1, text.Length - 1);
            }
        }
    }  
}
