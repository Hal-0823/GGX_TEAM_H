using UnityEngine;

public class Magnetitem : MonoBehaviour
{
    [SerializeField] private Magnet magnet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        void OnTriggerEnter(Collider other)//Magnetのアイテムを入手した時の判定
    {
        Debug.Log("Magnetitemをとったよ");
        magnet.GetMagnet();
        Destroy(gameObject);
    }


}
