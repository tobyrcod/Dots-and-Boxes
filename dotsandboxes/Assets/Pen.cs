using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;

    [Space]

    [SerializeField] Sprite playerOneSprite;
    [SerializeField] Sprite playerTwoSprite;

    public void ChangePenSprite(bool isPlayerOne) {
        if (isPlayerOne) {
            sr.sprite = playerOneSprite;
        }
        else {
            sr.sprite = playerTwoSprite;
        }
    }
}
