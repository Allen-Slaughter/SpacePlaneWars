using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Canvas hUDCanvas;
    [SerializeField] Canvas menusCanvas;

    void OnEnable()
    {
        playerInput.onPause += Pause;
        playerInput.onUnPause += UnPause;
    }

    void OnDisable()
    {
        playerInput.onPause -= Pause;
        playerInput.onUnPause -= UnPause;
    }

    void Pause()
    {
        Time.timeScale = 0f;
        hUDCanvas.enabled = false;
        menusCanvas.enabled = true;
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
    }

    void UnPause()
    {
        Time.timeScale = 1f;
        hUDCanvas.enabled = true;
        menusCanvas.enabled = false;
        playerInput.EnableGamePlayInput();
        playerInput.SwitchToFixedUpdateMode();
    }
}
