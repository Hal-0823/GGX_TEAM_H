using UnityEngine;

public class FoundPlayerUI : MonoBehaviour
{
    float startTime = 0f;
    float duration = 10f;

    bool isFoundPlayer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime > duration)
        {
            gameObject.SetActive(false);
        }
    }

    //�v���C���[���������Ƃ��̏���.
    public void FoundPlayer(GameObject person)
    {
        if (!isFoundPlayer)
        {   
            //�A�N�e�B�u�ɂ���.
            gameObject.SetActive(true);
            startTime = Time.time;

            Vector3 personPos = person.transform.position;
            personPos.y = 4f;
            gameObject.transform.position = personPos;

            isFoundPlayer = true;
        }
    }
}
