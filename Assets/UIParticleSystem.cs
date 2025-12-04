using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParticleSystem : MaskableGraphic
{
    [SerializeField] ParticleSystemRenderer particleSystemRenderer;
    [SerializeField] Camera bakeCamera;

    [SerializeField] Texture texture;

    public override Texture mainTexture => texture ?? base.mainTexture;

    private void Update()
    {
        SetVerticesDirty();
    }
    protected override void OnPopulateMesh(Mesh mesh)
    {
        mesh.Clear();
        if (particleSystemRenderer !=null && bakeCamera != null)
        {
            particleSystemRenderer.BakeMesh(mesh,bakeCamera);
        }
    }

}
