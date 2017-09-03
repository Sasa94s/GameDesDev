using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPlatform : MonoBehaviour
{
    // Coin Prefab
    public GameObject coinPrefab;
    // Units of Platform Tiles
    public int units = 3;
    // Place to be spawned in Y-Axis
    public float spawnY = 1.5f;


    //// Private Variables

    // Delta is the constant size of every Tile in Platform
    private float spawnXDelta;
    // Place to be spawned in X-Axis
    private float spawnX = 0.0f;
    // Coins' references in each platform
    private List<GameObject> coins;

	// Use this for initialization
	public void Awake ()
	{
	    Initialize();

	    createCoinsAlongPlatform();
    }

    void Start()
    {


        // Randomizing coins on platform
        //randomizeCoins();
    }

    private void Initialize()
    {
        // Initializing array of coins, It's length is number of tiles
        coins = new List<GameObject>();
        // Delta is the width of platform divided to number of tiles of platform
        spawnXDelta = GetComponent<BoxCollider2D>().size.x / units;
    }

    private void createCoinsAlongPlatform()
    {
        // creating first coin centered in x-position
        createCoin(1);
        for (int i = 1; i < units/2+units%2; ++i)
        {
            // moving spawn point of x to the next unit
            spawnX += spawnXDelta;
            // creating two coins, left and right
            createCoin(1);
            createCoin(-1);
        }
    }

    private void createCoin(int direction)
    {
        // Instantiating coin game object
        GameObject coin = Instantiate(coinPrefab) as GameObject;
        // Attaching it as a parent to the platform
        coin.transform.parent = transform;
        // Setting it's local position relatively to the parent
        coin.transform.localPosition = new Vector3(direction*spawnX, spawnY, 0);
        coins.Add(coin);
    }

    private void activeCoin(int n, bool state)
    {
        if (n >= units) return;
        coins[n].SetActive(state);
    }

    public void disableAll()
    {
        for (int i = 0; i < units; i++)
        {
            activeCoin(i, false);
        }
    }
    public void randomizeCoins()
    {
        disableAll();
        int n = Random.Range(1, units);
        randomize(0, n);
    }

    private void randomize(int i, int n)
    {
        if (i >= units) return;
        activeCoin(i, true);
        randomize(i+n, n);
    }
}
