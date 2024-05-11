using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        instance = this;
    }

    public AudioClip MenuBGM;
    public AudioClip Level1BGM;
    public AudioClip Level2BGM;
    public AudioClip Level3BGM;
    public AudioClip Level4BGM;
    public AudioSource UI;
    public AudioSource BGMsource;

    public void UIClick()
    {
        UI.Play();
    }

    public void SwitchBGM(int levelCode)
    {
        switch (levelCode)
        {
            case 0:
                BGMsource.clip = Level1BGM;
                break;
            case 1:
                BGMsource.clip = Level2BGM;
                break;
            case 2:
                BGMsource.clip = Level3BGM;
                break;
            case 3:
                BGMsource.clip = Level4BGM;
                break;
            case -1:
                BGMsource.clip = MenuBGM;
                break;
            default:
                BGMsource.clip = Level4BGM;
                break;
        }
        BGMsource.Play();
    }
}
