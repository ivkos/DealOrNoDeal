using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DealOrNoDeal.Models;
using DealOrNoDeal.Support;
using DealOrNoDeal.Support.Constants;
using DealOrNoDeal.Views;

namespace DealOrNoDeal.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly GameView view;
        private readonly Random random = new Random();

        public List<Box> BluePrices { get; set; }
        public List<Box> RedPrices { get; set; }
        public List<Box> AllBoxes { get; set; } = new List<Box>();
        public ObservableCollection<GridBox> GridBoxes { get; set; }

        private Box currentBox;

        public Box CurrentBox
        {
            get { return currentBox; }
            set { this.currentBox = value; OnPropertyChanged(); }
        }

        #region Offer and offer text handling

        public double MoneyOffer { get; private set; }

        private int remainingBoxesToOpen = 5;
        public int RemainingBoxesToOpen
        {
            get { return remainingBoxesToOpen; }
            set
            {
                remainingBoxesToOpen = value;
                OnPropertyChanged("CurrentOfferText");
            }
        }

        private GameState currentGameState = GameState.KeepOpeningBoxes;

        public GameState CurrentGameState
        {
            get { return currentGameState; }

            set
            {
                currentGameState = value;
                OnPropertyChanged("HasOffer");
                OnPropertyChanged("CurrentOfferText");
                OnPropertyChanged("ClickCommand");
            }
        }

        public string CurrentOfferText
        {
            get
            {
                switch (CurrentGameState)
                {
                    case GameState.MoneyOfferPending:
                        return MoneyOffer.ToString();
                    case GameState.KeepOpeningBoxes:
                        return $"{RemainingBoxesToOpen} кути{(RemainingBoxesToOpen == 1 ? 'я' : 'и')}";
                    case GameState.SwapBoxesOfferPending:
                        return "смяна на кутиите";
                    case GameState.GameOver:
                        return $"Спечелихте {(MoneyOffer > 0 ? MoneyOffer + " лв." : CurrentBox.ItemName ?? CurrentBox + " лв.")}!";
                    default:
                        return null;
                }
            }
        }

        public bool HasOffer => CurrentGameState != GameState.KeepOpeningBoxes && CurrentGameState != GameState.GameOver;

        #endregion

        #region Constructor

        public GameViewModel(GameView view)
        {
            this.view = view;

            #region Fill boxes

            {
                BluePrices = BoxPrices.BluePrices
                    .OrderBy(_ => random.Next())
                    .Select(v => new Box(v))
                    .OrderBy(b => b.Value)
                    .ToList();

                RedPrices = BoxPrices.RedPrices
                    .OrderBy(_ => random.Next())
                    .Select(v => new Box(v))
                    .OrderBy(b => b.Value)
                    .ToList();

                AllBoxes.AddRange(BluePrices);
                AllBoxes.AddRange(RedPrices);
            }

            #endregion

            #region Pick a random box for the player

            {
                var allBoxes = new List<Box>();
                allBoxes.AddRange(BluePrices);
                allBoxes.AddRange(RedPrices);

                CurrentBox = allBoxes[new Random().Next(allBoxes.Count)];

                GridBoxes =
                    new ObservableCollection<GridBox>(
                        GridBox.Of(AllBoxes.Except(new Box[] { CurrentBox }).OrderBy(_ => random.Next())));
            }

            #endregion
        }

        #endregion

        #region Commands

        public ICommand ClickCommand
        {
            get
            {
                switch (CurrentGameState)
                {
                    case GameState.KeepOpeningBoxes:
                        return new DelegateCommand(OpenBox);
                    case GameState.SwapBoxesOfferPending:
                        return new DelegateCommand(SwapBoxes);
                    default:
                        return new DelegateCommand(_ => { });
                }
            }
        }


        private bool boxOpenInProgress = false;
        private int boxesToOpenCurrently = 5;
        public void OpenBox(object src)
        {
            if (boxOpenInProgress)
                return;

            Button btn = (Button) src;
            Box box = ((GridBox) btn.DataContext).Box;

            btn.Foreground = Brushes.White;
            if (BoxPrices.BluePrices.Any(v => v == box.Value))
                btn.Background = Brushes.RoyalBlue;
            else
                btn.Background = Brushes.DarkRed;

            btn.FontSize = 16;
            btn.Content = box.ToString();

            boxOpenInProgress = true;
            new Thread(() =>
            {
                Thread.Sleep(500);
                box.IsOpen = true;
                boxOpenInProgress = false;
                RemainingBoxesToOpen--;

                if (RemainingBoxesToOpen == 0)
                {
                    MoneyOffer = Banker.GetOfferForBoxes(AllBoxes);

                    if (MoneyOffer == 0d)
                    {
                        CurrentGameState = GameState.SwapBoxesOfferPending;
                    }
                    else
                    {
                        CurrentGameState = GameState.MoneyOfferPending;
                    }
                }
            }).Start();
        }

        public void SwapBoxes(object src)
        {
            Button btn = (Button) src;
            Box box = ((GridBox) btn.DataContext).Box;

            GridBoxes.FirstOrDefault(gb => gb.Box == box).Box = CurrentBox;
            CurrentBox = box;

            ResetOffer();

            if (AllBoxes.Count(b => !b.IsOpen) == 2)
            {
                CurrentGameState = GameState.GameOver;
                view.RevealAllBoxes();
            }
        }
        #endregion

        private void ResetOffer()
        {
            if (boxesToOpenCurrently > 2)
                boxesToOpenCurrently--;

            RemainingBoxesToOpen = boxesToOpenCurrently;

            CurrentGameState = GameState.KeepOpeningBoxes;
        }

        #region Deal/No Deal Commands
        public ICommand AcceptOfferCommand => new DelegateCommand(AcceptOffer);
        public ICommand DenyOfferCommand => new DelegateCommand(DenyOffer);

        public void AcceptOffer(object _)
        {
            if (CurrentGameState == GameState.MoneyOfferPending)
            {
                CurrentGameState = GameState.GameOver;
                view.RevealAllBoxes();
            }
        }

        public void DenyOffer(object _)
        {
            ResetOffer();

            if (AllBoxes.Count(b => !b.IsOpen) == 2)
            {
                CurrentGameState = GameState.GameOver;
                view.RevealAllBoxes();
            }
        }
        #endregion

        public enum GameState
        {
            KeepOpeningBoxes, SwapBoxesOfferPending, MoneyOfferPending, GameOver
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
