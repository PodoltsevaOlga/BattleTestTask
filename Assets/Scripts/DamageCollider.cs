using UnityEngine;

using ActionCollider = System.Action<UnityEngine.Collider2D, UnityEngine.Collider2D>;

[RequireComponent(typeof(Collider2D))]
public class DamageCollider : MonoBehaviour
{
    private Collider2D _collider2D;
    public ActionCollider TriggerEnter { get; set; }
    public ActionCollider TriggerStay { get; set; }
    public ActionCollider TriggerExit { get; set; }

    void Start()
    {
        _collider2D = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerEnter?.Invoke(_collider2D, collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TriggerStay?.Invoke(_collider2D, collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TriggerExit?.Invoke(_collider2D, collision);
    }
}
