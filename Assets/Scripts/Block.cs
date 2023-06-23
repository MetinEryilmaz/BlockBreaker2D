using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Block : MonoBehaviour
{
    public int blockHealth;
    public Vector2 gridCoordinates;
    public BlockType blockType;
    public int neighboursToKillAmount;
    [HideInInspector] public Level level;

    [Space(20)]
    [Header("Block Visuals")]
    [Tooltip("Low to high")] [SerializeField] Gradient healthGradient;
    [SerializeField] Image mainImage;
    [SerializeField] List<GameObject> crackImages;
    [SerializeField] GameObject normalBlockSprite;
    [SerializeField] GameObject verticalKillerSprite;
    [SerializeField] GameObject horizontalKillerSprite;
    [SerializeField] GameObject neighbourKillerSprite;
    [Tooltip("A script that spawns a text with an animation")] [SerializeField] Plusone plusOne;


    Vector3 startingScale;
    int startingHealth;
    bool isPunchingScale = false;
    bool isDead = false;
    bool isGettingHit = false;
    GameManager gameManager;

    private void Awake()
    {
        startingScale = transform.localScale;
        startingHealth = blockHealth;

        gameManager = GameManager.Instance;
        SetStartingVisuals();
        SetHealthVisuals();
    }

    ///<summary>Gives damage to the block. Gives score to the player.</summary>
    ///<param name="damage">How much damage the block will get</param>
    ///<param name="combo">If the block gives damage to other blocks, it will ++ this combo and sends it to them.</param>
    public void GetHit(int damage, int combo)
    {
        if (!isDead && !isGettingHit)//Just in case if you hit this block again after its death && Not to call it multiple times if it gets hit by a couple of blocks at the same time
        {
            isGettingHit = true;
            blockHealth -= damage;
            Debug.Log("Hit:" + gridCoordinates.x + "," + gridCoordinates.y + "  Combo: " + combo);

            //Send plusOne text to show given damage
            string comboStr = "";
            if (combo > 1) { comboStr = " X" + combo; }
            Instantiate(plusOne, new Vector3(transform.position.x, transform.position.y - 1, -1), Quaternion.Euler(-20, 0, 0), transform).SendIt("-" + damage + comboStr, true) ;

            //Punch scale animation && Calls for block types' behaviours
            if (!isPunchingScale)
            {
                isPunchingScale = true;

                //Lerps to startingScale, then does the punching animation
                transform.DOScale(startingScale, 0.05f).OnComplete(() =>
                {
                    transform.DOPunchScale(Vector3.one * 0.1f, 0.2f).OnComplete(() =>
                    {
                        isPunchingScale = false;
                        isGettingHit = false;

                        #region BlockType Dying Behaviours
                        if (blockHealth <= 0)
                        {
                            if (blockType == BlockType.HorizontalKiller)
                            {
                                ExplodeHorizontalNeighbours(damage, combo);
                            }
                            else if (blockType == BlockType.VerticalKiller)
                            {
                                ExplodeVerticalNeighbours(damage, combo);
                            }
                            else if (blockType == BlockType.NeighbourKiller)
                            {
                                ExplodeSurroundingNeighbours(damage, combo);
                            }

                            gameManager.AddScore(damage * combo);
                            //send plusOne text to show earned points+combo;
                            Instantiate(plusOne, new Vector3(transform.position.x, transform.position.y - 1, -1), Quaternion.Euler(-20, 0, 0), null).SendIt(damage + " x" + combo, false);

                            Die();
                        }
                        #endregion

                    });

                });
            }
            SetHealthVisuals();
        }
    }

    private void Die()
    {
        isDead = true;
        level.blockCount--;
        if (level.blockCount <= 0)
        {
            gameManager.GameOver(true);
        }
        Destroy(gameObject, 0.25f);//waiting for the punchScaleAnimation to end        
    }

    private void ExplodeVerticalNeighbours(int damage, int combo)
    {
        int numAbove = neighboursToKillAmount / 2;
        int numBelow = neighboursToKillAmount - numAbove;

        // Check upper neighbours
        for (int i = 1; i <= numAbove; i++)
        {
            int checkX = (int)gridCoordinates.x;
            int checkY = (int)gridCoordinates.y + i;

            if (level.IsValidGridPosition(checkX, checkY))
            {
                Block block = level.blockGrid[checkX, checkY];
                block.GetHit(damage/2, combo + 1);
            }
        }

        // Check lower neighbours
        for (int i = 1; i <= numBelow; i++)
        {
            int checkX = (int)gridCoordinates.x;
            int checkY = (int)gridCoordinates.y - i;

            if (level.IsValidGridPosition(checkX, checkY))
            {
                Block block = level.blockGrid[checkX, checkY];
                block.GetHit(damage/2, combo + 1);
            }
        }
    }

    private void ExplodeHorizontalNeighbours(int damage, int combo)
    {
        int numLeft = neighboursToKillAmount / 2;
        int numRight = neighboursToKillAmount - numLeft;

        // Check left neighbours
        for (int i = 1; i <= numLeft; i++)
        {
            int checkX = (int)gridCoordinates.x - i;
            int checkY = (int)gridCoordinates.y;

            if (level.IsValidGridPosition(checkX, checkY))
            {
                Block block = level.blockGrid[checkX, checkY];
                block.GetHit(damage/2, combo + 1);
            }
        }

        // Check right neighbours
        for (int i = 1; i <= numRight; i++)
        {
            int checkX = (int)gridCoordinates.x + i;
            int checkY = (int)gridCoordinates.y;

            if (level.IsValidGridPosition(checkX, checkY))
            {
                Block block = level.blockGrid[checkX, checkY];
                block.GetHit(damage/2, combo + 1);
            }
        }
    }

    private void ExplodeSurroundingNeighbours(int damage, int combo)
    {
        int startX = (int)gridCoordinates.x - 1;
        int startY = (int)gridCoordinates.y - 1;
        int endX = (int)gridCoordinates.x + 1;
        int endY = (int)gridCoordinates.y + 1;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (level.IsValidGridPosition(x, y) && (x != (int)gridCoordinates.x || y != (int)gridCoordinates.y))
                {
                    Block block = level.blockGrid[x, y];
                    block.GetHit(damage/2, combo + 1);
                }
            }
        }
    }


    ///<summary>Adjusts the sprite color and the crack images on the block based on the health.</summary>
    private void SetHealthVisuals()
    {
        float healthNormalized = Extentions.Remap(blockHealth, 0, startingHealth, 0, 1);

        mainImage.color = healthGradient.Evaluate(healthNormalized);

        int cracksToOpen = Mathf.Clamp(Mathf.CeilToInt((1 - healthNormalized) * crackImages.Count), 0, crackImages.Count);

        for (int i = 0; i < cracksToOpen; i++)
        {
            crackImages[i].SetActive(true);
        }
    }

    /// <summary>Selects the correct block type images</summary>
    public void SetStartingVisuals()
    {
        if (blockType == BlockType.Normal)
        {
            normalBlockSprite.SetActive(true);
        }
        else if (blockType == BlockType.HorizontalKiller)
        {
            horizontalKillerSprite.SetActive(true);
        }
        else if (blockType == BlockType.VerticalKiller)
        {
            verticalKillerSprite.SetActive(true);
        }
        else if (blockType == BlockType.NeighbourKiller)
        {
            neighbourKillerSprite.SetActive(true);
        }
    }
}
public enum BlockType { Normal, HorizontalKiller, VerticalKiller, NeighbourKiller }
