using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Astar探索ロジッククラス
/// </summary>

public class AStarAlgorithm
{
    /// <summary>
    /// 検索当たりのコスト
    /// </summary>
    private int m_costStep = 1;
    /// <summary>
    /// 検索するマップの大きさ
    /// </summary>
    private Vector2Int m_size;
    /// <summary>
    /// ノードを検索する順番
    /// </summary>
    private Vector2Int[] m_searchOrder = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public AStarAlgorithm(Vector2Int size_)
    {
        m_size = size_;
    }
    public AStarAlgorithm(Vector2Int size_, int costStep_)
    {
        m_size = size_;
        m_costStep = costStep_;
    }
    public AStarAlgorithm(Vector2Int size_, Vector2Int[] searchOrder_)
    {
        m_size = size_;
        m_searchOrder = searchOrder_;
    }
    public AStarAlgorithm(Vector2Int size_, int costStep_, Vector2Int[] searchOrder_)
    {
        m_size = size_;
        m_costStep = costStep_;
        m_searchOrder = searchOrder_;
    }

    public List<Vector2Int> Search(Vector2Int start_, Vector2Int goal_)
    {
        var _path = new List<Vector2Int>();

        PathFinding(m_costStep, m_size, m_searchOrder, start_, goal_)?.GetPath(_path);
        _path.Reverse();

        return _path;
    }

    protected class Node
    {
        private enum State
        {
            Non,
            Open,
            Close
        }
        private State m_state;
        private Vector2Int m_position;
        private Node m_orizin;
        private int
            m_costActual,
            m_costEstimated,
            m_score;

        public Node(int cost_)
        {
            m_costActual = cost_;
        }
        public Node(int cost_, Node orizin_)
        {
            m_costActual = cost_;
            m_orizin = orizin_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start(Vector2Int start_, Vector2Int goal_, List<Node> opens_)
        {
            m_state = State.Open;
            m_position = start_;
            m_costEstimated = Mathf.Abs(goal_.x - start_.x + goal_.y - start_.y);
            m_score = m_costActual + m_costEstimated;

            opens_.Add(this);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Open(Vector2Int pos_, Vector2Int start_, Vector2Int goal_, List<Node> opens_)
        {
            if (m_state != State.Non) return false;

            m_state = State.Open;
            m_position = pos_;
            m_costEstimated = Mathf.Abs(goal_.x - start_.x + goal_.y - start_.y);
            m_score = m_costActual + m_costEstimated;

            opens_.Add(this);
            return pos_ == goal_;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Close(List<Node> opens)
        {
            m_state = State.Close;
            opens.Remove(this);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsBetter(Node comp_)
        {
            return this.m_score < comp_.m_score || this.m_score == comp_.m_score && this.m_costActual < comp_.m_costActual;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2Int Route(Vector2Int search_)
        {
            return m_position + search_;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetPath(List<Vector2Int> path_)
        {
            path_.Add(m_position);
            m_orizin?.GetPath(path_);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Node[][] GetNodeMap(Vector2Int size_)
    {
        var _map = new Node[size_.y][];
        for (int i = 0, len = size_.y; i < len; ++i)
        {
            _map[i] = new Node[size_.x];
        }
        return _map;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Node GetBaseNode(List<Node> opens_)
    {
        var _base = opens_[0];
        for (int i = 1, cnt = opens_.Count; i < cnt; ++i)
        {
            // スコアが小さい　もしくは同じだが実コスト(移動にかかったコスト)が低いのでより近い
            if (opens_[i].IsBetter(_base))
            {
                _base = opens_[i];
            }
        }
        return _base;
    }

    /// <summary>
    /// ある配列がある二次元配列の大きさを超えていないか
    /// </summary>
    /// <param name="size_"></param>
    /// <param name="point_"></param>
    /// <returns>配列外か否か</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckOverFlow(Vector2Int size_, Vector2Int point_)
    {
        return 0 <= point_.x && 0 <= point_.y && size_.x > point_.x && size_.y > point_.y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Node PathFinding(int costStep_, Vector2Int size_, Vector2Int[] searchOrder_, Vector2Int start_, Vector2Int goal_)
    {
        var _map = GetNodeMap(size_);

        var _opens = new List<Node>();
        var _costTotal = 0;

        _map[start_.y][start_.x] = new(_costTotal);
        _map[start_.y][start_.x].Start(start_, goal_, _opens);

        while (true)
        {
            _costTotal += costStep_;

            if (_opens.Count == 0)
            {
                return null;
            }

            // 開く原点を決める
            var _baseNode = GetBaseNode(_opens);

            // 原点から周りを検索
            foreach (var search in searchOrder_)
            {
                var _searchTarget = _baseNode.Route(search);
                if (CheckOverFlow(size_, _searchTarget))
                {
                    var _costSearch = _costTotal;

                    // もし検索に対して新たな重みを足す場合_costSearchに対して重みを足す

                    _map[_searchTarget.y][_searchTarget.x] ??= new(_costSearch, _baseNode);

                    if (_map[_searchTarget.y][_searchTarget.x].Open(_searchTarget, start_, goal_, _opens))// 現在空いているノードのリスト
                    {
                        return _map[_searchTarget.y][_searchTarget.x];
                    }
                }
            }

            // 原点を閉じる
            _baseNode.Close(_opens);
        }
    }
}