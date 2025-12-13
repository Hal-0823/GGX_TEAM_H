using UnityEngine;

public class DebrisShrinker : MonoBehaviour
{
    private float shrinkSpeed = 2.0f; // 縮む速さ

    void Update()
    {
        // 毎フレーム小さくする
        transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;

        // 0以下になったら消滅
        if (transform.localScale.x <= 0f)
        {
            Destroy(gameObject);
        }
    }
}