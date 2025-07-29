using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public RectTransform minimapRect;
    public RectTransform playerIcon;

    public Transform player;

    public Vector2 mapSize = new Vector2(100, 100); // kích thước thật của map
    public Vector2 minimapSize = new Vector2(200, 200); // pixel UI minimap

    void Update()
    {
        UpdateIconPosition(player, playerIcon);
    }

    void UpdateIconPosition(Transform target, RectTransform icon)
    {
        Vector2 normalizedPos = new Vector2(
            target.position.x / mapSize.x,
            target.position.z / mapSize.y
        );

        Vector2 anchoredPos = new Vector2(
            normalizedPos.x * minimapSize.x,
            normalizedPos.y * minimapSize.y
        );

        // Căn giữa minimap
        anchoredPos -= minimapSize / 2f;

        icon.anchoredPosition = anchoredPos;
    }
}
