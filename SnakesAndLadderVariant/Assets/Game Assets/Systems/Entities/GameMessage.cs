using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMessage : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] AnimationCurve popCurve;
    
    TextMeshProUGUI myText;
    float messageTime = 1f;

    public void Initialize(float time)
    {
        messageTime = time;
        myText= GetComponent<TextMeshProUGUI>();    
        StartCoroutine(ZoomChange());
    }
    IEnumerator ZoomChange()
    {
        float messageSize = 0f;

        while (messageSize < 1f)
        {
            messageSize += (speed / messageTime) * Time.fixedDeltaTime;

            this.transform.localScale = popCurve.Evaluate(messageSize) * new Vector3(1, 1, 1);
            myText.alpha = popCurve.Evaluate(messageSize);

            yield return new WaitForFixedUpdate();  
        }

        UIController.instance.showingMessage = false;

        Destroy(this.gameObject);
    }
}

