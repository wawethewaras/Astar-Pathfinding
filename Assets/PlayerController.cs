using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rigidbody;
    public float moveSpeed;


	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody2D>();

    }
	
	// Update is called once per frame
	void Update () {
        Vector2 movement_Vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        movement_Vector = movement_Vector.normalized * moveSpeed;
        rigidbody.velocity = movement_Vector;

        Vector2 mouse_pos = Input.mousePosition;
        Vector2 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
