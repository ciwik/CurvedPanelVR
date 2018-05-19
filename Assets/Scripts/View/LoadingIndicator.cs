using UnityEngine;

public class LoadingIndicator : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1f;

    private MeshRenderer _meshRenderer;

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void OnEnable()
    {
        _meshRenderer.enabled = true;
    }

    void OnDisable()
    {
        _meshRenderer.enabled = false;
    }

	void Update () {
		transform.Rotate(Vector3.forward, _speed);
	}
}
