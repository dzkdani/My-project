﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void Quit() {
        SoundController.Instance.SelectSFX("button");
        Application.Quit();
    }
}
