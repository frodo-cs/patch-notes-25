using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class MouseReaction : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        /// <summary>
        /// Dan click al objeto, inicia el click
        /// </summary>
        public virtual void OnInteractStart() {
            Debug.Log("Interact obj starts");
        }

        /// <summary>
        /// Dan click al objeto, termina el click
        /// </summary>
        public virtual void OnInteractEnd() {
            Debug.Log("Interact obj ends");
        }
    }

}