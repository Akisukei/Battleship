using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BattleShipV1
{
    /// <summary>
    /// Interaction logic for pictureLegend.xaml
    /// </summary>
    public partial class pictureLegend : Window
    {
        int firstPage = 1;
        public pictureLegend()
        {
            InitializeComponent();

            if (firstPage == 1)
            {
                btnPrevious.IsEnabled = false;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var img = new ImageBrush();

            if (btn.Name == "btnNext")
            {
                firstPage++;
                btnPrevious.IsEnabled = true;
            }
            else if (btn.Name == "btnPrevious")
            { 
                firstPage--;
                btnNext.IsEnabled = true;
            }

            if (firstPage == 1)
                btnPrevious.IsEnabled = false;
            if (firstPage == 3)
                btnNext.IsEnabled = false;

            if (firstPage == 1)
                img.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/legend/001.png", UriKind.RelativeOrAbsolute));
            else if (firstPage == 2)
                img.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/legend/002.png", UriKind.RelativeOrAbsolute));
            else if (firstPage == 3)
                img.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/legend/003.png", UriKind.RelativeOrAbsolute));
            
            legend.Background = img;
        }
    }
}
