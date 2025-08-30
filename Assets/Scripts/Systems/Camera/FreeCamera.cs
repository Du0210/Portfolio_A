namespace HDU.GameSystem
{
    using UnityEngine;

    public class FreeCamera : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float lookSpeed = 3f;
        public CameraConfiner confiner;   // �Ʒ� ��ũ��Ʈ ����

        private float yaw, pitch;

        void Update()
        {
            // ȸ��
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -80, 80);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);

            // �Է����� ��ǥ ��ġ ���
            Vector3 move =
                transform.right * Input.GetAxis("Horizontal") +
                transform.forward * Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.E)) move += Vector3.up;
            if (Input.GetKey(KeyCode.Q)) move += Vector3.down;

            Vector3 targetPos = transform.position + move * moveSpeed * Time.deltaTime;

            // �� ��� �������� ��ġ ����
            if (confiner)
                targetPos = confiner.ClampInside(targetPos);

            transform.position = targetPos;
        }
    }
}