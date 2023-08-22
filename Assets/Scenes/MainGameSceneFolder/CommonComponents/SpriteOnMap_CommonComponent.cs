using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteOnMap_CommonComponent : MonoBehaviour
{
    public Image SpriteImage;
    public Sprite SpriteToSpawn;
    public Vector2 SpawnPosition;

    Image SpawnedImage;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPosition = new Vector2(0, 0);
        SpawnedImage = GetComponent<Image>();
        SpawnedImage.rectTransform.sizeDelta = new Vector2(25, 25);
        SpawnSprite();
    }

    private void SpawnSprite()
    {
        // Set the position of the spawned image in local space
        SpawnedImage.rectTransform.localPosition = SpawnPosition;

        // Set the sprite of the spawned image
        SpawnedImage.sprite = SpriteToSpawn;

        // Ensure the spawned image is active
        SpawnedImage.gameObject.SetActive(true);
    }

    public void UpdateSpriteLocationInGame(double Normalized_XCoordinate, double Normalized_YCoordinate)
    {
        SpawnPosition.x = (float)Normalized_XCoordinate;
        SpawnPosition.y = (float)Normalized_YCoordinate;
        SpawnedImage.rectTransform.localPosition = SpawnPosition;
    }
}
