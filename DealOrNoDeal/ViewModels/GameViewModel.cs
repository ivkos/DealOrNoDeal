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

                // Pick a random box for the player
                CurrentBox = AllBoxes[random.Next(AllBoxes.Count)];

                // Fill grid with boxes
                GridBoxes = new ObservableCollection<GridBox>(GridBox.Of(AllBoxes.Except(new[] { CurrentBox }).OrderBy(_ => random.Next())));
            }
            #endregion
        }
        #endregion

        #region Boxes collections
        public List<Box> BluePrices { get; set; }
        public List<Box> RedPrices { get; set; }
        public List<Box> AllBoxes { get; set; } = new List<Box>();
        public ObservableCollection<GridBox> GridBoxes { get; set; }
        #endregion

        #region CurrentBox
        private Box currentBox;
        public Box CurrentBox
        {
            get { return currentBox; }
            set { this.currentBox = value; OnPropertyChanged(); }
        }
        #endregion

        #region Game state properties
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
        private int boxesToOpenInRound = 5;

        public void OpenBox(object element)
        {
            if (boxOpenInProgress)
                return;

            Button btn = (Button) element;
            Box box = ((GridBox) btn.DataContext).Box;

            btn.Foreground = Brushes.White;
            btn.Background = BoxPrices.BluePrices.Any(v => v == box.Value) ? Brushes.RoyalBlue : Brushes.DarkRed;
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
                    CurrentGameState = MoneyOffer == 0d ? GameState.SwapBoxesOfferPending : GameState.MoneyOfferPending;
                }
            }).Start();
        }

        public void SwapBoxes(object element)
        {
            Button btn = (Button) element;
            Box box = ((GridBox) btn.DataContext).Box;

            GridBoxes.FirstOrDefault(gb => gb.Box == box).Box = CurrentBox;
            CurrentBox = box;

            ResetGameState();

            if (AllBoxes.Count(b => !b.IsOpen) == 2)
            {
                CurrentGameState = GameState.GameOver;
                view.RevealAllBoxes();
            }
        }

        #region Accept/Deny Offer Commands
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
            ResetGameState();

            if (AllBoxes.Count(b => !b.IsOpen) == 2)
            {
                CurrentGameState = GameState.GameOver;
                view.RevealAllBoxes();
            }
        }
        #endregion

        private void ResetGameState()
        {
            if (boxesToOpenInRound > 2)
                boxesToOpenInRound--;

            RemainingBoxesToOpen = boxesToOpenInRound;

            CurrentGameState = GameState.KeepOpeningBoxes;
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
