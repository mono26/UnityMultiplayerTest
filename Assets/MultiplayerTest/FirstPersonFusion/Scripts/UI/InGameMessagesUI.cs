using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// This class is responsible for displaying the in-game messages.
/// </summary>
public class InGameMessagesUI : MonoBehaviour
{
    public TextMeshProUGUI[] inGameMessagesText;

    Queue messageQueue = new Queue();

    /// <summary>
    /// This method is called when a new message is received and needed to display.
    /// </summary>
    /// <param name="message">The message received.</param>
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
