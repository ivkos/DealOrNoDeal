using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DealOrNoDeal.Support.Constants;

namespace DealOrNoDeal.Models
{
    public class Box : INotifyPropertyChanged
    {
        private static int IdCounter = 0;

        public int Id { get; private set; } = ++IdCounter;
        public double Value { get; }
        public string ItemName { get; }

        private bool isOpen = false;
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                OnPropertyChanged();
            }
        }

        public Box(double value)
        {
            Value = value;

            if (value == 5d)
            {
                ItemName = BoxPrices.Items[new Random().Next(BoxPrices.Items.Length)];
            }
        }

        public Box(string itemName)
        {
            Value = 5;
            ItemName = itemName;
        }

        public override string ToString()
        {
            string format = Value < 1 ? "0.00" : "";
            return ItemName ?? Value.ToString(format);
        }

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
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Box) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
