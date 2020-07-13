using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Villiger
{
    [SerializeField]
    private float runSpeed;

    protected override void Update() {
        if (Manager.ins.Warning && !Manager.ins.Player.hiding) {
            walking = false;
            idleCount = 0;
            Vector3 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, Manager.ins.Player.transform.position.x, runSpeed * Time.deltaTime);
            transform.position = pos;
            transform.localScale = new Vector3(Manager.ins.Player.transform.position.x > pos.x? -1: 1, 1, 1);

            animCounter += Time.deltaTime;
            if (animCounter > (animSpeed * 0.8f)) {
                animCounter = 0;
                spriteRenderer.sprite = walks[index];
                if (++index >= walks.Length)
                    index = 0;
            }
        }

        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (Manager.ins.Warning && !Manager.ins.Player.hiding)
            Manager.ins.GameOver();
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (Manager.ins.Warning && !Manager.ins.Player.hiding)
            Manager.ins.GameOver();
    }
}
