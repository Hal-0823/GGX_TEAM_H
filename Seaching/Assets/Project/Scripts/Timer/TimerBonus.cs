using UnityEngine;

public class TimeBonusItem : MonoBehaviour
{
    public float bonusTime = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameTimer timer = FindObjectOfType<GameTimer>();
            if (timer != null)
            {
                timer.AddTime(bonusTime);
            }

            Destroy(gameObject); // ÉAÉCÉeÉÄÇè¡Ç∑
        }
    }
}
