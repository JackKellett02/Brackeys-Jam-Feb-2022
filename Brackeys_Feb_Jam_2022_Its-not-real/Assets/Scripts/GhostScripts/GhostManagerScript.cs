using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManagerScript : MonoBehaviour
{
    #region Variables to assign via the unity inspector (SerializeFields).
    [SerializeField]
    private GameObject ghostModeUiTint = null;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float ghostModeTimeScale = 0.0f;

    [SerializeField]
    [Range(2, 20)]
    private int trampolineResolution = 5;
    #endregion

    #region Private Variable Declarations.
    private static bool ghostModeActive;
    private static float static_timeScale;


    private Vector3 worldPos1 = Vector3.zero;
    private Vector3 worldPos2 = Vector3.zero;
    #endregion

    #region Private Functions.
    // Start is called before the first frame update
    void Start()
    {
        ghostModeActive = false;
        static_timeScale = ghostModeTimeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!ghostModeActive)
            {
                ActivateGhostMode();
            }
            else
            {
                DeactivateGhostMode();
            }
        }

        //Check if the game is in "Ghost Mode"
        if (ghostModeActive)
        {
            //Activate the tint.
            if (ghostModeUiTint)
            {
                ghostModeUiTint.SetActive(true);
            }

            //Let the player draw trampolines.
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Mouse Down.");
                //Reset the world positions for the new trampoline.
                worldPos1 = Vector3.zero;
                worldPos2 = Vector3.zero;

                //Get the first position when the user presses the mouse down.
                worldPos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Mouse Up.");
                //Get the second position for the trampoline.
                worldPos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //Convert the points to vector2 and pass them to the spawn trampoline function.
                Vector2 v2Point1 = new Vector2(worldPos1.x, worldPos1.y);
                Vector2 v2Point2 = new Vector2(worldPos2.x, worldPos2.y);
                TrampolineManagerScript.SpawnTrampoline(v2Point1, v2Point2, trampolineResolution);
            }
        }
        else
        {
            //Deactivate the tint.
            if (ghostModeUiTint)
            {
                ghostModeUiTint.SetActive(false);
            }
        }
    }
    #endregion

    #region Public Access Functions (Getters and Setters).
    /// <summary>
    /// Freezes time and let's the ghost be controlled.
    /// </summary>
    public static void ActivateGhostMode()
    {
        ghostModeActive = true;
        Time.timeScale = static_timeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    /// <summary>
    /// Unfreezes time and stops the ghost from being controlled.
    /// </summary>
    public static void DeactivateGhostMode()
    {
        ghostModeActive = false;
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
    }
    #endregion
}
