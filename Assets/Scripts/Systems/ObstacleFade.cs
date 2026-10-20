using System.Collections.Generic;
using UnityEngine;

public class ObstacleFade : MonoBehaviour
{
    public Transform player;
    public LayerMask obstacleMask;

    private List<Renderer> fadedObjects = new();

    void LateUpdate()
    {
        RestoreObjects();

        Vector3 direction =
            player.position - transform.position;

        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(
            transform.position,
            direction.normalized,
            distance,
            obstacleMask
        );

        foreach (RaycastHit hit in hits)
        {
            Renderer rend =
                hit.collider.GetComponent<Renderer>();

            if (rend != null)
            {
                fadedObjects.Add(rend);

                foreach (Material mat in rend.materials)
                {
                    Color color = mat.color;
                    color.a = 0.3f;
                    mat.color = color;

                    SetMaterialTransparent(mat);
                }
            }
        }
    }

    void RestoreObjects()
    {
        foreach (Renderer rend in fadedObjects)
        {
            if (rend == null) continue;

            foreach (Material mat in rend.materials)
            {
                Color color = mat.color;
                color.a = 1f;
                mat.color = color;
            }
        }

        fadedObjects.Clear();
    }

    void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend",
            (int)UnityEngine.Rendering.BlendMode.SrcAlpha);

        mat.SetInt("_DstBlend",
            (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        mat.SetInt("_ZWrite", 0);

        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        mat.renderQueue = 3000;
    }
}