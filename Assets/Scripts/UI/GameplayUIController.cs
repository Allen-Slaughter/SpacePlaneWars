using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    [Header("==== PLAYER INPUT ====")]
    [SerializeField] PlayerInput playerInput;

    [Header("==== AUDIO DATA ====")]
    [SerializeField] AudioData pauseSFX;
    [SerializeField] AudioData unpauseSFX;

    [Header("==== CANVAS ====")]
    [SerializeField] Canvas hUDCanvas;
    [SerializeField] Canvas menusCanvas;

    [Header("==== PLAYER INPUT ====")]
    [SerializeField] Button resumeButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button mainMenuButton;

    int buttonPressedParameterID = Animator.StringToHash("Pressed");

    void OnEnable()
    {
        playerInput.onPause += Pause;
        playerInput.onUnPause += UnPause;

        ButtonPressedBehaviour.buttonFunctionTable.Add(resumeButton.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(optionsButton.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(mainMenuButton.gameObject.name, OnMainMenuButtonClick);
    }

    void OnDisable()
    {
        playerInput.onPause -= Pause;
        playerInput.onUnPause -= UnPause;

        ButtonPressedBehaviour.buttonFunctionTable.Clear();
    }

    void Pause()
    {
        hUDCanvas.enabled = false;
        menusCanvas.enabled = true;
        GameManager.GameState = GameState.Paused;
        TimeController.Instance.Pause();
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButton);
        AudioManager.Instance.PlaySFX(pauseSFX);
    }

    void UnPause()
    {
        resumeButton.Select();
        resumeButton.animator.SetTrigger(buttonPressedParameterID);
        //OnResumeButtonClick();
        AudioManager.Instance.PlaySFX(unpauseSFX);
    }

    void OnResumeButtonClick()
    {
        hUDCanvas.enabled = true;
        menusCanvas.enabled = false;
        GameManager.GameState = GameState.Playing;
        TimeController.Instance.UnPause();
        playerInput.EnableGamePlayInput();
        playerInput.SwitchToFixedUpdateMode();
    }

    void OnOptionsButtonClick()
    {
        //TODO
        UIInput.Instance.SelectUI(optionsButton);
        playerInput.EnablePauseMenuInput();
    }

    void OnMainMenuButtonClick()
    {
        menusCanvas.enabled = false;
        //Load Main Menu Scene
        SceneLoader.Instance.LoadMainMenuScene();
    }
}
