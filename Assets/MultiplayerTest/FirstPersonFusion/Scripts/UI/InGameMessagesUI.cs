using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameMessagesUI : MonoBehaviour
{
    public TextMeshProUGUI[] inGameMessagesText;

    Queue messageQueue = new Queue();

    void Start()
    {
        
    }

    public void OnGameMessageReceived(string message)
    {
        messageQueue.Enqueue(message);

        if (messageQueue.Count > 3)
            messageQueue.Dequeue();
        
        int queueIndex = 0;

        foreach (string queuedMessage in messageQueue)
        {
            inGameMessagesText[queueIndex].text = queuedMessage;
            queueIndex++;
        }
    }
}
