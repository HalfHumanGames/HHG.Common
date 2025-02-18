using UnityEngine;

namespace HHG.Common.Runtime
{
    [System.Serializable]
    public class DiceSprites
    {
        public Sprite D4Sprite => d4Sprite;
        public Sprite D6Sprite => d6Sprite;
        public Sprite D8Sprite => d8Sprite;
        public Sprite D10Sprite => d10Sprite;
        public Sprite D12Sprite => d12Sprite;
        public Sprite D20Sprite => d20Sprite;

        [SerializeField] private Sprite d4Sprite;
        [SerializeField] private Sprite d6Sprite;
        [SerializeField] private Sprite d8Sprite;
        [SerializeField] private Sprite d10Sprite;
        [SerializeField] private Sprite d12Sprite;
        [SerializeField] private Sprite d20Sprite;

        public Sprite GetDiceSprite(Dice dice) => dice switch
        {
            Dice.D4 => D4Sprite,
            Dice.D6 => D6Sprite,
            Dice.D8 => D8Sprite,
            Dice.D10 => D10Sprite,
            Dice.D12 => D12Sprite,
            Dice.D20 => D20Sprite,
            _ => null
        };
    }
}