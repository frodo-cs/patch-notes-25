using UnityEngine;

namespace Player.UI
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private Texture2D arrowLeft;
        [SerializeField] private Texture2D arrowRight;
        [SerializeField] private Texture2D arrowUp;
        [SerializeField] private Texture2D arrowDown;

        [System.Serializable]
        public struct FieldData
        {
            public enum Alignment { Left, Right, Up, Down }

            public Alignment alignment;
            public int scene;
        }

        [SerializeField] private FieldData[] fields;

        private Camera mainCam;
        private const float screenWidth = 960f;
        private const float screenHeight = 535f;
        private const float inventoryHeight = 125f;
        private const float buttonSize = 50f;
        private const float offset = 40f;

        private void Start()
        {
            mainCam = Camera.main;
            CreateMovementButtons();
        }
        private void CreateMovementButtons()
        {
            foreach (var field in fields)
            {
                GameObject go = new GameObject(field.alignment.ToString());
                go.transform.parent = transform;

                BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;

                Vector2 pixelSize = new Vector2(buttonSize, buttonSize);
                Vector2 pixelPos = Vector2.zero;
                Texture2D icon = null;

                switch (field.alignment)
                {
                    case FieldData.Alignment.Left:
                        pixelPos = new Vector2(offset, inventoryHeight + (screenHeight - inventoryHeight) / 2);
                        icon = arrowLeft;
                        break;

                    case FieldData.Alignment.Right:
                        pixelPos = new Vector2(screenWidth - offset, inventoryHeight + (screenHeight - inventoryHeight) / 2);
                        icon = arrowRight;
                        break;

                    case FieldData.Alignment.Up:
                        pixelPos = new Vector2(screenWidth / 2f, screenHeight - offset);
                        icon = arrowUp;
                        break;

                    case FieldData.Alignment.Down:
                        pixelPos = new Vector2(screenWidth / 2f, inventoryHeight + offset);
                        icon = arrowDown;
                        break;
                }

                if (icon != null)
                {
                    SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                    sr.sprite = Sprite.Create(
                        icon,
                        new Rect(0, 0, icon.width, icon.height),
                        new Vector2(0.5f, 0.5f),
                        175f
                    );
                    sr.sortingOrder = 10;

                    collider.size = sr.sprite.bounds.size;
                }

                go.transform.position = PixelToWorldPos(pixelPos);

                var clickComp = go.AddComponent<MovementClick>();
                clickComp.sceneIndex = field.scene;
            }
        }

        private Vector3 PixelToWorldPos(Vector2 pixelPos)
        {
            float camHeight = 2f * mainCam.orthographicSize;
            float camWidth = camHeight * mainCam.aspect;

            float worldX = (pixelPos.x / screenWidth - 0.5f) * camWidth;
            float worldY = (pixelPos.y / screenHeight - 0.5f) * camHeight;
            return new Vector3(worldX, worldY, 0f);
        }

        private Vector2 PixelToWorldSize(Vector2 pixelSize)
        {
            float camHeight = 2f * mainCam.orthographicSize;
            float camWidth = camHeight * mainCam.aspect;

            float worldWidth = (pixelSize.x / screenWidth) * camWidth;
            float worldHeight = (pixelSize.y / screenHeight) * camHeight;
            return new Vector2(worldWidth, worldHeight);
        }
    }

    public class MovementClick : MonoBehaviour
    {
        public int sceneIndex;

        private void OnMouseDown()
        {
            Debug.Log($"Move to scene: {sceneIndex}");
            var result = ChangeScene.LoadScene?.Invoke(sceneIndex);
            if (result == null || !result.Value)
                OnFail();
        }

        void OnFail()
        {
            Debug.LogWarning("Change Scene Singleton doesn't exist or there is dialog on screen!");

            //Feedback animation
        }
    }
}
