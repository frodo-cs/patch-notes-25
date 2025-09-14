using Player.Puzzles;
using UnityEngine;

public class SimonButton : MonoBehaviour
{
    [SerializeField] private Simon simon;
    private bool active = true;
    [HideInInspector] public bool Active
    {
        get { return active; }
        set { active = value; }
    }


    private void OnMouseDown()
    {
        if (Active)
            simon.Play();
    }
}
