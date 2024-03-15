using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    private void Awake()
    {
        //EventManager.AddListener("scores_changed", _OnScoresChanged);
    }

    private void _OnScoresChanged()
    {
        Debug.Log("分數改變");
    }
}
