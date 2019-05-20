using System;
using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    public float shiftSpeed;

    public GameObject lowerBlock;

    public bool isMoving = true;
    public float movingDirectionX = 1.0f;

    public GameObject fragmentPrefab;

    public delegate void StopBlockMovementAction();

    public event StopBlockMovementAction OnStopBlockMovement;

    public delegate void MissedBlockAction();

    public event MissedBlockAction OnMissedBlock;

    void Update()
    {
        HandleInput();
        TryMovingBlock();
    }

    void HandleInput()
    {
        if (isMoving && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            isMoving = false;

            if (CheckBlocksIntersection())
            {
                UpdateBlockPositionAndScale();

                OnStopBlockMovement?.Invoke();
            }
            else // handle block miss
            {
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                OnMissedBlock?.Invoke();
            }
        }
    }

    void UpdateBlockPositionAndScale()
    {
        var lowerBlockX = lowerBlock.transform.position.x;
        var currentBlockX = transform.position.x;

        var deltaX = lowerBlockX - currentBlockX;

        // decrease block scale so it matches the the part the covers the block below 
        var scale = transform.localScale;
        var scaleDelta = (scale.x - Math.Abs(deltaX)) / scale.x;
        var oldScaleX = scale.x;

        transform.localScale = new Vector3(scale.x * scaleDelta, scale.y, scale.z);

        // shift reduced block
        var pos = transform.position;

        transform.position = new Vector3(pos.x + deltaX / 2, pos.y, pos.z);

        // spawn fragment that falls down
        SpawnFallingFragment(deltaX, oldScaleX, transform.localScale, transform.position);
    }

    void TryMovingBlock()
    {
        if (isMoving)
        {
            var pos = gameObject.transform.position;
            transform.position = new Vector3(pos.x + shiftSpeed * movingDirectionX * Time.deltaTime, pos.y, pos.z);

            if (Math.Abs(transform.position.x) > 3)
            {
                shiftSpeed *= -1.0f;
            }
        }
    }

    void SpawnFallingFragment(float deltaX, float oldScaleX, Vector3 scale, Vector3 pos)
    {
        var fragmentPosX = pos.x - Mathf.Sign(deltaX) * oldScaleX / 2 + deltaX / 2;
        var fragmentScaleDelta = Math.Abs(deltaX) / oldScaleX;

        var fragment = Instantiate(fragmentPrefab, new Vector3(fragmentPosX, pos.y, pos.z),
            Quaternion.identity);

        fragment.transform.parent = transform.parent;

        var fragmentScale = fragment.transform.localScale;
        fragment.transform.localScale =
            new Vector3(scale.x / 2.0f * fragmentScale.x * fragmentScaleDelta, fragmentScale.y, fragmentScale.z);
        fragment.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        fragment.GetComponent<Rigidbody2D>().AddTorque(Mathf.Sign(deltaX), ForceMode2D.Impulse);

        Destroy(fragment, 10.0f);
    }

    bool CheckBlocksIntersection()
    {
        var blockX = transform.position.x;
        var lowerBlockX = lowerBlock.transform.position.x;
        var lowerBlockScaleX = lowerBlock.transform.localScale.x;

        return Math.Abs(blockX - lowerBlockX) < lowerBlockScaleX / 2.0f;
    }
}