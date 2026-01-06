using UnityEngine;

public class MagnetManager : MonoBehaviour
{
    public float draindistance = 5f;//この距離まで近づいたら実行

    [SerializeField] private float Mp = 0;

    private MagnetManager magnetmanager;

    public void GetMp()
    {
        Mp += 1;
    }

    public void GetMagnet()
    {
        draindistance = 100f;
    }
}
