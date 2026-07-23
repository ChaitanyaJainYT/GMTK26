using UnityEngine;
using TMPro;

public class BatParticleController : MonoBehaviour
{
    [Header("Bat Settings")]
    [SerializeField] private Sprite batSprite;
    [SerializeField] private int batCount = 5;
    [SerializeField] private float batLifetime = 1.2f;
    [SerializeField] private float batSpread = 2f;
    [SerializeField] private float batSpeed = 3f;

    [Header("Floating Text")]
    [SerializeField] private TMP_Text floatingTextPrefab;
    [SerializeField] private float textLifetime = 1.2f;
    [SerializeField] private float textRiseDistance = 1.5f;

    private DraculaController dracula;

    void Start()
    {
        dracula = FindObjectOfType<DraculaController>();
        if (dracula != null)
            dracula.OnJump += HandleJump;

        MirrorPortal[] portals = FindObjectsOfType<MirrorPortal>();
        foreach (var portal in portals)
            portal.OnWarpped += HandleWarp;
    }

    private void HandleJump()
    {
        if (dracula == null) return;
        BurstBats(dracula.transform.position + Vector3.up * 0.5f);
        SpawnFloatingText(dracula.transform.position + Vector3.up * 1.2f, "-1 Jump", new Color(1f, 0.3f, 0.3f));
    }

    private void HandleWarp()
    {
        if (dracula == null) return;
        BurstBats(dracula.transform.position);
        SpawnFloatingText(dracula.transform.position + Vector3.up * 1.2f, "Mirror Warp!", new Color(0.7f, 0.3f, 1f));
    }

    private void BurstBats(Vector3 origin)
    {
        if (batSprite == null) return;

        for (int i = 0; i < batCount; i++)
        {
            GameObject bat = new GameObject($"Bat_{i}");
            bat.transform.position = origin + Random.insideUnitSphere * batSpread * 0.3f;
            bat.transform.localScale = Vector3.one * 0.3f;

            SpriteRenderer sr = bat.AddComponent<SpriteRenderer>();
            sr.sprite = batSprite;
            sr.sortingOrder = 10;

            Vector2 dir = Random.insideUnitCircle.normalized;
            float speed = Random.Range(batSpeed * 0.5f, batSpeed);
            float angular = Random.Range(-360f, 360f);

            Destroy(bat, batLifetime);

            StartCoroutine(AnimateBat(bat, dir, speed, angular));
        }
    }

    private System.Collections.IEnumerator AnimateBat(GameObject bat, Vector2 dir, float speed, float angular)
    {
        float timer = 0f;
        while (timer < batLifetime && bat != null)
        {
            float t = timer / batLifetime;
            bat.transform.position += (Vector3)(dir * speed * Time.deltaTime * (1f - t * 0.5f));
            bat.transform.Rotate(0f, 0f, angular * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void SpawnFloatingText(Vector3 position, string text, Color color)
    {
        if (floatingTextPrefab == null) return;

        TMP_Text tmp = Instantiate(floatingTextPrefab, position, Quaternion.identity);
        tmp.text = text;
        tmp.color = color;
        tmp.transform.localScale = Vector3.one * 0.5f;

        Destroy(tmp.gameObject, textLifetime);

        StartCoroutine(AnimateFloatingText(tmp));
    }

    private System.Collections.IEnumerator AnimateFloatingText(TMP_Text tmp)
    {
        Vector3 startPos = tmp.transform.position;
        Vector3 endPos = startPos + Vector3.up * textRiseDistance;

        for (float t = 0; t < textLifetime; t += Time.deltaTime)
        {
            if (tmp == null) yield break;
            float p = t / textLifetime;
            tmp.transform.position = Vector3.Lerp(startPos, endPos, p);
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1f - p);
            yield return null;
        }
    }

    void OnDestroy()
    {
        if (dracula != null)
            dracula.OnJump -= HandleJump;

        MirrorPortal[] portals = FindObjectsOfType<MirrorPortal>();
        foreach (var portal in portals)
            portal.OnWarpped -= HandleWarp;
    }
}
