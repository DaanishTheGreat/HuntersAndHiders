using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteOnImage : MonoBehaviour
{
    public Image SpriteImage;
    public Image ImageMap;
    public Sprite SpriteToSpawn;
    public Vector2 SpawnPosition;


    // Start is called before the first frame update
    void Start()
    {
        SpawnPosition.x = -1250;
        SpawnPosition.y = 0;
        SpawnedSprite();
    }

    // Update is called once per frame
    private void SpawnedSprite()
    {
        // Instantiate the Image prefab
        Image spawnedImage = Instantiate(SpriteImage, transform);

        // Set the position of the spawned image in local space
        spawnedImage.rectTransform.localPosition = SpawnPosition;

        // Set the sprite of the spawned image
        spawnedImage.sprite = SpriteToSpawn;

        // Ensure the spawned image is active
        spawnedImage.gameObject.SetActive(true);
        
    }
}
