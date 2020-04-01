using System.Collections;
using UnityEngine;

public static class TransformExtension
{

    public static IEnumerator Move(this Transform t, Vector3 target, float duration)
    {
        Vector3 diffVector = target - t.position;
        float diffLength = diffVector.magnitude;
        diffVector.Normalize();

        float counter = 0;
        while (counter < duration)
        {
            float movAmount = (Time.deltaTime * diffLength) / duration;
            t.position += diffVector * movAmount;
            counter += Time.deltaTime;
            yield return null;
        }

        t.position = target;
    }

    public static IEnumerator Scale(this Transform t, Vector3 target, float duration)
    {
        Vector3 diffVector = (target - t.localScale);
        float diffLength = diffVector.magnitude;
        diffVector.Normalize();
        float counter = 0;
        while (counter < duration)
        {
            float moveAmount = (Time.deltaTime * diffLength) / duration;
            t.localScale += diffVector * moveAmount;
            counter += Time.deltaTime;
            yield return null;
        }

        t.localScale = target;
    }

    public static IEnumerator Blink(this Transform t, float duration) {
        SpriteRenderer spriteRenderer = t.GetComponent<SpriteRenderer>();
        int count = 3;
        while(count >= 0)
        {
            count--;
            for (float i = Constants.BlinkDuration; i >= 0.3f; i -= Time.deltaTime)
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, i);
                yield return null;
            }

            for (float i = 0.3f; i <= Constants.BlinkDuration; i += Time.deltaTime)
            {
                if (spriteRenderer != null)
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, i);
                yield return null;
            }
            yield return null;
        }
        yield return null;
    }

    public static void SetAlpha(this Transform t, float alphaValue)
    {
        SpriteRenderer spriteRenderer = t.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alphaValue);
        }
    }

}
