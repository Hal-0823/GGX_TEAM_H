using UnityEngine;

public class FoundPlayerUI : MonoBehaviour
{
    float startTime = 0f;
    float duration = 5f;

    bool isFoundPlayer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var cam = Camera.main;
        transform.LookAt(cam.transform);
        
        if (Time.time - startTime > duration)
        {
            gameObject.SetActive(false);
        }
    }

    public void FoundPlayer(GameObject person)
    {
        if (!isFoundPlayer)
        {   
            gameObject.SetActive(true);
            startTime = Time.time;

            Vector3 personPos = person.transform.position;
            personPos.y = 4f;
            gameObject.transform.position = personPos;

            isFoundPlayer = true;
        }
    }
}
