using System.Windows;

namespace MARCUS
{
    public partial class ErrorOkCancel : Window
    {
        public ErrorOkCancel()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Marcosi.PDF.PdfErrorChoice = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Marcosi.PDF.PdfErrorChoice = false;
            Close();
        }
    }
}