using UnityEngine;

public class Magnetitem : MonoBehaviour
{
    private MagnetManager magnetmanager;
    private Magnet magnet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    magnetmanager = FindObjectOfType<MagnetManager>();
    //シーン内のMagnetを探す
    magnet = GetComponentInParent<Magnet>();
    }

    // Update is called once per frame
    void Update()
    {}
        //Magnetのアイテムを入手した時の判定
    void OnTriggerEnter(Collider other)
    {
    if (other.CompareTag("Player"))
        {
        Debug.Log("Magnetitemをとったよ");
        magnetmanager.GetMagnet();
        Destroy(gameObject);           
        }
    }
}