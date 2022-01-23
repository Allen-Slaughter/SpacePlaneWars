using UnityEngine;

public class OverdriveMaterialController : MonoBehaviour
{
    [SerializeField] Material overdriveMaterial;

    Material defaultMaterial;

    new Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        defaultMaterial = renderer.material;
    }

    void OnEnable()
    {
        PlayerOverdirve.on += PlayerOverdriveOn;
        PlayerOverdirve.off += PlayerOverdriveOff;
    }

    void OnDisable()
    {
        PlayerOverdirve.on -= PlayerOverdriveOn;
        PlayerOverdirve.off -= PlayerOverdriveOff;
    }

    void PlayerOverdriveOn() => renderer.material = overdriveMaterial;

    void PlayerOverdriveOff() => renderer.material = defaultMaterial;
}