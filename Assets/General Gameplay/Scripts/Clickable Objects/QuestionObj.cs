using UnityEngine;
using Cinematics;
using Player;
using UnityEngine.Events;
using System.Collections;

namespace Player.Gameplay {
    public class QuestionObj : MouseReaction {
        [SerializeField] Question question;

        [SerializeField] UnityEvent[] responsesCases = new UnityEvent[4];

        public override void OnInteractStart() {
            var a = DialogBoxController.IsDialogRunning?.Invoke();
            if(a != null && !a.Value) {
                DialogBoxController.PlayQuestion?.Invoke(question);
                DialogBoxController.OnQuestionEnds = OnQuestionResponsed;
            }
        }

        void OnQuestionResponsed(byte r) {
            StartCoroutine(corrutine());

            IEnumerator corrutine() {
                yield return new WaitForSeconds(0.1f);

                responsesCases[r]?.Invoke();
                DialogBoxController.OnQuestionEnds = null;
            }
        }
    }

}