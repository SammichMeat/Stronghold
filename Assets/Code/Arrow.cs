using UnityEngine;

public class Arrow : MonoBehaviour
{
    public string Team;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Fire(string team)
    {
        Team = team;
        transform.SetParent(null);
        rb.AddForce(250 * transform.right);
    }
}
