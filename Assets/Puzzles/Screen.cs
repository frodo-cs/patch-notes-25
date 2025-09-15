using UnityEngine;

namespace Player.Puzzles
{
    public class Screen : MonoBehaviour
    {
        [SerializeField] private Simon simon;
        [SerializeField] private int screenId;
        [SerializeField] private Color color;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            spriteRenderer.color = color;
            TurnOff();
        }

        public void SetFailed()
        {
            spriteRenderer.color = Color.red;
        }

        public void SetSuccess()
        {
            spriteRenderer.color = Color.green;
        }

        public void TurnOff()
        {
            spriteRenderer.color = color * 0.3f;
        }

        public void TurnOn()
        {
            spriteRenderer.color = color;
        }

        private void OnMouseDown()
        {
            simon.PlayLight(screenId);
        }
    }
}