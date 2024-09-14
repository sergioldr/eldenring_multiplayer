using System.Collections;
using TMPro;
using UnityEngine;

namespace SL
{
    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("YOU DIED Pop Up")]
        [SerializeField] private GameObject youDiedPopUp;
        [SerializeField] private TextMeshProUGUI youDiedBackgroundText;
        [SerializeField] private TextMeshProUGUI youDiedText;
        [SerializeField] private CanvasGroup youDiedPopUpCanvasGroup;

        public void SendYouDiedPopUp()
        {
            youDiedPopUp.SetActive(true);
            youDiedBackgroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(youDiedBackgroundText, 8f, 20f));
            StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5f));
            StartCoroutine(FadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2f, 5f));
        }

        private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
        {
            if (duration > 0f)
            {
                text.characterSpacing = 0;
                float timer = 0;
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                    yield return null;
                }
            }
        }

        private IEnumerator FadeInPopUpOverTime(CanvasGroup canvasGroup, float duration)
        {
            if (duration > 0f)
            {
                canvasGroup.alpha = 0;
                float timer = 0;
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvasGroup.alpha = 1;

            yield return null;
        }

        private IEnumerator FadeOutPopUpOverTime(CanvasGroup canvasGroup, float duration, float delay)
        {
            if (duration > 0f)
            {
                while (delay > 0)
                {
                    delay -= Time.deltaTime;
                    yield return null;
                }

                canvasGroup.alpha = 0;
                float timer = 0;
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvasGroup.alpha = 0;

            yield return null;
        }
    }
}
