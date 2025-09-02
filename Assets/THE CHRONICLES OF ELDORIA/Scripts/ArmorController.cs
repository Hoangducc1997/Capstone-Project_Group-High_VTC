using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BodySpecialPart
{
    public string bodyPart;                     // Tên phần body (Head, UpperBody, Legs...)
    public SkinnedMeshRenderer bodyRenderer;    // Renderer của phần đó
}

public class ArmorController : MonoBehaviour
{
    public List<BodySpecialPart> BodyParts = new List<BodySpecialPart>();
    public List<SkinnedMeshRenderer> ArmorParts = new List<SkinnedMeshRenderer>();

    // --- Hàm mới: Ẩn phần thân dưới ---
    public void HideLowerBody()
    {
        if (BodyParts.Count == 0) return;

        foreach (var bodyPart in BodyParts)
        {
            if (bodyPart == null) continue;

            if (bodyPart.bodyPart == "Legs" || bodyPart.bodyPart == "LowerBody")
            {
                if (bodyPart.bodyRenderer != null)
                {
                    bodyPart.bodyRenderer.enabled = false;
                }
            }
        }
    }

    // --- Hàm mới: Hiện phần thân dưới ---
    public void ShowLowerBody()
    {
        if (BodyParts.Count == 0) return;

        foreach (var bodyPart in BodyParts)
        {
            if (bodyPart == null) continue;

            if (bodyPart.bodyPart == "Legs" || bodyPart.bodyPart == "LowerBody")
            {
                if (bodyPart.bodyRenderer != null)
                {
                    bodyPart.bodyRenderer.enabled = true;
                }
            }
        }
    }

    // --- Hàm mặc giáp ---
    public void ShowArmor(string slotType)
    {
        foreach (var armor in ArmorParts)
        {
            if (armor != null && armor.name.Contains(slotType))
            {
                armor.enabled = true;

                // Nếu là quần → ẩn body legs
                if (slotType == "Legs")
                {
                    HideLowerBody();
                }
            }
        }
    }

    // --- Hàm tháo giáp ---
    public void HideArmor(string slotType)
    {
        foreach (var armor in ArmorParts)
        {
            if (armor != null && armor.name.Contains(slotType))
            {
                armor.enabled = false;

                // Nếu là quần → hiện lại body legs
                if (slotType == "Legs")
                {
                    ShowLowerBody();
                }
            }
        }
    }
}
