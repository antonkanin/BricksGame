using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlocksGenerator : MonoBehaviour
{
    public GameObject blockPrefab;

    private GameObject _currentBlock;

    public float spawnPositionY = -4.0f;

    private float _spawnPositionX;

    public float yShift = 0.5f;

    public IntType scoreSO;

    public GameEvent gameOverEvent;

    void Start()
    {
        SpawnBlock(false);
        _spawnPositionX = -1.0f;

        scoreSO.value = 0;
    }

    void SpawnBlockCallback()
    {
        SpawnBlock(true);
    }

    void SpawnBlock(bool isAddScore)
    {
        var newBlock = Instantiate(blockPrefab, new Vector3(_spawnPositionX, spawnPositionY, 0.0f),
            Quaternion.identity);

        var blockMovement = newBlock.GetComponent<BlockMovement>();

        blockMovement.OnStopBlockMovement += SpawnBlockCallback;
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
        spawnPositionY += yShift;

        _currentBlock = newBlock;

        TryShiftCamera();

        if (isAddScore)
        {
            AddScore();
        }
    }

    void TryShiftCamera()
    {
        if (_currentBlock.transform.position.y < 0)
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