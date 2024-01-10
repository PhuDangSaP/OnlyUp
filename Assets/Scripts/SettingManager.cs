using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public void Back()
    {
        this.gameObject.SetActive(false);
    }
    public void SetVolumn(float volumn)
    {
        SoundManager.volumn = volumn;
    }
}
