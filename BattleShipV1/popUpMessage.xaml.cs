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
    /// Interaction logic for popUpMessage.xaml
    /// </summary>
    public partial class popUpMessage : Window
    {
        public popUpMessage()
        {
            InitializeComponent();
            timer();
        }

        private async void timer() { await delay(); }
        private async Task delay()
        {
            await Task.Delay(1500); //Creates a task that completes after a time delay.
            this.Close();
            
        }
    }
}
