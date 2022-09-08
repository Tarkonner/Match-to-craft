using UnityEngine;

public class Mouse : MonoBehaviour
{
    public Mouse Instance { get; private set; }

    //Holding
    public DotTable holdingTable;

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
    }

    void Update()
    {
        if (MenuManager.Instance.ActiveMenu)
            return;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        //Make table follow mouse
        if (holdingTable != null)
        {
            Vector2 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            holdingTable.transform.position = camPos;
            holdingTable.HoldingUpdate();
        }

        //Left mouse button
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hit.collider == null)
                return;

            if (holdingTable == null)
            {
                if (hit.collider.TryGetComponent(out DotTable table))
                {
                    holdingTable = table;
                    table.PickupTable();
                    table.HighlightTable();
                    PlayAudio(pickupSound);
                }
                else if(hit.collider.TryGetComponent(out Board board))
                {
                    DotTable result = board.TakeFromBoard(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (result != null)
                    {
                        holdingTable = result;
                        result.HighlightTable();
                        PlayAudio(pickupSound);
                    }
                }

            }
            else
            {               
                if (hit.collider.TryGetComponent(out Board board))
                {
                    bool result = board.PlaceDots(holdingTable);

                    if (result)
                    {
                        holdingTable.DropTable();
                        holdingTable = null;
                        PlayAudio(placeSounds);
                    }
                    else
                        PlayAudio(failPlacement);
                }
            }
        }

        //Right mouse button
        if (Input.GetKeyDown(KeyCode.Mouse1) && holdingTable != null)
        {
            holdingTable.RotateTable();
            PlayAudio(rotateSounds);
        }

        //Middle mouse buttom
        if (Input.GetKeyDown(KeyCode.Mouse2) && holdingTable != null)
        {
            holdingTable.ResetTable();
            holdingTable.DropTable();
            holdingTable = null;
            PlayAudio(dropSound);
        }
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
