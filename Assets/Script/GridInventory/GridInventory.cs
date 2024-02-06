using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GridInventory
{
    class GridInventory : MonoBehaviour
    {
        [SerializeField] private Image m_select;
        [SerializeField] private GridInventoryPart[] m_parts;

        public const int
            GRID_SIZE = 48,
            GRID_HALF = 24;

        private GridInventoryPart m_target;

        private RectTransform m_rect;

        private void OnEnable()
        {
            InputManager.MouseMove += OnMove;
            InputManager.MouseClick += OnClick;
        }
        private void OnDisable()
        {
            InputManager.MouseMove -= OnMove;
            InputManager.MouseClick -= OnClick;
        }

        private void Start()
        {
            m_rect = m_select.transform as RectTransform;
            foreach(var part in m_parts)
            {
                part.EventEnter += OnEnter;
                part.EventExit += OnExit;
            }

            void OnEnter(GridInventoryPart part_)
            {
                m_target = part_;
            }
            void OnExit(GridInventoryPart part_)
            {
                if (m_target == part_)
                {
                    m_target = null;
                }
            }
        }

        private void OnMove(Vector2 pos_)
        {
            if (m_target == null)
            {
                return;
            }
            var _target = m_target;
            var _rect = _target.Rect;
            var _size = _target.Size;
            var _origin = new Vector2(_rect.position.x - _rect.sizeDelta.x / 2.0f, _rect.position.y + _rect.sizeDelta.y / 2.0f);
            var _select = pos_ - _origin;
            var _index = new Vector2Int((int)(_select.x / GRID_SIZE % _size.x), (int)(_select.y / GRID_SIZE % _size.y));
            var _pos = _index * GRID_SIZE;
            var _offset = new Vector2(-_size.x * GRID_HALF + GRID_HALF, _size.y * GRID_HALF - GRID_HALF) + new Vector2(_rect.position.x, _rect.position.y);

            m_rect.anchoredPosition = _pos + _offset;
        }

        private void OnClick()
        {
            if (m_target == null)
            {
                return;
            }
        }
    }
}