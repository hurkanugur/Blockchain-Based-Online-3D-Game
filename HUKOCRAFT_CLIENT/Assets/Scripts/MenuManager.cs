using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public Camera mainCamera;
    public GameObject startMenu;
    public GameObject gameScreen;
    public InputField usernameField;
    public InputField serverIPAddress;
    public InputField serverPort;
    public Image fadingEffect;

    public Text announcement;
    public Text qualityButtonText;
    public Text soundButtonText;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void Start()
    {
        //PROVIDES LOWER CPU 
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }

    private static bool isHighQualityActivated = false;
    public static bool isSoundActivated = true;
    public void ChangeQualityButtonClicked()
    {
        //IF HIGH IS ALREADY SET, MAKE IT LOW QUALITY
        if (isHighQualityActivated == true)
        {
            //PROVIDES LOWER CPU 
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            isHighQualityActivated = false;
            qualityButtonText.text = "SD";
            CameraController.instance.mouseSensitivity = 80;
            GameChat.instance.Announcement("Standart quality is activated");
        }
        else
        {
            //PROVIDES HIGH QUALITY 
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
            isHighQualityActivated = true;
            qualityButtonText.text = "HD";
            CameraController.instance.mouseSensitivity = 150;
            GameChat.instance.Announcement("High quality is activated");
        }

    }

    public void ChangeSoundSettingsButtonClicked()
    {
        //IF HIGH IS ALREADY SET, MAKE IT LOW QUALITY
        if (isSoundActivated == true)
        {
            isSoundActivated = false;
            soundButtonText.text = "♯";
            GameChat.instance.Announcement("Game music is muted");
        }
        else
        {
            isSoundActivated = true;
            soundButtonText.text = "♬";
            GameChat.instance.Announcement("Game music is activated");
        }

    }

    private float cameraRotationCounter = 0.0f;

    public void FixedUpdate()
    {
        if (fadingEffect.canvasRenderer.GetAlpha() == 1.0f)
        {
            fadingEffect.raycastTarget = false; //MAKE SCREEN CLICKABLE (CUZ FADING IMAGE IS ABOVE ALL)
            fadingEffect.CrossFadeAlpha(0, 1, false); //FADING ANIMATION
        }

        if (usernameField.isFocused == true)
        {
            usernameField.placeholder.enabled = false;

            //TURN TURKISH "İ, ı" TO ENGLISH CHARACTER
            if (usernameField.text.Contains("ı"))
                usernameField.text = usernameField.text.Replace("ı", "i");
            if (usernameField.text.Contains("İ"))
                usernameField.text = usernameField.text.Replace("İ", "I");

            foreach (char character in usernameField.text.ToCharArray())
            {
                //ONLY THE NUMBERS AND ENGLISH CHARACTERS ARE ALLOWED
                if (!((character > 47 && character < 58) || (character > 64 && character < 91) || (character > 96 && character < 123)))
                {
                    usernameField.text = usernameField.text.Replace(character, '\0');
                    Announcement("You cannot use special characters");
                }
            }
            usernameField.text = usernameField.text.ToUpper(new System.Globalization.CultureInfo("en-US", false));
        }

        if (serverIPAddress.isFocused == true)
        {
            serverIPAddress.placeholder.enabled = false;
            foreach (char character in serverIPAddress.text.ToCharArray())
            {
                //ONLY THE NUMBERS AND '.' ARE ALLOWED
                if (!((character > 47 && character < 58) || character.Equals('.')))
                {
                    serverIPAddress.text = serverIPAddress.text.Replace(character, '\0');
                    Announcement("You cannot use special characters");
                }
            }
        }

        if (serverPort.isFocused == true)
        {
            serverPort.placeholder.enabled = false;
            foreach (char character in serverPort.text.ToCharArray())
            {
                //ONLY THE NUMBERS ARE ALLOWED
                if (!(character > 47 && character < 58))
                {
                    serverPort.text = serverPort.text.Replace(character, '\0');
                    Announcement("You cannot use special characters");
                }
            }
        }

        if (mainCamera.gameObject.activeSelf == true) //IF THE MENU IS ACTIVATED
        {
            mainCamera.transform.SetPositionAndRotation(new Vector3(0f, 4f, 0f), Quaternion.Euler(mainCamera.transform.rotation.x, cameraRotationCounter, mainCamera.transform.rotation.z));
            cameraRotationCounter += 0.2f;
        }
    }

    public void ExitButtonOnClicked()
    {
        Application.Quit();
    }

    //CLIENT ATTEMPTS TO CONNECT TO THE SERVER
    public void ConnectToServer()
    {
        if (usernameField.text.Trim().Length >= 3 && serverIPAddress.text.Length != 0 && serverPort.text.Length != 0)
        {
            fadingEffect.raycastTarget = true; //MAKE SCREEN UNCLICKABLE (CUZ FADING IMAGE IS ABOVE ALL)

            //IF USERNAME FIELD IS ENABLED, THAT MEANS THE USER WITH CORRESPONDING WALLET IS A NEW PLAYER, SO SAVE HIM ON THE BLOCKCHAIN !
            //IF NOT ENABLED, IT WILL NOTIFY THE PLAYER THAT HE ALREADY EXISTS
            _ = HukocraftSmartContract.instance.HukocraftCharacterInitialization(usernameField.text);

            StartCoroutine(ConnectAnimation());
        }
        else
        {
            if (usernameField.text.Trim().Length == 0)
                Announcement("Username field cannot be empty");
            else if (usernameField.text.Trim().Length < 3)
                Announcement("The username can have min 3, max 8 characters");
            else if (serverIPAddress.text.Trim().Length == 0)
                Announcement("Please enter the Server IP Address");
            else if (serverPort.text.Trim().Length == 0)
                Announcement("Please enter the Server Port Number");
        }
    }

    private IEnumerator ConnectAnimation()
    {
        fadingEffect.CrossFadeAlpha(1, 2, false); //FADING ANIMATION
        yield return new WaitForSeconds(2.0f);

        fadingEffect.gameObject.SetActive(false);
        startMenu.SetActive(false);
        gameScreen.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        try
        {
            //IF SERVER IP ADDRESS AND PORT NUMBER IS ENTERED MANUALLY
            Client.instance.ConnectToServer(serverIPAddress.text, Convert.ToInt32(serverPort.text));
        }
        catch (Exception)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }



    //IT IS FOR HOW MUCH EXP YOU GAINED OR LOST (TOP CENTER TEXT AREA)
    public void Announcement(string _announcementText)
    {
        StopCoroutine(AnnouncementAnimation());
        isAnimationVisible = false;
        A = 0f;
        announcement.color = new Color(0, 0, 0, A);
        announcement.text = _announcementText;
        StartCoroutine(AnnouncementAnimation());
    }

    //IT IS FOR HOW MUCH EXP YOU GAINED OR LOST (TOP CENTER TEXT AREA)
    private bool isAnimationVisible;
    private float A = 0;
    private IEnumerator AnnouncementAnimation()
    {
        yield return new WaitForSeconds(0.001f);

        if (isAnimationVisible == false && A < 255f)
        {
            A += 15;
            announcement.color = new Color(0, 0, 0, A / 255.0f);

            if (A == 255)
            {
                yield return new WaitForSeconds(2f); //TEXT WILL BE SHOWN FOR 5 SECONDS
                isAnimationVisible = true;
            }
            StartCoroutine(AnnouncementAnimation());
        }
        else if (isAnimationVisible == true && A > 0f)
        {
            A -= 15;
            announcement.color = new Color(0, 0, 0, A / 255.0f);
            if (A > 0)
            {
                StartCoroutine(AnnouncementAnimation());
            }
            else
                isAnimationVisible = false;
        }
    }
}
