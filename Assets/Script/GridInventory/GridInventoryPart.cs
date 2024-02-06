using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GridInventory
{
    public class GridInventoryPart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private int m_x = 1;
        [SerializeField] private int m_y = 1;

        public Vector2Int Size => new(m_x, m_y);

        private RectTransform m_rect;
        public RectTransform Rect
        {
            get
            {
                if (m_rect == null)
                {
                    m_rect = transform as RectTransform;
                }
                return m_rect;
            } 
        }
        public event Action<GridInventoryPart> EventEnter, EventExit;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData_) => EventEnter?.Invoke(this);
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData_) => EventExit?.Invoke(this);
#if UNITY_EDITOR
        private void OnValidate()
        {
            m_x = m_x < 1 ? 1 : m_x;
            m_y = m_y < 1 ? 1 : m_y;
            Rect.sizeDelta = new(m_x * GridInventory.GRID_SIZE, m_y * GridInventory.GRID_SIZE);
        }
#endif
    }
}