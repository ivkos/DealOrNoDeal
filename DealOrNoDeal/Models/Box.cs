﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DealOrNoDeal.Support.Constants;

namespace DealOrNoDeal.Models
{
    public class Box : INotifyPropertyChanged
    {
        #region Properties
        public int Id { get; set; }
        public double Value { get; }
        public string ItemName { get; }

        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                OnPropertyChanged();
            }
        }

        private bool revealed;
        public bool Revealed
        {
            get { return revealed; }
            set
            {
                revealed = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public Box(double value)
        {
            Value = value;

            if (value == 5d)
            {
                ItemName = BoxPrices.Items[new Random().Next(BoxPrices.Items.Length)];
            }
        }

        public override string ToString()
        {
            string format = Value < 1 ? "0.00" : "";
            return ItemName ?? Value.ToString(format);
        }

        #region Equals & GetHashCode
        protected bool Equals(Box other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Box) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        #endregion

        #region INotifyPropertcyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
