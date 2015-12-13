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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BombeClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ComputingExecutor executor;

        public MainWindow()
        {
            InitializeComponent();
            Bridge.setWindow(this);
            executor = new ComputingExecutor();
            makePreparations();
        }

        private void makePreparations()
        {
            bindHandlers();
            setCoresAmount();
        }

        private void setCoresAmount()
        {
            int coresAmount = Bridge.computingSide.findProcessorCoresAmount();
            for (int i = 1; i <= coresAmount; i++)
            {
                coressamount.Items.Add(i);
            }
            coressamount.SelectedIndex = coressamount.Items.Count - 1;
        }

        private void bindHandlers()
        {
            this.cmdReceiveConnections.MouseUp += new MouseButtonEventHandler(this.cmdListen_Click);
        }

        private void cmdListen_Click(object sender, System.EventArgs e)
        {
            executor.changeClientStatus();
        }
    }
}
