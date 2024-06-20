using System;

namespace HHG.Common.Runtime
{
    public enum EaseType
    {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InSine,
        OutSine,
        InOutSine,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce
    }

    public static class EaseExtensions
    {
        public static float Ease(this EaseType easeType, float t)
        {
            return easeType.GetEase().Invoke(t);
        }

        public static Func<float, float> GetEase(this EaseType easeType)
        {
            switch (easeType)
            {
                case EaseType.Linear: return EaseUtil.Linear;
                case EaseType.InQuad: return EaseUtil.InQuad;
                case EaseType.OutQuad: return EaseUtil.OutQuad;
                case EaseType.InOutQuad: return EaseUtil.InOutQuad;
                case EaseType.InCubic: return EaseUtil.InCubic;
                case EaseType.OutCubic: return EaseUtil.OutCubic;
                case EaseType.InOutCubic: return EaseUtil.InOutCubic;
                case EaseType.InQuart: return EaseUtil.InQuart;
                case EaseType.OutQuart: return EaseUtil.OutQuart;
                case EaseType.InOutQuart: return EaseUtil.InOutQuart;
                case EaseType.InQuint: return EaseUtil.InQuint;
                case EaseType.OutQuint: return EaseUtil.OutQuint;
                case EaseType.InOutQuint: return EaseUtil.InOutQuint;
                case EaseType.InSine: return EaseUtil.InSine;
                case EaseType.OutSine: return EaseUtil.OutSine;
                case EaseType.InOutSine: return EaseUtil.InOutSine;
                case EaseType.InExpo: return EaseUtil.InExpo;
                case EaseType.OutExpo: return EaseUtil.OutExpo;
                case EaseType.InOutExpo: return EaseUtil.InOutExpo;
                case EaseType.InCirc: return EaseUtil.InCirc;
                case EaseType.OutCirc: return EaseUtil.OutCirc;
                case EaseType.InOutCirc: return EaseUtil.InOutCirc;
                case EaseType.InElastic: return EaseUtil.InElastic;
                case EaseType.OutElastic: return EaseUtil.OutElastic;
                case EaseType.InOutElastic: return EaseUtil.InOutElastic;
                case EaseType.InBack: return EaseUtil.InBack;
                case EaseType.OutBack: return EaseUtil.OutBack;
                case EaseType.InOutBack: return EaseUtil.InOutBack;
                case EaseType.InBounce: return EaseUtil.InBounce;
                case EaseType.OutBounce: return EaseUtil.OutBounce;
                case EaseType.InOutBounce: return EaseUtil.InOutBounce;
                default: return EaseUtil.Linear;
            }
        }
    }
}