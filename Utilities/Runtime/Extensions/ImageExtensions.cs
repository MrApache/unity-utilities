using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class ImageExtensions
    {
        public static IEnumerator LerpFillAmount(this Image image, float time, float startValue = 0, float endValue = 1)
        {
            float timeLeft = time;
            while (timeLeft > 0f)
            { 
                timeLeft -= Time.deltaTime;
                image.fillAmount = Mathf.Lerp(startValue, endValue, 1 -  timeLeft / time);
                yield return null;
            }
        }
    }
}