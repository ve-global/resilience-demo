using ResilienceDemo.Client.ViewModel;
using System.Windows;

namespace ResilienceDemo.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpammerViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new SpammerViewModel();
            
            this.DataContext = _viewModel;
        }
    }
}
