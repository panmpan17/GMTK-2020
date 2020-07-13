using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl ins;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private float followDistance;
    [SerializeField]
    private float moveSpeed, maxMoveSpeed;

    [SerializeField]
    private float focusTime, focusSize;
    private float focusTimer;
    private bool focusing;
    private bool unfocusing;

    private Vector3 focusOriginPos, focusTragetPos;

    private new Camera camera;
    private float originY;

    private void Awake() {
        ins = this;
        camera = GetComponent<Camera>();
        originY = transform.position.y;
    }

    private void Update() {
        if (focusing) {
            focusTimer += Time.deltaTime;

            focusTragetPos = target.position;
            focusTragetPos.y += 1;
            focusTragetPos.z = focusOriginPos.z;

            if (focusTimer >= focusTime) {
                camera.orthographicSize = focusSize;
                focusing = false;
                transform.position = focusTragetPos;
            }
            else {
                camera.orthographicSize = Mathf.Lerp(5, focusSize, focusTimer / focusTime);
                transform.position = Vector3.Lerp(focusOriginPos, focusTragetPos, focusTimer / focusTime);
            }
            return;
        }
        if (unfocusing) {
            focusTimer += Time.deltaTime;
            if (focusTimer >= focusTime) {
                camera.orthographicSize = 5;
                unfocusing = false;
                transform.position = focusTragetPos;
            }
            else {
                camera.orthographicSize = Mathf.Lerp(focusSize, 5, focusTimer / focusTime);
                transform.position = Vector3.Lerp(focusOriginPos, focusTragetPos, focusTimer / focusTime);
            }
            return;
        }



        float targetX = target.position.x;
        float delta = Mathf.Abs(targetX - transform.position.x);
        if (delta > followDistance)
        {
            delta -= followDistance;
            float speed = Mathf.Lerp(moveSpeed, maxMoveSpeed, Mathf.InverseLerp(0, 2, delta));

            Vector3 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, targetX, speed * Time.deltaTime);
            transform.position = pos;
        }
    }

    public void FocusCamera() {
        focusing = true;
        focusTimer = 0;
        focusOriginPos = transform.position;
        focusTragetPos = target.position;
        focusTragetPos.y += 1;
        focusTragetPos.z = focusOriginPos.z;
    }

    public void UnfocusCamera()
    {
        unfocusing = true;
        focusTimer = 0;

        focusOriginPos = transform.position;
        focusTragetPos = transform.position;
        focusTragetPos.y = originY;
    }

    // private IEnumerator Focus() {
    //     yield break;
    // }

    // private IEnumerator Unfocus() {
    //     yield break;
    // }
}
