using Zenject;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public LevelDataPaths LevelDataPaths;
    public Transform ShelvesParent;

    public int CurrentLevelIndex = 1;
    public int ShelvesPerRow = 4;
    public float ShelfSpacingX = 0f;
    public float ShelfSpacingY = 0f;
    public float SlotSpacing = 1.0f;
    public float ShelfDownOffset = 2f;

    private LevelData _currentLevelData;
    private List<int> _itemPool = new List<int>();
    private List<int> _itemPoolReduced = new List<int>();

    private GameObject _shelfPrefab;
    private GameObject _slotPrefab;
    private GameObject _itemPrefab;
    private int itemTypes;

    public GameObject PopupText;
    public GameObject Star;
    public GameObject StarTargetPosition;
    public LevelDisplay LevelDisp;

    public int Seconds;
    public Timer TimerScript;

    private int _destroyedItems;

    private int _numberOfAvailableItems;

    public ComboBar Combo;

    [Inject]
    public void Construct(
        [Inject(Id = "ShelfPrefab")] GameObject shelfPrefab,
        [Inject(Id = "SlotPrefab")] GameObject slotPrefab,
        [Inject(Id = "ItemPrefab")] GameObject itemPrefab)
    {
        _shelfPrefab = shelfPrefab;
        _slotPrefab = slotPrefab;
        _itemPrefab = itemPrefab;

        Debug.Log($"ShelfPrefab injected: {_shelfPrefab != null}");
        Debug.Log($"SlotPrefab injected: {_slotPrefab != null}");
        Debug.Log($"ItemPrefab injected: {_itemPrefab != null}");
    }

    private void Start()
    {
        _numberOfAvailableItems = _itemPrefab.GetComponent<ItemType>().Sprites.Count;
        ShelvesParent = this.transform;
        LoadCurrentLevel();

    }

    public void MatchedItems()
    {
        _destroyedItems++;
        Debug.Log(_destroyedItems + "  " + itemTypes);
        if (_destroyedItems == itemTypes)
        {
            if (CurrentLevelIndex < 4)
            {
                StartCoroutine(DestroyChildrenAndProceedToNextLevel());
            }
            else
            {
                Debug.Log("You won the game!!");
            }
        }
    }

    private IEnumerator DestroyChildrenAndProceedToNextLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        yield return new WaitUntil(() => transform.childCount == 0);

        _destroyedItems = 0;

        NextLevel();
    }

    public void LoadCurrentLevel()
    {
        Combo.ResetCombo();
        _itemPool.Clear();
        if (CurrentLevelIndex < 0 || CurrentLevelIndex >= LevelDataPaths.levelPaths.Length)
        {
            Debug.LogError("Invalid level index!");
            return;
        }

        LevelDisp.UpdateLevelDisplay(CurrentLevelIndex);

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
        Seconds = _currentLevelData.duration;
        TimerScript.Duration = Seconds;
        TimerScript.TimeLeft = Seconds;
        TimerScript.Text.color = Color.white;
    }

    private void CreateShelves()
    {
        int orderInLayer = 5;

        for (int i = 0; i < _currentLevelData.shelves; i++)
        {
            Vector3 position = CalculateShelfPosition(i);
            GameObject shelf = Instantiate(_shelfPrefab, position, Quaternion.identity, ShelvesParent);
            Shelf shelfScript = shelf.GetComponent<Shelf>();
            shelfScript.layers = _currentLevelData.layers;
            shelfScript.width = 3; // 3 slots wide

            SpriteRenderer shelfRenderer = shelf.GetComponent<SpriteRenderer>();

            shelfRenderer.sortingOrder = orderInLayer - i/3;


            float shelfWidth = shelfRenderer.bounds.size.x;

            float slotWidth = _slotPrefab.GetComponent<SpriteRenderer>().bounds.size.x * _slotPrefab.transform.localScale.x;
            float spacing = SlotSpacing;

            float leftPos = position.x - (shelfWidth / 2) + (slotWidth / 2) + spacing;
            float middlePos = position.x;
            float rightPos = position.x + (shelfWidth / 2) - (slotWidth / 2) - spacing;

            for (int layer = 0; layer < shelfScript.layers; layer++)
            {
                // Left slot
                Vector3 leftSlotPosition = new Vector3(leftPos, position.y, 0);
                GameObject leftSlot = Instantiate(_slotPrefab, leftSlotPosition, Quaternion.identity, shelf.transform);
                Slot leftSlotScript = leftSlot.GetComponent<Slot>();
                leftSlotScript.Layer = layer;

                // Middle slot
                Vector3 middleSlotPosition = new Vector3(middlePos, position.y, 0);
                GameObject middleSlot = Instantiate(_slotPrefab, middleSlotPosition, Quaternion.identity, shelf.transform);
                Slot middleSlotScript = middleSlot.GetComponent<Slot>();
                middleSlotScript.Layer = layer;

                // Right slot
                Vector3 rightSlotPosition = new Vector3(rightPos, position.y, 0);
                GameObject rightSlot = Instantiate(_slotPrefab, rightSlotPosition, Quaternion.identity, shelf.transform);
                Slot rightSlotScript = rightSlot.GetComponent<Slot>();
                rightSlotScript.Layer = layer;
            }
        }
    }

    private Vector3 CalculateShelfPosition(int shelfIndex)
    {
        if (_shelfPrefab == null)
        {
            return Vector3.zero;
        }

        SpriteRenderer spriteRenderer = _shelfPrefab.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            return Vector3.zero;
        }

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        Vector2 shelfScaledSize = new Vector2(spriteSize.x * _shelfPrefab.transform.localScale.x,
                                              spriteSize.y * _shelfPrefab.transform.localScale.y);

        int row = shelfIndex / ShelvesPerRow;
        int column = shelfIndex % ShelvesPerRow;

        float posX = column * (shelfScaledSize.x + ShelfSpacingX);
        float posY = -row * (shelfScaledSize.y + ShelfSpacingY);
        if (CurrentLevelIndex % 2 == 0 && (shelfIndex + 2) % 3 == 0)
            posY -= ShelfDownOffset;
        return new Vector3(posX, posY, 0);
    }




    private void PopulateItemPool()
    {
        _itemPool.Clear();
        _itemPoolReduced.Clear();

        itemTypes = 2;

        if (_currentLevelData.type == "Start")
            itemTypes = 2;
        if (_currentLevelData.type == "Normal")
            itemTypes = 16;
        if (_currentLevelData.type == "Hard")
            itemTypes = 22;
        if (_currentLevelData.type == "SuperHard")
            itemTypes = 30;

        int itemsPerType = 3;

        for (int i = 0; i < _numberOfAvailableItems; i++)
        {
            _itemPoolReduced.Add(i);
        }

        _itemPoolReduced = _itemPoolReduced.OrderBy(x => Random.Range(0, _itemPoolReduced.Count)).ToList();


        for (int i = 0; i < itemTypes; i++)
        {
            for (int j = 0; j < itemsPerType; j++)
            {
                _itemPool.Add(_itemPoolReduced[i]);
            }
        }
        Debug.Log("Item pool count: "+_itemPool.Count);

        _itemPool = _itemPool.OrderBy(x => Random.Range(0, _itemPool.Count)).ToList();
    }



    private void PlaceItemsInShelves()
    {
        List<Transform> allSlots = new List<Transform>();

        // Collect all available slots
        foreach (Transform shelf in ShelvesParent)
        {
            foreach (Transform slot in shelf)
            {
                Slot slotScript = slot.GetComponent<Slot>();
                if (slotScript != null)
                {
                    allSlots.Add(slot);
                }
            }
        }

        allSlots = allSlots.OrderBy(x => Random.Range(0, allSlots.Count)).ToList();


        if (allSlots.Count < _itemPool.Count)
        {
            Debug.LogError("Not enough slots to place all items!");
            return;
        }

        List<int> itemQueue = new List<int>(_itemPool);

        // Shuffle the items initially
        itemQueue = itemQueue.OrderBy(x => Random.Range(0, itemQueue.Count)).ToList();

        // Adjust items to avoid consecutive duplicates
        for (int i = 0; i < itemQueue.Count; i++)
        {
            if (i > 1 && itemQueue[i] == itemQueue[i - 1] && itemQueue[i] == itemQueue[i - 2])
            {
                // Find a different item to swap
                bool itemSwapped = false;
                for (int j = i + 1; j < itemQueue.Count; j++)
                {
                    if (itemQueue[j] != itemQueue[i - 1])
                    {
                        // Swap items
                        int temp = itemQueue[i];
                        itemQueue[i] = itemQueue[j];
                        itemQueue[j] = temp;
                        itemSwapped = true;
                        break;
                    }
                }

                if (!itemSwapped)
                {
                    return;
                }
            }
        }

        int itemIndex = 0;
        for (int i = 0; i < allSlots.Count; i++)
        {
            if (itemIndex >= itemQueue.Count)
            {
                Debug.Log("All items have been placed.");
                break;
            }

            Slot slotScript = allSlots[i].GetComponent<Slot>();

            if (slotScript != null && !slotScript.IsHoldingItem)
            {
                int currentItem = itemQueue[itemIndex];
                GameObject item = Instantiate(_itemPrefab, allSlots[i].position, Quaternion.identity);

                ItemType itemScript = item.GetComponent<ItemType>();
                if (itemScript == null)
                {
                    continue;
                }

                itemScript.Type = currentItem;
                slotScript.HoldTheItem(true);

                item.transform.SetParent(allSlots[i]);
                itemScript.ChangeColorTint(slotScript.Layer);
                itemScript.ChangeOrderInLayer(slotScript.Layer);

                itemIndex++;
            }
            else
            {
            }
        }

        Debug.Log($"Items placed: {itemIndex}, Items left in pool: {itemQueue.Count - itemIndex}");
    }

    public void SpawnPopup(Vector2 position)
    {
        if(Combo.ComboLevel>=2)
            Instantiate(PopupText, position, Quaternion.identity);
        StartCoroutine(SpawnStarsWithDelay(position));
    }

    private IEnumerator SpawnStarsWithDelay(Vector2 position)
    {
        int starCount = Mathf.Max(Combo.ComboLevel / 2, 1);

        for (int i = 0; i < starCount; i++)
        {
            Vector2 randomOffset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            Vector2 starPosition = position + randomOffset;

            Instantiate(Star, starPosition, Quaternion.identity, this.transform);

            yield return new WaitForSeconds(0.1f);
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
