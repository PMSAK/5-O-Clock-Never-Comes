using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Inventory UI")]
    [SerializeField] private Image[] slotIcons = new Image[3];

    private readonly string[] itemIDs = new string[3];
    private readonly Sprite[] itemSprites = new Sprite[3];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        ConnectInventoryUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ConnectInventoryUI();
    }

    private void ConnectInventoryUI()
    {
        // Find the inventory canvas in the newly loaded scene.
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas == null)
        {
            Debug.LogWarning("InventorySystem: Canvas not found.");
            return;
        }

        Transform inventory = canvas.transform.Find("Inventory");

        if (inventory == null)
        {
            Debug.LogWarning("InventorySystem: Inventory object not found under Canvas.");
            return;
        }

        Image[] newSlots = new Image[3];

        for (int i = 0; i < 3; i++)
        {
            Transform slot = inventory.Find("Slot" + (i + 1));

            if (slot == null)
            {
                Debug.LogWarning("InventorySystem: Slot" + (i + 1) + " not found.");
                return;
            }

            newSlots[i] = slot.Find("Itemicon" + (i + 1))?.GetComponent<Image>();
        }

        slotIcons = newSlots;
        RefreshInventoryUI();

        Debug.Log("Inventory UI connected for scene: " + sceneName());
    }

    private string sceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public bool AddItem(string itemID, Sprite itemIcon)
    {
        if (string.IsNullOrEmpty(itemID) || itemIcon == null)
        {
            Debug.LogWarning("InventorySystem: Item ID or icon is missing.");
            return false;
        }

        if (HasItem(itemID))
            return false;

        for (int i = 0; i < itemIDs.Length; i++)
        {
            if (string.IsNullOrEmpty(itemIDs[i]))
            {
                itemIDs[i] = itemID;
                itemSprites[i] = itemIcon;

                RefreshInventoryUI();

                Debug.Log(itemID + " added to inventory slot " + (i + 1));
                return true;
            }
        }

        Debug.LogWarning("Inventory is full!");
        return false;
    }

    public bool HasItem(string itemID)
    {
        foreach (string id in itemIDs)
        {
            if (id == itemID)
                return true;
        }

        return false;
    }

    public bool RemoveItem(string itemID)
    {
        for (int i = 0; i < itemIDs.Length; i++)
        {
            if (itemIDs[i] == itemID)
            {
                itemIDs[i] = null;
                itemSprites[i] = null;

                RefreshInventoryUI();

                Debug.Log(itemID + " removed from inventory.");
                return true;
            }
        }

        return false;
    }

    private void RefreshInventoryUI()
    {
        if (slotIcons == null)
            return;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            Image icon = slotIcons[i];

            if (icon == null)
                continue;

            icon.sprite = itemSprites[i];
            icon.enabled = itemSprites[i] != null;
            icon.gameObject.SetActive(itemSprites[i] != null);
        }
    }
}
