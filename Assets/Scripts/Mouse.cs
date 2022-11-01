using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mouse : MonoBehaviour
{
    [Header("Mouse UI")]
    [SerializeField] MouseUI rightMouseButton;
    [SerializeField] MouseUI middleMouseButton;
    [SerializeField] MouseUI leftMouseButton;
    private Action acPickUp;
    public Action acDrop { get; private set; }

    public static Mouse Instance { get; private set; }

    //Holding
    [SerializeField] private DotTable holdingTable;

    private AudioSource audioSource;

    [Header("Sound")]
    [SerializeField] float minPitch = .9f;
    [SerializeField] float maxPitch = 1;
    [SerializeField] AudioClip[] rotateSounds;
    [SerializeField] AudioClip failPlacement;
    [SerializeField] AudioClip[] placeSounds;
    [SerializeField] AudioClip pickupSound;
    [SerializeField] AudioClip dropSound;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();

        acPickUp += UIPickup;
        acDrop += UIDrop;
        acDrop?.Invoke();
    }

    void Update()
    {
        if (MenuManager.Instance.ActiveMenu)
        {
            //Turnoff UI
            rightMouseButton.TurnOff();
            middleMouseButton.TurnOff();
            leftMouseButton.TurnOff();

            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        //Make table follow mouse
        if (holdingTable != null)
        {
            Vector2 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            holdingTable.transform.position = camPos;
            holdingTable.HoldingUpdate();
        }

        //Update left mouse UI sprite to what the button do
        if (hit.collider != null && holdingTable != null)
            leftMouseButton.ChangeText("Place");
        else if (hit.collider != null && hit.collider.TryGetComponent(out DotTable table))
            leftMouseButton.ChangeText("Pickup");
        else if (hit.collider == null && holdingTable != null)
            leftMouseButton.ChangeText("Drop");

        //Left mouse button
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hit.collider == null)
            {
                //Return holdt table
                if(holdingTable)
                    ReturnTable();

                return;
            }
                        
            //Not holding a piece
            if (holdingTable == null)
            {
                //Pickup piece not in grid
                if (hit.collider.TryGetComponent(out DotTable table))
                {
                    holdingTable = table;
                    table.PickupTable();
                    table.HighlightTable();
                    PlayAudio(pickupSound);

                    acPickUp?.Invoke();
                }
                //Pickup piece in grid
                else if (hit.collider.TryGetComponent(out GameGrid grid))
                {
                    DotTable result = grid.TakeFromBoard(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (result != null)
                    {
                        holdingTable = result;
                        result.HighlightTable();
                        PlayAudio(pickupSound);

                        acPickUp?.Invoke();
                    }
                }
            }
            else
            {                  
                //Holding a piece
                if (hit.collider.TryGetComponent(out GameGrid grid))
                {
                    bool result = grid.PlaceDots(holdingTable);

                    if (result)
                    {
                        holdingTable.DropTable();
                        holdingTable = null;
                        PlayAudio(placeSounds);

                        //Update UI
                        acDrop?.Invoke();
                    }
                    else
                        PlayAudio(failPlacement);
                }
            }
        }

        //Right mouse button
        if (Input.GetKeyDown(KeyCode.Mouse1) && holdingTable != null)
        {
            //Rotate holding piece
            if(!holdingTable.CanNotRotate)
            {
                holdingTable.RotateTable();
                PlayAudio(rotateSounds);
            }
        }

        //Middle mouse buttom
        if (Input.GetKeyDown(KeyCode.Mouse2) && holdingTable != null)
            ReturnTable();
    }

    private void ReturnTable()
    {
        //UI
        acDrop?.Invoke();

        //Reset table
        holdingTable.ResetTable();
        holdingTable.DropTable();
        holdingTable = null;
        PlayAudio(dropSound);
    }

    private void UIPickup()
    {
        if(holdingTable != null && !holdingTable.CanNotRotate)
        {
            rightMouseButton.TurnOn();
            rightMouseButton.ChangeText("Rotate");
        }
        else if(holdingTable != null && holdingTable.CanNotRotate)
        {
            rightMouseButton.TurnOff();
        }

        middleMouseButton.TurnOn();
        middleMouseButton.ChangeText("Return piece");

        //leftMouseButton.ChangeText("Drop");
    }
    private void UIDrop()
    {
        rightMouseButton.TurnOff();
        middleMouseButton.TurnOff();

        //leftMouseButton.ChangeText("Pickup");
    }

    public void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    public void PlayAudio(AudioClip[] clips)
    {
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }
}
