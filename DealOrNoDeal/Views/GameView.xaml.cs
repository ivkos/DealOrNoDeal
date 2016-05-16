using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DealOrNoDeal.Models;
using DealOrNoDeal.Support.Constants;
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
            this.DataContext = new GameViewModel(this);
            InitializeComponent();
        }

        public void RevealAllBoxes()
        {
            for (int i = 0; i < BoxesGrid.Items.Count; i++)
            {
                ContentPresenter c = (ContentPresenter) BoxesGrid.ItemContainerGenerator.ContainerFromItem(BoxesGrid.Items[i]);
                Button btn = c.ContentTemplate.FindName("BoxButton", c) as Button;
                
                RevealButton(btn);
            }

            RevealPlayerBox(PlayerBox);
        }

        public void RevealButton(Button btn)
        {
            Box box = ((GridBox) btn.DataContext).Box;

            btn.Foreground = Brushes.White;

            if (BoxPrices.BluePrices.Any(v => v == box.Value))
                btn.Background = Brushes.RoyalBlue;
            else
                btn.Background = Brushes.DarkRed;

            btn.FontSize = 16;
            btn.Content = box.ToString();
        }

        public void RevealPlayerBox(Label label)
        {
            Box box = ((GameViewModel) label.DataContext).CurrentBox;

            label.Foreground = Brushes.White;

            if (BoxPrices.BluePrices.Any(v => v == box.Value))
                label.Background = Brushes.RoyalBlue;
            else
                label.Background = Brushes.DarkRed;

            label.FontSize = 16;
            label.Content = box.ToString();
        }
    }
}
