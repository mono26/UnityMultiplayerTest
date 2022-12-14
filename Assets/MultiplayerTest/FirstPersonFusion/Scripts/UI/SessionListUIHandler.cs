using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using System;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public GameObject sessionInfoListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    public void ClearList()
    {
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        statusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo)
    {
        SessionInfoListUI sessionInfoListUI = Instantiate(sessionInfoListPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoListUI>();

        sessionInfoListUI.SetInformation(sessionInfo);

        sessionInfoListUI.OnJoinSession += AddedSessionInfoListUI_OnJoinSession;
    }

    private void AddedSessionInfoListUI_OnJoinSession(SessionInfo obj)
    {
        
    }

    public void OnNoSessionFound()
    {
        statusText.text = "No Session Found";
        statusText.gameObject.SetActive(true);
    }

    public void OnLookingForSessions()
    {
        statusText.text = "Looking for Sessions";
        statusText.gameObject.SetActive(true);
    }
}
