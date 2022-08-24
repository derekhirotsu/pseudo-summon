using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PseudoSummon
{
    public class CameraHolder : MonoBehaviour
    {

        [SerializeField] protected Camera cam;
        [SerializeField] protected Image flashOverlay;
        private IEnumerator hitstopCoroutine;

        public void CameraShake(float duration, float magnitude)
        {
            if (cam != null)
            {
                StartCoroutine(ShakeForDuration(duration, magnitude));
            }
        }

        protected IEnumerator ShakeForDuration(float duration, float magnitude)
        {
            Quaternion og = cam.transform.localRotation;

            while (duration >= 0.0f)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                float z = Random.Range(-1f, 1f) * magnitude;

                cam.transform.localPosition = new Vector3(x, y, z);

                duration -= Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            cam.transform.localPosition = Vector3.zero;
        }

        public void CameraFlash(float speed)
        {
            if (flashOverlay != null)
            {
                StartCoroutine(FlashForDuration(speed));
            }
        }

        protected IEnumerator FlashForDuration(float flashSpeed)
        {
            while (flashOverlay.color.a < 1)
            {
                float newAlpha = flashOverlay.color.a + flashSpeed * Time.unscaledDeltaTime;
                if (newAlpha > 1)
                {
                    newAlpha = 1;
                }

                Color newColor = new Color(flashOverlay.color.r, flashOverlay.color.g, flashOverlay.color.b, newAlpha);
                flashOverlay.color = newColor;
                yield return new WaitForEndOfFrame();
            }

            while (flashOverlay.color.a > 0)
            {
                float newAlpha = flashOverlay.color.a - flashSpeed * 1.5f * Time.unscaledDeltaTime;
                if (newAlpha < 0)
                {
                    newAlpha = 0;
                }
                Color newColor = new Color(flashOverlay.color.r, flashOverlay.color.g, flashOverlay.color.b, newAlpha);
                flashOverlay.color = newColor;
                yield return new WaitForEndOfFrame();
            }
        }

        public void HitStop(float duration)
        {
            CancelHitStop();

            hitstopCoroutine = HitStopRoutine(duration);
            StartCoroutine(hitstopCoroutine);
        }

        public void CancelHitStop()
        {
            if (hitstopCoroutine != null)
            {
                StopCoroutine(hitstopCoroutine);
            }
        }

        private IEnumerator HitStopRoutine(float duration)
        {
            Time.timeScale = 0f;

            yield return new WaitForSecondsRealtime(duration);

            hitstopCoroutine = null;
            Time.timeScale = 1f;
        }
    }
}