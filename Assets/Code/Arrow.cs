using UnityEngine;

public class Arrow : MonoBehaviour
{
    public string Team;
    private Rigidbody2D rb;
    public bool Active;
    public int DMG;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Fire(string team)
    {
        Team = team;
        rb.simulated = true;
        rb.position = rb.transform.position + transform.right * 3;
        transform.SetParent(null);
        Active = true;
        rb.AddForce(250 * transform.right);
        Destroy(gameObject, 5);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Active)
        {
            GameObject Hit = collision.gameObject;
            try
            {
                Damageable Victim = Hit.GetComponent<Damageable>();
                while (Victim == null && Hit.transform.parent != null)
                {
                    Hit = Hit.transform.parent.gameObject;
                    Victim = Hit.GetComponent<Damageable>();
                }
                if (Victim != null && Victim.Team != Team)
                {
                    //Debug.Log($"Arrow hit {Hit.name}");
                    Victim.TakeDamage(DMG);
                    Destroy(gameObject);
                }
            }
            catch(System.NullReferenceException)
            {

            }
            catch(UnityEngine.MissingReferenceException)
            {

            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
}
