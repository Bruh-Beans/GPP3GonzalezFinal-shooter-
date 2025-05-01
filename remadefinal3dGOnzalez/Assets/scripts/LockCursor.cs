using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCursor : MonoBehaviour
{
    void Start()
    {
        // Initially unlock the cursor, assuming the game starts at the title screen
        Unlock();
    }

    //when game is being played, the cursor is locked so that the player can focus on the crosshair and not the cursor
    public void Lock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
//this variable unlocks the cursor if either the game hasn't started or the player has died and entered the game over screen
    public void Unlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
