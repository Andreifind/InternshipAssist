using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public LevelDataPaths LevelDataPaths;
    public GameObject ShelfPrefab;
    public GameObject SlotPrefab;
    public GameObject ItemPrefab; 
    public Transform ShelvesParent;

    public int CurrentLevelIndex = 1;
    public int ShelvesPerRow = 4; 
    public float ShelfSpacingX = 0f; 
    public float ShelfSpacingY = 0f; 
    public float SlotSpacing = 1.0f;


    private LevelData _currentLevelData;
    private List<int> _itemPool = new List<int>(); 

    private void Start()
    {
        ShelvesParent = this.transform;
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        if (CurrentLevelIndex < 0 || CurrentLevelIndex >= LevelDataPaths.levelPaths.Length)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        string path = LevelDataPaths.levelPaths[CurrentLevelIndex];
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogError($"Level data not found at path: {path}");
            return;
        }

        _currentLevelData = JsonUtility.FromJson<LevelData>(jsonFile.text);

        CreateShelves();
        PopulateItemPool();
        PlaceItemsInShelves();
    }

    void CreateShelves()
    {
        for (int i = 0; i < _currentLevelData.shelves; i++)
        {
            Vector3 position = CalculateShelfPosition(i);

            GameObject shelf = Instantiate(ShelfPrefab, position, Quaternion.identity, ShelvesParent);
            Shelf shelfScript = shelf.GetComponent<Shelf>();
            shelfScript.layers = _currentLevelData.layers;
            shelfScript.width = 3; // 3 slots wide

            SpriteRenderer shelfRenderer = shelf.GetComponent<SpriteRenderer>();
            float shelfWidth = shelfRenderer.bounds.size.x; 

            float slotWidth = SlotPrefab.GetComponent<SpriteRenderer>().bounds.size.x * SlotPrefab.transform.localScale.x;
            float spacing = SlotSpacing;

            float leftPos = position.x - (shelfWidth / 2) + (slotWidth / 2) + spacing;
            float middlePos = position.x;
            float rightPos = position.x + (shelfWidth / 2) - (slotWidth / 2) - spacing;

            // Create slots
            for (int layer = 0; layer < shelfScript.layers; layer++)
            {
                // Left slot
                Vector3 leftSlotPosition = new Vector3(leftPos, position.y, 0);
                GameObject leftSlot = Instantiate(SlotPrefab, leftSlotPosition, Quaternion.identity, shelf.transform);
                Slot leftSlotScript = leftSlot.GetComponent<Slot>();
                leftSlotScript.Layer = layer;

                // Middle slot
                Vector3 middleSlotPosition = new Vector3(middlePos, position.y, 0);
                GameObject middleSlot = Instantiate(SlotPrefab, middleSlotPosition, Quaternion.identity, shelf.transform);
                Slot middleSlotScript = middleSlot.GetComponent<Slot>();
                middleSlotScript.Layer = layer;

                // Right slot
                Vector3 rightSlotPosition = new Vector3(rightPos, position.y, 0);
                GameObject rightSlot = Instantiate(SlotPrefab, rightSlotPosition, Quaternion.identity, shelf.transform);
                Slot rightSlotScript = rightSlot.GetComponent<Slot>();
                rightSlotScript.Layer = layer;
            }
        }
    }







    Vector3 CalculateShelfPosition(int shelfIndex)
    {
        SpriteRenderer spriteRenderer = ShelfPrefab.GetComponent<SpriteRenderer>();

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        Vector2 shelfScaledSize = new Vector2(spriteSize.x * ShelfPrefab.transform.localScale.x,
                                              spriteSize.y * ShelfPrefab.transform.localScale.y);

        int row = shelfIndex / ShelvesPerRow;
        int column = shelfIndex % ShelvesPerRow;

        float posX = column * (shelfScaledSize.x + ShelfSpacingX);
        float posY = -row * (shelfScaledSize.y + ShelfSpacingY);

        return new Vector3(posX, posY, 0);
    }


    void PopulateItemPool()
    {
        _itemPool.Clear();

        for (int i = 0; i < _currentLevelData.items; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _itemPool.Add(i); 
            }
        }

        for (int i = 0; i < _itemPool.Count; i++)
        {
            int randomIndex = Random.Range(0, _itemPool.Count);
            int temp = _itemPool[i];
            _itemPool[i] = _itemPool[randomIndex];
            _itemPool[randomIndex] = temp;
        }
    }

    void PlaceItemsInShelves()
{
    List<int> itemPool = new List<int>();
    for (int i = 0; i < _currentLevelData.items; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            itemPool.Add(i);
        }
    }

    System.Random rng = new System.Random();
    itemPool = itemPool.OrderBy(x => rng.Next()).ToList();

    List<Transform> allSlots = new List<Transform>();
    foreach (Transform shelf in ShelvesParent)
    {
        // Add all slots of this shelf to the list
        foreach (Transform slot in shelf)
        {
            Slot slotScript = slot.GetComponent<Slot>();
            if (slotScript != null)
            {
                allSlots.Add(slot);
            }
        }
    }

    allSlots = allSlots.OrderBy(x => rng.Next()).ToList();

    if (itemPool.Count > allSlots.Count)
    {
        Debug.LogError("Not enough slots to place all items!");
        return;
    }

    int itemIndex = 0;
    foreach (Transform slot in allSlots)
    {
        if (itemIndex >= itemPool.Count)
        {
            break;
        }

        Slot slotScript = slot.GetComponent<Slot>();
        if (slotScript != null && !slotScript.IsHoldingItem)
        {
            int itemType = itemPool[itemIndex];
            GameObject item = Instantiate(ItemPrefab, slot.position, Quaternion.identity);
            ItemType itemScript = item.GetComponent<ItemType>();
            itemScript.Type = itemType;
            slotScript.HoldTheItem(true);

            item.transform.SetParent(slot); 
            itemIndex++;
        }
    }
}
     

    public void NextLevel()
    {
        CurrentLevelIndex++;
        LoadCurrentLevel();
    }

    public void RestartLevel()
    {
        LoadCurrentLevel();
    }
}
