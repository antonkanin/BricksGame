using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlocksGenerator : MonoBehaviour
{
    public GameObject blockPrefab;

    public GameObject baseBlock;
    private GameObject _currentBlock;

    public Transform blocksContainer;

    public float initialSpawnPositionY = -4.0f;

    private float _spawnPositionX;
    private float _spawnPositionY;

    public float yShift = 0.5f;

    public IntType scoreSO;

    public GameEvent gameOverEvent;

    private bool isGameStarted = false;

    void Start()
    {
        InitializeGame();
    }

    public void GameRestart()
    {
        BlocksCleanUp();
        InitializeGame();
    }

    void Update()
    {
        if (!isGameStarted && Input.GetMouseButton(0))
        {
            SpawnBlock();
            isGameStarted = true;
        }
    }

    void InitializeGame()
    {
        _spawnPositionY = initialSpawnPositionY;
        _spawnPositionX = -1.0f;
        scoreSO.value = 0;
        isGameStarted = false;
        _currentBlock = baseBlock;
    }

    void BlocksCleanUp()
    {
        foreach (Transform child in blocksContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void SpawnBlock()
    {
        var newBlock = Instantiate(blockPrefab, new Vector3(_spawnPositionX, _spawnPositionY, 0.0f),
            Quaternion.identity);

        newBlock.transform.parent = blocksContainer;

        var blockMovement = newBlock.GetComponent<BlockMovement>();

        blockMovement.OnStopBlockMovement += SpawnBlock;
        blockMovement.OnMissedBlock += GameOverCallback;
        blockMovement.isMoving = true;
        blockMovement.movingDirectionX = -1.0f * _spawnPositionX;

        if (_currentBlock)
        {
            newBlock.transform.localScale = _currentBlock.transform.localScale;
            blockMovement.lowerBlock = _currentBlock;
        }

        newBlock.GetComponent<SpriteRenderer>().color =
            new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        _spawnPositionX *= -1.0f;
        _spawnPositionY += yShift;

        _currentBlock = newBlock;

        TryShiftCamera();

        AddScore();
    }

    void TryShiftCamera()
    {
        if (_currentBlock.transform.position.y < 2.0f)
        {
            return;
        }

        if (Camera.main != null)
        {
            var cameraTransform = Camera.main.transform;
            var cameraPos = cameraTransform.position;
            cameraTransform.GetComponent<MoveCamera>().targetPosition =
                new Vector3(cameraPos.x, cameraPos.y + 0.5f, cameraPos.z);
        }
        else
        {
            throw new Exception("Main camera not found");
        }
    }

    void AddScore()
    {
        scoreSO.value++;
    }

    void GameOverCallback()
    {
        Debug.Log("Game over");
        gameOverEvent.Raise();
    }
}