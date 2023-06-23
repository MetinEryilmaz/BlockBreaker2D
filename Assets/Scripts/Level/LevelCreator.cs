using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData
{
    public BlockType blockType;
    [Range(0f, 100f)]
    public float spawnProbability;
    public int health;
    public int neighboursToKillAmount;
}

public class LevelCreator : MonoBehaviour
{
    [Header("Set the values you want and hit 'SPACE'")]
    [Space(30)]

    [Tooltip("Last one needs to be the normal type block")]public List<BlockData> blockDatas;
    [SerializeField] Vector2 gridSize;
    [SerializeField] Vector2 spacing;
    [SerializeField] Vector2 startingPosition;
    [SerializeField] GameObject levelPrefab;
    [SerializeField] GameObject blockPrefab;
    [HideInInspector] public Level levelScript;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (levelScript != null)
            {
                Destroy(levelScript.gameObject);
            }
            CreateLevel();
        }
    }

    ///<summary>Generates a level by instantiating blocks and setting their values based on the block data that gets selected by the probability percentage of itself</summary>
    public void CreateLevel(List<BlockData> blockDataCustom = null)
    {

        GameObject levelObject = Instantiate(levelPrefab);
        levelScript = levelObject.GetComponent<Level>();

        Transform blocksTransform = new GameObject("Blocks").transform;
        blocksTransform.SetParent(levelObject.transform);

        // Spawn objects in the grid and store them in the grid array
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                BlockData selectedBlockData;

                if (blockDataCustom != null)
                {
                    selectedBlockData = GetRandomBlockData(blockDataCustom);
                }
                else
                {
                    selectedBlockData = GetRandomBlockData();
                }

                //Spawn a new block
                GameObject spawnedObject = Instantiate(blockPrefab, Vector3.zero, Quaternion.identity, blocksTransform);
                Block block = spawnedObject.GetComponent<Block>();

                // Set the values for the blocks based on the given values in the selected blockData
                if (block != null)
                {
                    block.blockType = selectedBlockData.blockType;
                    block.gridCoordinates = new Vector2(x, y);
                    block.blockHealth = selectedBlockData.health;
                    block.neighboursToKillAmount = selectedBlockData.neighboursToKillAmount;
                    block.level = levelScript;
                    levelScript.blockList.Add(block);
                    levelScript.blockCount++;
                    block.SetStartingVisuals();
                }

                Vector2 position = new Vector2(x * spacing.x, y * spacing.y);
                spawnedObject.transform.position = position;
            }
        }

        levelScript.gridSize = gridSize;

        blocksTransform.position = new Vector3(startingPosition.x, startingPosition.y, 0);

    }

    /// <summary>Selects a random blockData based on the porbability</summary>
    /// <param name="blockData">If blockData is null it will use the one in this script</param>
    /// <returns>Returns the randomized blockData</returns>
    BlockData GetRandomBlockData(List<BlockData> blockDataCustom = null)
    {
        float randomValue = Random.value;
        float probability = 0f;

        if (blockDataCustom != null)
        {
            for (int i = 0; i < blockDataCustom.Count - 1; i++)
            {
                probability += (blockDataCustom[i].spawnProbability / 100);

                if (randomValue <= probability)
                {
                    return blockDataCustom[i];
                }
            }
            return blockDatas[blockDatas.Count - 1];
        }
        else
        {
            for (int i = 0; i < blockDatas.Count - 1; i++)
            {
                probability += (blockDatas[i].spawnProbability / 100);

                if (randomValue <= probability)
                {
                    return blockDatas[i];
                }
            }
        }

        // Return the last blockData if the first types of objects didn't spawn 
        return blockDatas[blockDatas.Count - 1];
    }
}
