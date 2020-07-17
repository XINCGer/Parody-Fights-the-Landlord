//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ParticleOrderAutoSorter : MonoBehaviour
{
    private int canvasOrder = 0;
    private Canvas canvas;
    private Dictionary<Renderer, int> particleOffsetDic = new Dictionary<Renderer, int>();

    void Awake()
    {
        canvasOrder = GetComponent<Canvas>().sortingOrder;
    }

    void Start()
    {
        ReGetCompenent();
    }

    public void UpdateSortingOrder()
    {
        if (particleOffsetDic.Count == 0)
        {
            return;
        }
        using (var enumerator = particleOffsetDic.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Key != null)
                {
                    enumerator.Current.Key.sortingOrder = canvasOrder + enumerator.Current.Value;
                }
            }
        }
    }

    public void ReGetCompenent()
    {
        canvas = GetComponent<Canvas>();
        ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < particles.Length; i++)
        {
            var render = particles[i].GetComponent<Renderer>();
            particleOffsetDic[render] = render.sortingOrder - canvasOrder;
        }
        CheckUpdate(false);
    }

    public void AddComponent(ParticleSystem particle)
    {
        if (particle == null)
        {
            return;
        }
        var render = particle.GetComponent<Renderer>();
        particleOffsetDic[render] = render.sortingOrder - canvasOrder;
    }

    public void RemoveConponent(ParticleSystem particle)
    {
        if (particle == null)
        {
            return;
        }
        var render = particle.GetComponent<Renderer>();
        particleOffsetDic.Remove(render);
    }

    private void CheckUpdate(bool force)
    {
        if (canvas != null && (canvas.sortingOrder != canvasOrder || force))
        {
            canvasOrder = canvas.sortingOrder;
            UpdateSortingOrder();
        }
    }

    void Update()
    {
        CheckUpdate(false);
    }
}
