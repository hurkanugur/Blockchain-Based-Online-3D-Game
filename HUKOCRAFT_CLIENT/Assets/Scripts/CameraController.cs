using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Camera mainPlayerCamera;
    public PlayerManager player;
    public RawImage TargetImage;
    public Transform shootingPlace;
    public float mouseSensitivity = 80f, xRotation = 0f, yRotation = 0f, mouseX, mouseY;
    public static bool Zoom = false;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Debug.Log("[Error]: CameraController Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void Start()
    {
        Cursor.visible = false;
        Zoom = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        //CURSOR VISIBILITY CHANGE
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (Inventory.isTradeMenuActivated == true && Zoom == true)
        {
            Zoom = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //KAMERA OYUNCUYA DISARIDAN BAKAR
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            yRotation += mouseX;
            xRotation = Mathf.Clamp(xRotation, -90f, 0f); //ASAGI YUKARI BAKMA ACISI KISITLANDI

            player.transform.Rotate(Vector3.up * mouseX);
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);            //(Uzaklik_x, Uzaklik_y, Uzaklik_z)//
            transform.position = player.transform.position + (transform.rotation * new Vector3(0.0f, 8f, -10f));

            if (transform.position.y < player.transform.position.y)
                transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
            transform.LookAt(player.transform);

            if (TargetImage.enabled == true) //---> EKRANIN ORTASINDAKI NISAN ALMA KARESI GORUNMEZ OLUR
                TargetImage.enabled = false;
        }

        //IF THE CURSOR IS NOT VISIBLE (LOCKED), THEN CAMERA FOLLOWS THE DIRECTION OF THE CURSOR
        if (Cursor.visible == false && GameChat.isChatActivated == false
            && ETHAccount.isSendEtherAmountFieldActivated == false && Inventory.isTradeMenuActivated == false)
        {
            //ZOOMING
            if (Input.GetKeyDown(KeyCode.Z))
            {

                if (Zoom == false)
                {
                    Zoom = true;
                    TargetImage.enabled = true;
                }
                else
                {
                    Zoom = false;
                    TargetImage.enabled = false;
                }
            }


            if (Zoom == true) //IF THE CAMERA IS THE PLAYER'S ITSELF, THE PLAYER CAN ATTACK IN 3D
                shootingPlace.rotation = this.transform.rotation;
            else //IF THE CAMERA LOOKS AT THE PLAYER FROM THE OUTSIDE, PLAYER CAN ATTACK IN 2D
                shootingPlace.rotation = player.transform.rotation;


            if (Zoom == true)
            {
                //KAMERA OYUNCUNUN KENDISI OLUR
                this.transform.position = player.transform.position + (transform.rotation * new Vector3(0, 0.8f, 0.0f));

                mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                xRotation -= mouseY;
                yRotation += mouseX;
                xRotation = Mathf.Clamp(xRotation, -30f, 30f); //ASAGI YUKARI BAKMA ACISI KISITLANDI

                player.transform.Rotate(Vector3.up * mouseX);
                transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

                if (TargetImage.enabled == false) //---> EKRANIN ORTASINDA NISAN ALMA KARESI BELIRIR
                    TargetImage.enabled = true;
            }
            if (Zoom == false)
            {
                //KAMERA OYUNCUYA DISARIDAN BAKAR
                mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                xRotation -= mouseY;
                yRotation += mouseX;
                xRotation = Mathf.Clamp(xRotation, -90f, 0f); //ASAGI YUKARI BAKMA ACISI KISITLANDI

                player.transform.Rotate(Vector3.up * mouseX);
                transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);            //(Uzaklik_x, Uzaklik_y, Uzaklik_z)//
                transform.position = player.transform.position + (transform.rotation * new Vector3(0.0f, 8f, -10f));

                if (transform.position.y < player.transform.position.y)
                    transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
                transform.LookAt(player.transform);

                if (TargetImage.enabled == true) //---> EKRANIN ORTASINDAKI NISAN ALMA KARESI GORUNMEZ OLUR
                    TargetImage.enabled = false;
            }
        }

    }
}