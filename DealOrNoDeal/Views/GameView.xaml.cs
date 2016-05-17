using System.Windows;
using DealOrNoDeal.ViewModels;

namespace DealOrNoDeal.Views
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : Window
    {
        public GameView()
        {
            DataContext = new GameViewModel();
            InitializeComponent();
        }
    }
}
