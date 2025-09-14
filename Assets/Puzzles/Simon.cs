using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Player.Puzzles
{
    public class Simon : MonoBehaviour
    {
        [SerializeField] private Screen[] screens;
        [SerializeField] private SimonButton button;

        [Header("Audio Setup")]
        [SerializeField] private float duration = 0.2f;

        private List<int> levelScreens = new();
        private int currentIndex = 0;

        enum State
        {
            None,
            Listening,
            Playing
        }

        private State currentState = State.None;

        private void Start()
        {
            foreach (var screen in screens)
            {
                screen.TurnOff();
            }
            StartCoroutine(MenuTileAnimation());
        }

        private IEnumerator MenuTileAnimation()
        {
            while (currentState == State.None)
            {
                yield return FlashScreen(Random.Range(0, screens.Length));
                yield return new WaitForSeconds(duration);
            }
        }

        private IEnumerator FlashScreen(int index)
        {
            screens[index].TurnOn();
            yield return new WaitForSeconds(duration);
            screens[index].TurnOff();
        }

        public void PlayLight(int index)
        {
            if (currentState == State.Playing)
            {
                StartCoroutine(FlashScreen(index));
                if (index == levelScreens[currentIndex])
                {
                    currentIndex++;
                    if (currentIndex == levelScreens.Count)
                    {
                        levelScreens.Add(Random.Range(0, screens.Length));
                        StartCoroutine(PlaySequence());
                    }
                } else
                {
                    currentState = State.None;
                    StartCoroutine(HandleFailure());
                    button.Active = true;
                }
            }
        }

        public void Play()
        {
            button.Active = false;
            StopCoroutine(MenuTileAnimation());

            levelScreens = new()
            {
                Random.Range(0, screens.Length),
                Random.Range(0, screens.Length),
                Random.Range(0, screens.Length)
            };

            StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            currentState = State.Listening;
            yield return new WaitForSeconds(2f);

            foreach (int index in levelScreens)
            {
                yield return FlashScreen(index);
                yield return new WaitForSeconds(duration);
            }

            currentIndex = 0;
            currentState = State.Playing;
        }

        private IEnumerator HandleFailure()
        {
            currentState = State.None;

            for (int i = 0; i < 3; i++)
            {
                foreach (var screen in screens)
                    screen.SetFailed();

                yield return new WaitForSeconds(0.25f);

                foreach (var screen in screens)
                    screen.TurnOff();

                yield return new WaitForSeconds(0.25f);
            }

            currentState = State.Listening;
            currentIndex = 0;
        }
    }
}

