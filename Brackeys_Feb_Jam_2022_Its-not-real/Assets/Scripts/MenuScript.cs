using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField]
    RectTransform pointer;
    [SerializeField]
    RectTransform menuHolder;
    [SerializeField]
    Image startCover;
    Vector2 menuPos1 = new Vector2(-197, 35);
    Vector2 menuPos2 = new Vector2(-102, -93.5f);
    bool pointerOnOne = true;
    bool firstMenu = true;
    bool starting = false;
    

    [SerializeField]
    private Slider volumeKnob;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pointerOnOne)
        {
            pointer.anchoredPosition = Vector2.Lerp(pointer.anchoredPosition, menuPos1, Time.deltaTime * 5);
        }
        else
        {
            pointer.anchoredPosition = Vector2.Lerp(pointer.anchoredPosition, menuPos2, Time.deltaTime * 5);
        }
        if (firstMenu)
        {
            menuHolder.anchoredPosition = Vector2.Lerp(menuHolder.anchoredPosition, Vector2.zero, Time.deltaTime * 3);
        }
        else
        {
            menuHolder.anchoredPosition = Vector2.Lerp(menuHolder.anchoredPosition, new Vector2(-1280,0), Time.deltaTime * 3);
        }
        if (starting)
        {
            Color newColor = startCover.color;
            newColor.a += Time.deltaTime * 1;
            startCover.color = newColor;
            if (startCover.color.a >= 1)
            {
                SceneManager.LoadScene(1);
                Debug.Log("bababooey");
                starting = false;
            }
        }
        AudioListener.volume = volumeKnob.value;
    }

    public void SwitchPointer(int itemNumber)
    {
        if(itemNumber == 1)
        {
            pointerOnOne = true;
        }
        else
        {
            pointerOnOne = false;
        }
    }
    
    public void SwitchMenus()
    {
        firstMenu = !firstMenu;
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }

    public void StartGame()
    {
        starting = true;
    }
}
