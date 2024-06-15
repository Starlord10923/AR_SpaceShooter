using System.Collections;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public float health = 4f;

    private Vector3 startPosition;

    void Start()
    {
        if(CompareTag("Enemy")){
            startPosition = transform.position;
            StartCoroutine(Levitate());
        }
    }

    IEnumerator Levitate()
    {
        float levitationHeight = 1f;
        float levitationSpeed = 1f;

        if(name=="StarDestroyer"){
            levitationHeight = 0.5f;
            levitationSpeed = 1.5f;
        }

        float elapsedTime = 0f;

        while (true)
        {
            float newY = startPosition.y + Mathf.Sin(elapsedTime * levitationSpeed) * levitationHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log(name + " Taken Damage " + damage);
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject, 0.1f);
        }
    }
}
