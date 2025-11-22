using UnityEngine;

/// <summary>
/// 진척도
/// </summary>
public class TravelMap : MonoBehaviour
{
    [SerializeField] private RectTransform TravelMapRoot;
    [SerializeField] private Transform MarkerRoot;
    [SerializeField] private TravelMarker MarkerPrefab;
    [SerializeField] private TravelMarker PlayerMarker;

    float totalHeight;
    float markerHeight;
    float markerSpacing;

    public void Initialize(int waveCount){
        totalHeight = TravelMapRoot.rect.height;
        markerHeight = MarkerPrefab.rectTransform.rect.height;
        markerSpacing = (totalHeight - (markerHeight * (waveCount + 1))) / waveCount;
        markerSpacing += markerHeight;
        for(int i = 0; i < waveCount + 1; i++){
            TravelMarker marker = Instantiate(MarkerPrefab, MarkerRoot);
            marker.rectTransform.anchoredPosition = new Vector2(0, markerSpacing * i);
        }
        PlayerMarker.rectTransform.anchoredPosition = new Vector2(0, markerHeight / 2);
    }

    public void UpdatePlayerPosition(int wave, float ratio){
        float position = Mathf.Lerp(0, markerSpacing, ratio);
        PlayerMarker.rectTransform.anchoredPosition = new Vector2(0, markerSpacing * wave + position + (markerHeight / 2));
    }

    public void Clear(){
        foreach(Transform child in MarkerRoot){
            Destroy(child.gameObject);
        }
    }
}
