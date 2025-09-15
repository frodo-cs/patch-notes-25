using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Puzzles
{
    public class Simon : MonoBehaviour
    {
        [SerializeField] private string itemId;
        [SerializeField] private int maxRounds = 5;
        [SerializeField] private Screen[] screens;
        [SerializeField] private SimonButton button;

        [Header("Audio Setup")]
        [SerializeField] private float duration = 0.2f;

        public static event Action OnSimonComplete;
        private List<int> levelScreens = new();
        private int currentIndex = 0;

        enum State
        {
            None,
            Listening,
            Playing,
        }

        private State currentState = State.None;

        private void Start()
        {
            foreach (var screen in screens)
            {
                screen.TurnOff();
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
                        if (levelScreens.Count <= maxRounds)
                        {
                            levelScreens.Add(UnityEngine.Random.Range(0, screens.Length));
                            StartCoroutine(PlaySequence());
                        } else
                        {
                            StartCoroutine(HandleEnd(true));
                        }
                    }
                } else
                {
                    currentState = State.None;
                    StartCoroutine(HandleEnd(false));
                    button.Active = true;
                }
            }
        }

        public void Play()
        {
            button.Active = false;

            levelScreens = new()
            {
                UnityEngine.Random.Range(0, screens.Length),
                UnityEngine.Random.Range(0, screens.Length),
                UnityEngine.Random.Range(0, screens.Length)
            };

            StartCoroutine(PlaySequence());
        }

        private IEnumerator PlaySequence()
        {
            currentState = State.Listening;
            yield return new WaitForSeconds(1.75f);

            foreach (int index in levelScreens)
            {
                yield return FlashScreen(index);
                yield return new WaitForSeconds(duration);
            }

            currentIndex = 0;
            currentState = State.Playing;
        }

        private IEnumerator HandleEnd(bool success)
        {
            currentState = State.None;

            for (int i = 0; i < 3; i++)
            {
                foreach (var screen in screens)
                {
                    if (success)
                    {
                        screen.SetSuccess();
                    } else
                    {
                        screen.SetFailed();
                    }
                }

                yield return new WaitForSeconds(0.25f);

                foreach (var screen in screens)
                    screen.TurnOff();

                yield return new WaitForSeconds(0.25f);
            }

            if (success)
            {
                button.Active = false;
                OnSimonComplete?.Invoke();
            } else
            {
                currentState = State.Listening;
                currentIndex = 0;
                button.Active = true;
            }

        }

    }
}

