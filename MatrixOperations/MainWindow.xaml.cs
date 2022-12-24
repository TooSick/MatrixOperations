using MatrixOperations.ViewModels;
using System.Windows;

namespace MatrixOperations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MatrixViewModel();
            InitializeComponent();
        }
    }
}
