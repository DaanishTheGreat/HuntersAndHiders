using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteOnMap : MonoBehaviour
{
    public Image SpriteImage;
    public Sprite SpriteToSpawn;
    public Vector2 SpawnPosition;

    Image spawnedImage;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPosition.x = 0;
        SpawnPosition.y = 0;
        SpawnedSprite();
    }

    private void SpawnedSprite()
    {
        // Instantiate the Image prefab
        spawnedImage = Instantiate(SpriteImage, transform);

        // Set the position of the spawned image in local space
        spawnedImage.rectTransform.localPosition = SpawnPosition;

        // Set the sprite of the spawned image
        spawnedImage.sprite = SpriteToSpawn;

        // Ensure the spawned image is active
        spawnedImage.gameObject.SetActive(true);
    }

    public void UpdateSpriteLocationInGame(double Normalized_XCoordinate, double Normalized_YCoordinate)
    {
        SpawnPosition.x = (float)Normalized_XCoordinate;
        SpawnPosition.y = (float)Normalized_YCoordinate;
        spawnedImage.rectTransform.localPosition = SpawnPosition;
    }
}
