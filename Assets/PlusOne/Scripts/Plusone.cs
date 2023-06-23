using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plusone : MonoBehaviour
{
    public Animator anim;
    [SerializeField] Text text;
    [SerializeField] Color minusColor;
    [SerializeField] Color plusColor;

    public void SendIt(string amount, bool isDamage)
    {
        text.text = amount;
        float destroyTimer = 1.5f;
        if (isDamage)
        {
            text.color = minusColor;
        }
        else
        {
            text.color = plusColor;
            transform.localScale *= 2f;
            anim.speed *= 0.5f;
            destroyTimer *= 2;
        }

        anim.Play("Send");
        Destroy(gameObject, destroyTimer);
    }
}
