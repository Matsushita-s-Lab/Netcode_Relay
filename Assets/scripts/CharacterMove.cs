using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : Unity.Netcode.NetworkBehaviour
{
    private Animator animator;
    private float speed = 8.0f;
    private Vector2 moveInput;
    // Start is called before the first frame update
    void Start()
    {
        this.animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.IsOwner)
        {
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");
            SetMoveInputServerRpc(inputX, inputY);
        }    
        
        if (this.IsServer)
        {
            Move();

            //ïúãAèàóù
            if (transform.position.y < -10.0f)
            {
                transform.position = new Vector3(Random.Range(-5, 5), 5, Random.Range(-5, 5));
            }
        }   
    }
    [Unity.Netcode.ServerRpc]
    private void SetMoveInputServerRpc(float x, float y)
    {
        this.moveInput = new Vector2(x, y);
    }

    private void Move()
    {
        var delta = new Vector3(moveInput.x, 0, moveInput.y);
        //å¸Ç´ïœçX
        if (delta.magnitude > 0.01f)
        {
            transform.LookAt(transform.position + delta);
        }
        //à⁄ìÆ
        transform.position += delta * Time.deltaTime * speed;
        //Animation
        animator.SetFloat("Speed", delta.magnitude);
    }
}
