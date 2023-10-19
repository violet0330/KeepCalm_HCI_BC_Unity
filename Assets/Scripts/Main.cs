using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace KeepCalm
{
    public class Main : MonoBehaviour
    {
        public GameObject[] stars;
        public AudioSource[] musics;

        internal float destroyDelay = 4f;
        internal float generationTime = 0.5f;
        internal float fadeTime = 1f;

        internal float musicFadeTime = 1f;
        int currentMusicIndex;
        int changeMusicIndex;
        bool musicCanChange;

        List<GameObject> starsList = new List<GameObject>();

        float frequency;
        Queue<float> clickIntervalQueue = new Queue<float>();
        int clickN = 5;
        float clickIntervalAvg;

        // Start is called before the first frame update
        void Start()
        {
            musics[1].Play();
            currentMusicIndex = 1;
            musicCanChange = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // ¼ì²éÊó±ê×ó¼üµã»÷
            {
                CreateStars();
                Mousefrequency();
                MusicDetect();

            }


        }

        #region Stars
        void CreateStars()
        {
            int index = Random.Range(0, stars.Length);
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = new Vector3(mousePosition.x, mousePosition.y, -1f);

            GameObject starClone = Instantiate(stars[index], position, Quaternion.identity);
            Vector3 localScale = stars[index].transform.localScale;
            starClone.transform.localScale = Vector3.zero;
            starsList.Add(starClone);

            starClone.transform.DOScale(localScale, generationTime).SetEase(Ease.OutBounce);
            starClone.transform.DOScale(localScale, generationTime).SetEase(Ease.OutBounce).OnComplete(() =>
               {
                   Invoke("DestroyObject", destroyDelay);
               });

        }

        void DestroyObject()
        {
            if (starsList.Count > 0)
            {
                GameObject starDestroy = starsList[0]; // Remove the first spawned object
                starsList.RemoveAt(0);
                starDestroy.transform.DOScale(Vector3.zero, fadeTime).SetEase(Ease.InBack);
                starDestroy.transform.DOScale(Vector3.zero, fadeTime).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(starDestroy);
                });
            }
        }
        #endregion

        #region Mouse-Speed
        void Mousefrequency()
        {
            float currentTime = Time.time;
            //Debug.Log(currentTime);
            clickIntervalQueue.Enqueue(currentTime);

            if (clickIntervalQueue.Count > clickN)
                clickIntervalQueue.Dequeue();
            if (clickIntervalQueue.Count == clickN)
            {
                clickIntervalAvg = IntercalCalculate();
            }

            FigRatioChange(clickIntervalAvg);
        }

        void FigRatioChange(float interval)
        {
            frequency = 1.2f / interval;
            float factor;
            //Debug.Log(frequency);
            if (frequency >= 4.5)
            { factor = 4.5f; }
            else if (frequency <= 0.8)
            { factor = 0.8f; }
            else
            { factor = frequency; }
            MoveBackground.ratio = factor * 1f;
            //Debug.Log(MoveBackground.ratio);
        }

        float IntercalCalculate()
        {
            float sum = 0f;
            float[] intervalArray = clickIntervalQueue.ToArray();
            for (int i = 1; i < intervalArray.Length; i++)
            {
                //Debug.Log(intervalArray[i]);
                sum += intervalArray[i] - intervalArray[i - 1];
            }
            return sum / (clickN - 1);
        }
        #endregion

        #region Musics
        void MusicDetect()
        {
            if (frequency >= 3f)
            { changeMusicIndex = 2; }
            else if (frequency <= 1.2)
            { changeMusicIndex = 0; }
            else
            { changeMusicIndex = 1; }

            if (changeMusicIndex != currentMusicIndex && musicCanChange)
            {
                Debug.Log(currentMusicIndex + "//" + changeMusicIndex);
                MusicChange(musics[currentMusicIndex], musics[changeMusicIndex]);

                currentMusicIndex = changeMusicIndex;
                musicCanChange = false;
                //Debug.Log("musicCanChange" + musicCanChange);
                StartCoroutine(WaitMusicChange());
            }
        }

        void MusicChange(AudioSource before, AudioSource after)
        {
            StartCoroutine(MusicFade(before, after));
        }

        IEnumerator MusicFade(AudioSource before, AudioSource after)
        {
            float startVolume = before.volume;
            for (float t = 0; t < musicFadeTime; t += Time.deltaTime)
            {
                before.volume = Mathf.Lerp(startVolume, 0, t / musicFadeTime);
                yield return null;
            }
            before.Stop();
            before.volume = startVolume;

            after.Play();
            for (float t = 0; t < musicFadeTime; t += Time.deltaTime)
            {
                after.volume = Mathf.Lerp(0, startVolume, t / musicFadeTime);
                yield return null;
            }
            after.volume = startVolume;
        }

        IEnumerator WaitMusicChange()
        {
            yield return new WaitForSeconds(3f);
            musicCanChange = true;
            Debug.Log("musicCanChange" + musicCanChange);
        }

        #endregion
    }
}