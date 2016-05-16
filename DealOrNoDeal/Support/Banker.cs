using System;
using System.Collections.Generic;
using System.Linq;
using DealOrNoDeal.Models;

namespace DealOrNoDeal.Support
{
    public class Banker
    {
        private static readonly Random random = new Random();

        public static double GetOfferForBoxes(List<Box> boxes)
        {
            // 25% chance to be offered a box swap
            if (random.Next(4) == 0)
            {
                return 0d; // 0 means box swap
            }

            // If there are a total of 2 boxes left in the game, always offer a swap to make it more interesting
            if (boxes.Count(b => !b.IsOpen) <= 2)
            {
                return 0d;
            }

            // Otherwise get a close average of all remaining boxes
            double average = boxes
                .Where(b => !b.IsOpen)
                .Average(b => b.Value);


            // 20% chance to rip off the player with 30% of the average
            if (random.Next(5) == 0)
                average *= 0.7;

            average = Math.Round(average);

            return average;
        }
    }
}
