using System.Windows;

namespace MARCUS
{
    public partial class Error : Window
    {
        public Error()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
