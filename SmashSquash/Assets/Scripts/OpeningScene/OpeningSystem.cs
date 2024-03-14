using System.Collections;
using System.Collections.Generic;
using Flower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 開頭故事系統
/// </summary>
public class OpeningSystem : MonoBehaviour
{
    private StorySystem storySystem;

    void Start()
    {
        storySystem = StorySystem.instance;
        storySystem.LoadingStory("OpeningScene/openingIntro");
    }
}
