using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace Cinematics {
    [RequireComponent(typeof(AudioSource))]
    public class DialogBoxController : MonoBehaviour {
        public UIDocument UI;

        VisualElement dialogBox;
        Label textElement;
        Label ActorName;

        VisualElement responsesBox;
        Label[] Responses = new Label[4];

        //Text sound source
        [SerializeField] AudioSource source;

        //Custom animations
        PlayableDirector timeLine;

        #region Events

        /// <summary>
        /// Play dialog from any script
        /// </summary>
        public static Action<Dialog> PlayDialog;

        /// <summary>
        /// Play dialog with multi-option list
        /// </summary>
        public static Action<Question> PlayQuestion;

        /// <summary>
        /// Callback when a dialog stars playing
        /// </summary>
        public static Action OnDialogStars;

        /// <summary>
        /// Callback when a dialog ends playing
        /// </summary>
        public static Action OnDialogEnds;

        /// <summary>
        /// Callback when an option for a question is selected
        /// </summary>
        public static Action<byte> OnQuestionEnds;

        /// <summary>
        /// Callback when player press <b>Interact</b> while a dialog is showing, if it is fully writen the dialog ends. If not, the dialog is instantly written and skipped
        /// </summary>
        public static Action InteractOnDialog;

        public delegate bool returnDialogRunning();
        public static returnDialogRunning IsDialogRunning;

        /// <summary>
        /// Gets the UI Document
        /// </summary>
        public delegate object returnDelegate();
        public static returnDelegate getUI;

        #endregion

        Coroutine currentCorrutine;
        Coroutine dialogWrittingCorroutine;

        private void Awake() {
            timeLine = GetComponentInChildren<PlayableDirector>();

            IsDialogRunning = isDialogRunning;

            textElement = UI.rootVisualElement.Q("Text") as Label;
            ActorName = UI.rootVisualElement.Q("ActorName") as Label;
            dialogBox = UI.rootVisualElement.Q("DialogContainer");

            //Responses variables
            responsesBox = UI.rootVisualElement.Q("Responses");
            Responses[0] = UI.rootVisualElement.Q("LeftResponse") as Label;
            Responses[1] = UI.rootVisualElement.Q("RightResponse") as Label;
            Responses[2] = UI.rootVisualElement.Q("UpperResponse") as Label;
            Responses[3] = UI.rootVisualElement.Q("DownResponse") as Label;

            //Responses callbacks
            Responses[0].RegisterCallback<ClickEvent>(SelectResponse);
            Responses[1].RegisterCallback<ClickEvent>(SelectResponse);
            Responses[2].RegisterCallback<ClickEvent>(SelectResponse);
            Responses[3].RegisterCallback<ClickEvent>(SelectResponse);

            //Events
            PlayDialog = DialogCorrutine;
            PlayQuestion = QuestionCorrutine;
            getUI = getUIDocument;
        }

        #region Dialog

        void DialogCorrutine(Dialog d) {
            if(ChangeScene.isSceneTransitioning) return;

            if(currentCorrutine != null) StopCoroutine(currentCorrutine);

            currentCorrutine = StartCoroutine(corrutine());

            IEnumerator corrutine() {
                yield return null;

                //Show Dialog data in UI
                dialogBox.style.display = DisplayStyle.Flex;
                DisplayActiveCharacters(d);

                textElement.text = "";

                //Play animation
                if(d.introAnim != null) timeLine.Play(d.introAnim);

                //Prepare events
                OnDialogStars?.Invoke();
                InteractOnDialog = SkipDialog;

                //Write text
                dialogWrittingCorroutine = StartCoroutine(WriteSpeechInBox(d));
                yield return dialogWrittingCorroutine;

                InteractOnDialog = DialogPostWrite;
            }

            void SkipDialog() {
                Debug.Log("Dialog Skipped!");
                textElement.text = d.text;
                InteractOnDialog = DialogPostWrite;

                StopCoroutine(currentCorrutine);
                StopCoroutine(dialogWrittingCorroutine);

                currentCorrutine = StartCoroutine(corrutineEnd());

                IEnumerator corrutineEnd() {
                    //Show next dialog icon
                    yield return null;
                }
            }


            void DialogPostWrite() {
                if(currentCorrutine != null)
                StopCoroutine(currentCorrutine);
                currentCorrutine = StartCoroutine(corrutineEnd());

                IEnumerator corrutineEnd() {
                    textElement.text = d.text;
                    yield return new WaitForSeconds(0.25f);

                    if(d.hideOnDialogEnds) {
                        dialogBox.style.display = DisplayStyle.None;
                    }

                    currentCorrutine = null;
                    InteractOnDialog = null;
                    OnDialogEnds?.Invoke();
                }
            }
        }


        private IEnumerator WriteSpeechInBox(Dialog d) {
            d.textSpeed = Mathf.Clamp(d.textSpeed, 1, 9999);
            float lastSpeakSound = Time.time;
            float speakDuration = 0;

            for(int i = 0; i < d.text.Length; i++) {
                textElement.text += d.text[i];
                yield return new WaitForSeconds(0.1f / d.textSpeed);

                Dialog.charData characterSpeaking = d.characterSpeaking;

                if(Time.time - lastSpeakSound > speakDuration && characterSpeaking != null && characterSpeaking.character != null && characterSpeaking.character.speechSounds.Length > 0) {
                    source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);

                    int soundIndex = UnityEngine.Random.Range(0, characterSpeaking.character.speechSounds.Length);
                    source.PlayOneShot(characterSpeaking.character.speechSounds[soundIndex]);

                    lastSpeakSound = Time.time;
                    speakDuration = characterSpeaking.character.speechSounds[soundIndex].length;
                }

                if(d.text[i] == '.' || d.text[i] == ' ') {
                    //source.Stop();
                    lastSpeakSound = Time.time;
                    speakDuration = 0.2f;
                }
            }

            dialogWrittingCorroutine = null;
        }

        #endregion

        #region Question

        void QuestionCorrutine(Question q) {
            if(ChangeScene.isSceneTransitioning) return;

            if(currentCorrutine != null) StopCoroutine(currentCorrutine);
            currentCorrutine = StartCoroutine(corrutine());

            IEnumerator corrutine() {
                yield return null;
                Dialog question = q.question;

                //Show Question Dialog in UI
                dialogBox.style.display = DisplayStyle.Flex;

                DisplayActiveCharacters(q.question);

                textElement.text = "";

                //Prepare events
                OnDialogStars?.Invoke();
                InteractOnDialog = SkipDialog;

                //Write question text
                dialogWrittingCorroutine = StartCoroutine(WriteSpeechInBox(question));
                yield return dialogWrittingCorroutine;

                yield return new WaitForSeconds(0.25f);
                InteractOnDialog = ShowResponses;
            }

            void SkipDialog() {
                Debug.Log("Dialog Skipped!");
                textElement.text = q.question.text;
                InteractOnDialog = ShowResponses;

                StopCoroutine(currentCorrutine);
                StopCoroutine(dialogWrittingCorroutine);
                currentCorrutine = StartCoroutine(corrutineEnd());

                IEnumerator corrutineEnd() {
                    //Show next dialog icon
                    yield return null;
                }
            }

            void ShowResponses() {
                //Show responses

                dialogBox.style.display = DisplayStyle.None;
                responsesBox.style.display = DisplayStyle.Flex;
                for(int i = 0; i < q.responses.Length; i++) {
                    Responses[i].style.display = DisplayStyle.None;

                    if(q.responses[i] != "") {
                        Responses[i].style.display = DisplayStyle.Flex;
                        Responses[i].text = q.responses[i];
                    }
                }

                InteractOnDialog = null;
            }
        }

        /// <summary>
        /// Response button callback
        /// </summary>
        /// <param name="ev"></param>
        void SelectResponse(ClickEvent ev) {
            byte response = 0;
            switch(((VisualElement)ev.currentTarget).name) {
                case "RightResponse":
                    response = 1;
                    break;

                case "UpperResponse":
                    response = 2;
                    break;

                case "DownResponse":
                    response = 3;
                    break;

                default:
                    break;
            }

            ProcessResponse(response);
        }

        /// <summary>
        /// End Question Dialog
        /// </summary>
        /// <param name="responseIndex"></param>
        void ProcessResponse(byte responseIndex) {
            OnQuestionEnds?.Invoke(responseIndex);

            dialogBox.style.display = DisplayStyle.None;
            responsesBox.style.display = DisplayStyle.None;

            currentCorrutine = null;
        }


        #endregion

        #region General Utilities
        private void DisplayActiveCharacters(Dialog d) {
            if(d.characterSpeaking == null || string.IsNullOrEmpty(d.characterSpeaking.expresionKey)) { //Character data
                UI.rootVisualElement.Q("RightActor").style.display = DisplayStyle.None; 
            } else {
                VisualElement actor = UI.rootVisualElement.Q("RightActor");
                actor.style.display = DisplayStyle.Flex;

                ActorName.text = d.characterSpeaking.character.name;

                Sprite sprite = d.characterSpeaking.getExpresionSprite();
                var expresion = actor.Q("Expresion");

                expresion.style.backgroundImage = new(sprite);
            }
        }

        private bool isDialogRunning() { return currentCorrutine != null; }

        public UIDocument getUIDocument() { return UI; }
        #endregion
    }

    public class DialogDataStruct { }

    [System.Serializable]
    public class Dialog : DialogDataStruct {

        public PlayableAsset introAnim;
        [Multiline]public string text = "TeST";
        public charData characterSpeaking;

        [Range(1, 5)]public float textSpeed = 2;
        public bool hideOnDialogEnds = true;
        public Dialog(string _text, charData _characterSpeakingData = null, float _textSpeed = 1) { 
            text = _text;
            characterSpeaking = _characterSpeakingData;

            hideOnDialogEnds = true;

            if(_textSpeed == 0) _textSpeed = 1;
            textSpeed = _textSpeed;
        }

        [System.Serializable]
        public class charData {
            public string expresionKey;
            public CharacterDialogData character;
        }
    }

    [System.Serializable]
    public class Question : DialogDataStruct {

        public Dialog question;
        public string[] responses = new string[4];
    }
}
