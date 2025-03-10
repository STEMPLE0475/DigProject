using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float sprintMultiplier = 2f; // 쉬프트 속도 배율
    public float mouseSensitivity = 2f; // 마우스 감도

    private float yaw = 0f;
    private float pitch = 0f;

    public MarchingCubes MarchingCubes;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0)) // 왼쪽 클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MarchingCubes.DigTerrain(hit.point);
            }
        }
        // 카메라 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // 상하 각도 제한

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // 이동
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= sprintMultiplier; // 쉬프트로 속도 증가
        }

        Vector3 move = new Vector3(
            Input.GetAxis("Horizontal"), // A(-1) / D(1)
            0f,
            Input.GetAxis("Vertical") // W(1) / S(-1)
        );
        move = transform.TransformDirection(move); // 로컬 좌표 기준 이동

        // Space로 상승, Shift로 하강
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
