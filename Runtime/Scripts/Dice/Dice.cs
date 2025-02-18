using UnityEngine;

namespace HHG.Common.Runtime
{
    public enum Dice
    {
        D4,
        D6,
        D8,
        D10,
        D12,
        D20
    }

    public static class DiceExtensions
    {
        public static int Roll(this Dice dice) => Random.Range(1, dice.Sides());

        public static int Sides(this Dice dice) => dice switch
        {
            Dice.D4 => 4,
            Dice.D6 => 6,
            Dice.D8 => 8,
            Dice.D10 => 10,
            Dice.D12 => 12,
            Dice.D20 => 20,
            _ => 0
        };
    }
}