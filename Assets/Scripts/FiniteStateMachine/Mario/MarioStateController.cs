using System;
using System.Collections;
using UnityEngine;

public class MarioStateController : StateController
{
    public PowerupType currentPowerupType = PowerupType.Default;
    public MarioState shouldBeNextState = MarioState.Default;

    private SpriteRenderer spriteRenderer;
    private Coroutine blinkCoroutine;

    public override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameRestart();
    }

    public void GameRestart()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
        
        isActive = true;
        
        currentPowerupType = PowerupType.Default;
        TransitionToState(startState);
    }

    public void SetPowerup(PowerupType i)
    {
        currentPowerupType = i;
    }

    public new void TransitionToState(State nextState)
    {
        base.TransitionToState(nextState);
    }

    public void SetRendererToFlicker()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        
        blinkCoroutine = StartCoroutine(BlinkSpriteRenderer());
    }

    private IEnumerator BlinkSpriteRenderer()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        float flickerInterval = 0.1f;
        
        while (string.Equals(currentState.name, "InvincibleSmallMario", StringComparison.OrdinalIgnoreCase))
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
        }
        spriteRenderer.enabled = true;
        blinkCoroutine = null;
    }
}