using Cinematics;
using UnityEngine;

public static class Utilities
{
    public static Sprite getExpresionSprite(this CharacterDialogData data, string key) {
        return data.expresionHashMap[key];
    }

    public static Sprite getExpresionSprite(this Dialog.charData data) {
        return getExpresionSprite(data.character, data.expresionKey);
    }

    public static void LookAt2D(this UnityEngine.Transform t, Vector3 position, float offset = 0) {
        Vector3 diff = position - t.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        t.rotation = Quaternion.Euler(0f, 0f, rot_z + offset);
    }

    public static Vector2 ClampVector(this Vector2 a, Vector2 min, Vector2 max) { return new(Mathf.Clamp(a.x, min.x, max.x), Mathf.Clamp(a.y, min.y, max.y)); }
    public static Vector3 ClampEulers(this Vector3 a) { return new Vector3(a.x > 180 ? a.x - 360 : a.x, a.y > 180 ? a.y - 360 : a.y, a.z > 180 ? a.z - 360 : a.z); }
}
