using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Dialog/Speaker")]
public class SpeakerData : ScriptableObject
{
    public Dictionary<string, int> Emotions = new Dictionary<string, int>
    {
        {"happy", 0}, {"sad", 1}, {"angry", 2},
        {"suspicious", 3}, {"smile", 4}, {"understanding", 5},
        {"serious", 6}, {"pleased", 7}, {"arrogant", 8},
        {"awkward", 9}, {"confused", 10}, {"crying", 11},
        {"furious", 12}, {"surprised", 13}, {"shy", 14}
    };

    public string speakerName;
    public Sprite[] NPCPortraits;

    public Sprite GetEmotionPortrait(string emotion)
    {
        return NPCPortraits[Emotions[emotion]];
    }
}
