using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 3f;

    private float yaw, pitch;

    void Update()
    {
        // ���콺 ȸ��
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -80, 80);
        transform.eulerAngles = new Vector3(pitch, yaw, 0);

        // Ű���� �̵�
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.Self);

        // ��/�Ʒ� �̵� (Q/E)
        if (Input.GetKey(KeyCode.E)) transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Q)) transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }
}
