using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    public float sprintMultiplier = 2f; // ����Ʈ �ӵ� ����
    public float mouseSensitivity = 2f; // ���콺 ����

    private float yaw = 0f;
    private float pitch = 0f;

    public MarchingCubes MarchingCubes;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� ����
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0)) // ���� Ŭ��
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MarchingCubes.DigTerrain(hit.point);
            }
        }
        // ī�޶� ȸ��
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // ���� ���� ����

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // �̵�
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= sprintMultiplier; // ����Ʈ�� �ӵ� ����
        }

        Vector3 move = new Vector3(
            Input.GetAxis("Horizontal"), // A(-1) / D(1)
            0f,
            Input.GetAxis("Vertical") // W(1) / S(-1)
        );
        move = transform.TransformDirection(move); // ���� ��ǥ ���� �̵�

        // Space�� ���, Shift�� �ϰ�
        if (Input.GetKey(KeyCode.Space))
        {
            move.y += 1;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move.y -= 1;
        }

        transform.position += move * speed * Time.deltaTime;
    }
}
