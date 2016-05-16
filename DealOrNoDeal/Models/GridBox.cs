using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DealOrNoDeal.Models
{
    public class GridBox : INotifyPropertyChanged
    {
        private const int GridColumns = 7;
        private const int GridRows = 3;

        private Box box;

        public Box Box
        {
            get { return box; }
            set { this.box = value; OnPropertyChanged(); }
        }

        public int Column { get; private set; }
        public int Row { get; private set; }

        public GridBox(Box box, int column, int row)
        {
            Box = box;
            Column = column;
            Row = row;
        }

        public static IEnumerable<GridBox> Of(IEnumerable<Box> boxes)
        {
            if (boxes.Count() > GridColumns * GridRows)
                throw new Exception("Too many boxes!");

            int currentColumn = 0, currentRow = 0;

            return boxes.Select(b =>
            {
                var gb = new GridBox(b, currentColumn, currentRow);

                if (++currentColumn >= GridColumns)
                {
                    currentColumn = 0;
                    currentRow++;
                }

                return gb;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
