using System;
using UnityEngine;
using UnityEngine.UI;

namespace AcceleracersCCG.Simulation
{
    /// <summary>
    /// Put this on your card rectangle prefab (an Image + Button). Optionally assign
    /// two child Text labels for the name and stat line. The GameBinder instantiates
    /// one of these per card/vehicle/realm and calls <see cref="Bind"/> to fill it in.
    ///
    /// Nothing here builds UI — you author the prefab's look; this just fills text,
    /// wires the click, and tints the background by state.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class CardView : MonoBehaviour
    {
        [Header("Optional references (auto-found if left empty)")]
        [SerializeField] private Image background;
        [SerializeField] private Button button;
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text statLabel;

        [Header("Tints")]
        [SerializeField] private Color normalColor = new Color(0.90f, 0.90f, 0.95f, 1f);
        [SerializeField] private Color dimColor = new Color(0.55f, 0.55f, 0.60f, 1f);
        [SerializeField] private Color highlightColor = new Color(1f, 0.85f, 0.35f, 1f);
        [SerializeField] private Color selectedColor = new Color(0.45f, 0.85f, 1f, 1f);

        private void Awake()
        {
            if (background == null) background = GetComponent<Image>();
            if (button == null) button = GetComponent<Button>();
        }

        public enum State { Normal, Dim, Highlight, Selected }

        public void Bind(string title, string stat, bool interactable, Action onClick, State state = State.Normal)
        {
            if (background == null) background = GetComponent<Image>();
            if (button == null) button = GetComponent<Button>();

            if (nameLabel != null) nameLabel.text = title;
            if (statLabel != null) statLabel.text = stat;

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.interactable = interactable;
                if (onClick != null) button.onClick.AddListener(() => onClick());
            }

            if (background != null)
            {
                switch (state)
                {
                    case State.Highlight: background.color = highlightColor; break;
                    case State.Selected: background.color = selectedColor; break;
                    case State.Dim: background.color = dimColor; break;
                    default: background.color = interactable ? normalColor : dimColor; break;
                }
            }
        }
    }
}
