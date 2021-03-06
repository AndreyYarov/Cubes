using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(1f)] private float m_Speed = 5f;
    [SerializeField, Min(1f)] private float m_JumpSpeed = 5f;

    private Transform _head;
    public Transform head
    {
        get
        {
            if (!_head)
            {
                var camera = GetComponentInChildren<Camera>();
                if (!camera)
                {
                    GameObject go = new GameObject("Head Camera");
                    go.tag = "MainCamera";
                    camera = go.AddComponent<Camera>();
                    go.AddComponent<AudioListener>();
                    go.AddComponent<CameraController>();
                }
                else if (!camera.GetComponent<CameraController>())
                    camera.gameObject.AddComponent<CameraController>();

                _head = camera.transform;
            }
            return _head;
        }
    }

    private Rigidbody _rb;
    public Rigidbody rb
    {
        get
        {
            if (!_rb)
                _rb = GetComponent<Rigidbody>();
            return _rb;
        }
    }

    private void FixedUpdate()
    {
        if (!head)
            return;

        Vector3 forward = head.forward;
        forward.y = 0f;
        forward = forward.normalized * Input.GetAxis("Vertical");

        Vector3 right = head.right;
        right.y = 0f;
        right = right.normalized * Input.GetAxis("Horizontal");

        Vector3 movement = (forward + right).normalized * m_Speed * Time.fixedDeltaTime;
        transform.position += movement;
    }

    private void Update()
    {
        if (!rb)
            return;

        if (Input.GetAxis("Jump") > 0f)
        {
            var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
            if (Physics.Raycast(ray, 0.2f) && rb.velocity.y < m_JumpSpeed * 0.1f)
                rb.velocity += Vector3.up * m_JumpSpeed;
        }
    }

    public void Init(Vector3 startPos)
    {
        gameObject.SetActive(true);
        transform.position = startPos;
        rb.velocity = Vector3.zero;
    }
}
