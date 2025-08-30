namespace HDU.GameSystem
{
    using UnityEngine;

    public class FreeCamera : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float lookSpeed = 3f;
        public CameraConfiner confiner;   // 아래 스크립트 참조

        private float yaw, pitch;

        void Update()
        {
            // 회전
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -80, 80);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);

            // 입력으로 목표 위치 계산
            Vector3 move =
                transform.right * Input.GetAxis("Horizontal") +
                transform.forward * Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.E)) move += Vector3.up;
            if (Input.GetKey(KeyCode.Q)) move += Vector3.down;

            Vector3 targetPos = transform.position + move * moveSpeed * Time.deltaTime;

            // ★ 허용 구역으로 위치 보정
            if (confiner)
                targetPos = confiner.ClampInside(targetPos);

            transform.position = targetPos;
        }
    }
}