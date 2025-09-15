using Cinematics;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public static ChangeScene instance;

    [SerializeField] Material transitionMaterial;

    Transform sceneRef;
    Coroutine currentCorrutine;

    public delegate bool loadScene(int indexBuild);
    public static loadScene LoadScene;

    public static bool isSceneTransitioning;
    [SerializeField] float transitionSpeed;

    private void Awake() {
        LoadScene = OnLoadScene;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);

            OnChangeScene();

        } else { Destroy(gameObject); }
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool OnLoadScene(int indexBuild) {

        var isDialoging = DialogBoxController.IsDialogRunning?.Invoke();

        if(currentCorrutine != null || (isDialoging != null && isDialoging.Value)) return false;
        currentCorrutine = instance.StartCoroutine(corrutine());

        return true;

        IEnumerator corrutine() {
            transitionMaterial.SetFloat("_time", 0);
            isSceneTransitioning = true;

            float t = 0;
            while(t <= 1) {
                transitionMaterial.SetFloat("_time", t);

                t += Time.deltaTime * transitionSpeed;
                yield return null;
            }

            SceneManager.LoadScene(indexBuild);
            currentCorrutine = null;

            yield return new WaitUntil(() => sceneRef == null);

            OnChangeScene();
        }
    }

    void OnChangeScene() {
        if(currentCorrutine != null) return;
        currentCorrutine = instance.StartCoroutine(corrutine());

        IEnumerator corrutine() {
            transitionMaterial.SetFloat("_time", 1);

            float t = 0;
            while(t <= 1) {
                transitionMaterial.SetFloat("_time", 1f - t);

                t += Time.deltaTime * transitionSpeed;
                yield return null;
            }

            sceneRef = new GameObject().transform;
            isSceneTransitioning = false;
            currentCorrutine = null;
        }
    }

    private void OnDestroy() {
        if(instance == this) instance = null;
    }
}
