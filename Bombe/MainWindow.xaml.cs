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

namespace Bombe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ComputingScheduler scheduler;

        public MainWindow()
        {
            InitializeComponent();
            scheduler = new ComputingScheduler(this);
            makePreparations();
            //scheduler.run();
        }

        private void makePreparations()
        {
            iplabel.Content = scheduler.getLocalIP();

            bindHandlers();
        }

        private void bindHandlers()
        {
            //this.cmdReceiveConnections.MouseUp += new RoutedEventHandler(this.cmdListen_Click);
            //this.cmdReceiveConnections.MouseUp += new MouseButtonEventHandler(this.cmdListen_Click);
            //this.cmdCompute.MouseUp += new
        }

        private void cmdListen_Click(object sender, System.EventArgs e)
        {
            scheduler.changeServerStatus();
        }

        private void cmdCompute_Click(object sender, System.EventArgs e)
        {
            scheduler.startBreaking();
        }

        public void mainListAppendText(string s)
        {
            mainlist.AppendText(s);
        }
    }
}
