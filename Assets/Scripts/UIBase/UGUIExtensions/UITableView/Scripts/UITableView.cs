//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using ColaFramework;
using Sirenix.OdinInspector;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Cell的对齐方式
    /// </summary>
    public enum CellAlign
    {
        LeftOrTop,
        Center
    }

    /// <summary>
    /// 可见区域的范围
    /// </summary>
    public struct VisibleRange
    {
        public int from;
        public int to;
    }

    /// <summary>
    /// 高度定制可重用Cell的无限滚动UITableview组件
    /// </summary>
    public class UITableView : MonoBehaviour, IControl
    {
        #region 内部字段
        private ScrollRect scroll;
        private bool isHorizontal;
        private Vector3 scaleFactor;
        private GridLayoutGroup gridConfig;
        private RectOffset padding;
        private Vector2 cellSize;
        private Vector2 spacing;
        private int numPerRowOrCol;
        private RectTransform contentRT;
        private RectTransform viewRT;
        private CellAlign cellAlign;
        private int cellTotal;
        private VisibleRange curVisibleRange;
        private VisibleRange newVisibleRange;
        private float cellAlignOffset = 0f;
        private float lastDirtyRate = 1f;//上一次Dirty＝true 需要刷新的 Content Rate
        private bool isInited = false;
        private bool isDirty = false;
        private bool isNeedReload = false;
        private bool isOnShowing = false;
        private bool isOnUsing = true;
        private bool isOverflow = false;//全部cell是否超过显示区域
        private bool isIncreased = true;//Scroll方向是否是 前进方向
        private float scrollMinDis = 20;//Scroll滑动不刷新的最小距离
        private float scrollMinRate = 0;//根据滑动不刷新最小距离计算的 min scroll rate
        private float viewStartPos = 0;
        private float viewEndPos = 0f;

        [SerializeField]
        [LabelText("是否循环")]
        private bool loop;

        private List<UITableViewCell> reUseCells = new List<UITableViewCell>();
        private List<UITableViewCell> inUseCells = new List<UITableViewCell>();
        private List<Vector2> cellPosList = new List<Vector2>();

        private List<int> reUseTags = new List<int>();
        private List<int> newInUseTags = new List<int>();
        #endregion

        #region 属性
        [LabelText("Cell数量")]
        public int CellCount;

        [LabelText("TableviewCell")]
        [SerializeField]
        private GameObject CellPrefab;

        public delegate void OnCellInitEvent(UITableView tableView, UITableViewCell cell);
        public OnCellInitEvent onCellInit;

        public delegate void OnProcessClick(UITableViewCell tableViewCell, GameObject gameObject);
        public OnProcessClick onProcessClick;

        public delegate void OnProcessPress(bool isPressDown, UITableViewCell tableViewCell, GameObject gameObject);
        public OnProcessPress onProcessPress;

        /// <summary>
        /// 当滚动停止的时候，返回当前cell对应的索引
        /// </summary>
        /// <param name="table"></param>
        /// <param name="index"></param>
        public delegate void OnScrollCompleted(UITableView table, int index);
        public OnScrollCompleted onScrollCompleted;

        /// <summary>
        /// UITableView在滚动的时候会调用
        /// </summary>
        /// <param name="value"></param>
        public delegate void OnTableScrolling(float value);
        public OnTableScrolling onTableScrolling;

        public bool Loop
        {
            get { return this.loop; }
            set { this.loop = value; }
        }
        #endregion

        public void Init()
        {
            if (this.isInited)
                return;
            if (this.scroll == null)
                this.scroll = GetComponent<ScrollRect>();
            this.scroll.onValueChanged.AddListener(OnScrolling);

            this.isHorizontal = this.scroll.horizontal;
            this.scaleFactor = this.scroll.content.localScale;
            this.gridConfig = this.scroll.content.transform.GetComponent<GridLayoutGroup>();
            this.padding = gridConfig.padding;
            this.cellSize = gridConfig.cellSize;
            this.spacing = gridConfig.spacing;
            this.numPerRowOrCol = gridConfig.constraintCount;
            gridConfig.enabled = false;
            this.contentRT = this.scroll.content;
            this.viewRT = this.scroll.viewport;
            this.cellAlign = gridConfig.childAlignment == 0 ? CellAlign.LeftOrTop : CellAlign.Center;
            this.cellTotal = 0;
            this.curVisibleRange = new VisibleRange() { from = 0, to = -1 };
            this.newVisibleRange = new VisibleRange() { from = 0, to = -1 };
            this.CellPrefab.SetActive(false);
            this.isInited = true;
        }

        void OnDestroy()
        {
            this.isOnUsing = false;

            if (this.scroll != null)
                this.scroll.onValueChanged.RemoveListener(OnScrolling);

            onScrollCompleted = null;
            onTableScrolling = null;
            onCellInit = null;
        }

        /// <summary>
        /// 设置滑动速度
        /// </summary>
        /// <param name="minDis"></param>
        public void SetScrollMinDis(float minDis)
        {
            this.scrollMinDis = minDis;
        }

        /// <summary>
        /// 从可复用列表里取cell,没有就创建新的cell
        /// </summary>
        /// <returns></returns>
        private UITableViewCell GetReUseCell()
        {
            if (reUseCells.Count == 0) return null;
            UITableViewCell cell = reUseCells[0];
            reUseCells.RemoveAt(0);
            return cell;
        }

        public UITableViewCell GetCellWithIndex(int index)
        {
            return this.inUseCells[index];
        }

        /// <summary>
        /// 刷新TableView
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="stayThere"></param>
        public void Reload(int totalCount, bool stayThere = true)
        {
            Init();
            this.cellTotal = totalCount;
            this.isNeedReload = true;
            this.isOnUsing = true;
            if (!stayThere)
                this.SetToTop();
        }


        /// <summary>
        /// 刷新TableView
        /// </summary>
        /// <param name="stayThere"></param>
        public void Reload(bool stayThere = true)
        {
            Init();
            this.isNeedReload = true;
            this.isOnUsing = true;
            if (!stayThere)
                this.SetToTop();
        }


        public void SetToTop()
        {
            Vector2 pos = this.contentRT.anchoredPosition;
            if (this.isHorizontal)
                pos.x = 0;
            else
                pos.y = 0;
            this.contentRT.anchoredPosition = pos;
            this.isIncreased = false;
            this.CalcViewRectPos();
            this.isDirty = true;
        }


        public void SetToBottom()
        {
            Vector2 pos = this.contentRT.anchoredPosition;
            if (this.isHorizontal)
                pos.x = -(this.contentRT.rect.width - this.viewRT.rect.width);
            else
                pos.y = this.contentRT.rect.height - this.viewRT.rect.height;
            this.contentRT.anchoredPosition = pos;
            this.isIncreased = true;
            this.CalcViewRectPos();
            this.isDirty = true;
        }

        internal void ProcessClick(UITableViewCell tableViewCell, GameObject targetObj)
        {
            if (null != onProcessClick)
            {
                onProcessClick(tableViewCell, targetObj);
            }
        }

        internal void ProcessPress(bool isPressDown, UITableViewCell tableViewCell, GameObject targetObj)
        {
            if (null != onProcessPress)
            {
                onProcessPress(isPressDown, tableViewCell, targetObj);
            }
        }

        private void Clear()
        {
            this.cellTotal = 0;
            this.curVisibleRange.from = 0; this.curVisibleRange.to = -1;
            this.newVisibleRange.from = 0; this.newVisibleRange.to = -1;
            this.reUseCells.Clear();
            this.inUseCells.Clear();
            this.isOnUsing = false;
        }

        public void GoTo(int index)
        {
            this.InitTableData();
            Vector2 pos = this.CalcCellPosition(index);
            if (this.isHorizontal)
            {
                this.isIncreased = (-pos.x < this.contentRT.anchoredPosition.x);
                this.contentRT.anchoredPosition = new Vector2(-pos.x, 0);
            }
            else
            {
                this.isIncreased = (-pos.y > this.contentRT.anchoredPosition.y);
                this.contentRT.anchoredPosition = new Vector2(0, -pos.y);
            }
            this.CalcViewRectPos();
            this.isDirty = true;
        }

        private void Reload()
        {
            InitTableData();
            this.isNeedReload = false;
            this.isDirty = true;
        }


        private void InitTableData()
        {
            Reset();
            this.cellTotal = CellCount;
            if (this.numPerRowOrCol == 0)
            {
                this.numPerRowOrCol = 1;
            }
            this.ResetContent();
            this.cellAlignOffset = this.CalcCellAlignOffset(this.isHorizontal, this.cellAlign);
            this.CalcAllCellPos();
            this.CalcViewRectPos();

        }

        private void Reset()
        {
            //this.cellTotal = 0;
            this.curVisibleRange.from = 0; this.curVisibleRange.to = -1;
            this.newVisibleRange.from = 0; this.newVisibleRange.to = -1;
            for (int i = 0; i < this.inUseCells.Count; i++)
            {
                UITableViewCell cell = this.inUseCells[i];
                this.HideCell(cell);
                this.reUseCells.Add(cell);
            }
            this.inUseCells.Clear();
        }

        void HideCell(UITableViewCell cell)
        {
            if (cell != null)
                cell.cacheTransform.anchoredPosition = new Vector2(1000, 1000);
        }

        void LateUpdate()
        {
            if (this.isOnShowing) return;
            if (this.isNeedReload) this.Reload();
            if (this.isDirty)
            {
                this.newVisibleRange = this.CalcVisibleRange();
                if (this.newVisibleRange.to - this.newVisibleRange.from >= 0) this.RefreshVisibleCell();
                this.isDirty = false;
            }
        }


        void RefreshVisibleCell()
        {
            if (this.curVisibleRange.from == this.newVisibleRange.from && this.curVisibleRange.to == this.newVisibleRange.to)
                return;
            int curFrom = this.curVisibleRange.from;
            int curTo = this.curVisibleRange.to;
            int newFrom = this.newVisibleRange.from;
            int newTo = this.newVisibleRange.to;

            reUseTags.Clear();
            newInUseTags.Clear();

            for (int i = curFrom; i <= curTo; i++)
            {
                if (i < newFrom || i > newTo)
                {
                    reUseTags.Add(i);
                }
            }
            for (int i = newFrom; i <= newTo; i++)
            {
                if (i < curFrom || i > curTo)
                {
                    newInUseTags.Add(i);
                }
            }

            this.CollectReUseCells(reUseTags);
            this.ShowNewCells(newInUseTags);
            this.curVisibleRange.from = this.newVisibleRange.from;
            this.curVisibleRange.to = this.newVisibleRange.to;

        }

        void CollectReUseCells(List<int> _reUseTags)
        {
            for (int i = 0; i < _reUseTags.Count; i++)
            {
                UITableViewCell cell = this.inUseCells[_reUseTags[i]];
                if (cell != null)
                {
                    this.reUseCells.Add(cell);
                    this.inUseCells[_reUseTags[i]] = null;
                    this.HideCell(cell);
                }
            }
        }

        void ShowNewCells(List<int> newUseTag)
        {
            if (CellPrefab == null)
                return;
            if (this.isOnShowing) return;
            for (int i = 0; i < newUseTag.Count; i++)
            {
                this.ShowCellByTag(newUseTag[i]);
                this.isOnShowing = false;
            }
        }

        private UITableViewCell GetOneTableviewCell(int index)
        {
            UITableViewCell cell = GetReUseCell();
            if (cell == null)
            {
                GameObject go = Instantiate(CellPrefab) as GameObject;
                cell = go.AddSingleComponent<UITableViewCell>();
                if (!go.activeSelf)
                {
                    go.SetActive(true);
                }
            }
            cell.index = index;
            cell.tableView = this;
            cell.tableViewCell = cell;
            return cell;
        }

        void ShowCellByTag(int tag)
        {
            UITableViewCell cell = GetOneTableviewCell(tag);

            if (cell != null)
            {

                Vector2 pos = this.cellPosList[tag];

                if (cell.cacheTransform.parent != this.contentRT)
                {
                    cell.cacheTransform.SetParent(this.scroll.content, false);

                    cell.cacheTransform.anchorMin = new Vector2(0, 1);
                    cell.cacheTransform.anchorMax = new Vector2(0, 1);
                    cell.cacheTransform.pivot = new Vector2(0, 1);
                    cell.cacheTransform.localScale = new Vector3(1, 1, 1);
                    cell.cacheTransform.sizeDelta = new Vector2(this.cellSize.x, this.cellSize.y);
                }

                cell.cacheTransform.anchoredPosition = pos;
                if (tag < this.inUseCells.Count)
                {
                    this.inUseCells[tag] = cell;
                }
                else
                {
                    this.inUseCells.Add(cell);
                }

                if (null != onCellInit)
                {
                    onCellInit(this, cell);
                }
            }
            else
            {
                Debug.Log("cell by index cell == null" + tag);
            }

        }


        void ResetContent()
        {
            float contentWidth = this.scroll.viewport.rect.width;
            float contentHeight = this.scroll.viewport.rect.height;
            float contentOffset = 0;
            if (this.isHorizontal)
            {
                float width = Mathf.Ceil(this.cellTotal * 1.0f / this.numPerRowOrCol) * (this.cellSize.x + this.spacing.x) + this.padding.left + this.padding.right;
                if (width > contentWidth)
                {
                    contentOffset = width - contentWidth;
                    contentWidth = width;
                    this.isOverflow = true;
                }
                else
                    this.isOverflow = false;
            }
            else
            {
                float height = Mathf.Ceil(this.cellTotal * 1.0f / this.numPerRowOrCol) * (this.cellSize.y + this.spacing.y) + this.padding.top + this.padding.bottom;
                if (height > contentHeight)
                {
                    contentOffset = height - contentHeight;
                    contentHeight = height;
                    this.isOverflow = true;
                }
                else
                {
                    this.isOverflow = false;
                }
            }
            this.scrollMinRate = this.scrollMinDis / contentOffset;
            this.scroll.content.sizeDelta = new Vector2(contentWidth, contentHeight);
        }

        /// <summary>
        /// 计算可见cell范围
        /// </summary>
        /// <returns></returns>
        VisibleRange CalcVisibleRange()
        {
            if ((this.cellSize.x < 0.01f) || (this.cellSize.y < 0.01f) || (this.cellTotal == 0))
            {
                return new VisibleRange() { from = 0, to = -1 };
            }
            float startPos = this.viewStartPos;
            float endPos = this.viewEndPos;
            int startTag = 0;
            int endTag = -1;

            int tagTemp = 0;
            Vector2 pos;
            if (this.isHorizontal)
            {
                if (this.curVisibleRange.from == 0) this.isIncreased = true;
                if (this.isIncreased)
                {
                    for (int i = this.curVisibleRange.from; i < this.cellTotal; i++)
                    {
                        pos = this.cellPosList[i];
                        if (pos.x + this.cellSize.x > startPos)
                        {
                            startTag = i;
                            break;
                        }

                    }
                }
                else
                {
                    for (int i = this.curVisibleRange.from; i >= 0; i--)//??
                    {
                        pos = this.cellPosList[i];
                        if (pos.x + this.cellSize.x > startPos)
                        {
                            startTag = i;
                        }
                        else
                            break;
                    }
                }

                if (this.curVisibleRange.to == -1)
                {
                    tagTemp = startTag;
                    this.isIncreased = true;
                }
                else
                {
                    tagTemp = this.curVisibleRange.to;
                }

                if (this.isIncreased)
                {
                    for (int i = tagTemp; i < this.cellTotal; i++)
                    {
                        pos = this.cellPosList[i];
                        if (pos.x < endPos)
                        {
                            endTag = i;
                        }
                        else
                            break;

                    }
                }
                else
                {
                    for (int i = tagTemp; i >= 0; i--)
                    {
                        pos = this.cellPosList[i];
                        if (pos.x <= endPos)
                        {
                            endTag = i;
                            break;
                        }
                    }
                }

            }
            else//垂直滑动
            {
                if (this.curVisibleRange.from == 0) this.isIncreased = true;
                if (this.isIncreased)
                {
                    for (int i = this.curVisibleRange.from; i < this.cellTotal; i++)
                    {
                        pos = this.cellPosList[i];
                        if (pos.y - this.cellSize.y < startPos - 0.01f)
                        {
                            startTag = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = this.curVisibleRange.from; i >= 0; i--)
                    {
                        pos = this.cellPosList[i];
                        if (pos.y - this.cellSize.y < startPos)
                        {
                            startTag = i;
                        }
                        else
                        {
                            break;

                        }

                    }
                }

                if (this.curVisibleRange.to == -1)
                {
                    tagTemp = startTag; this.isIncreased = true;
                }
                else
                {
                    tagTemp = this.curVisibleRange.to;
                }
                if (this.isIncreased)
                {
                    for (int i = tagTemp; i < this.cellTotal; i++)
                    {
                        pos = this.cellPosList[i];
                        if (pos.y - 0.01f > endPos)
                        {
                            endTag = i;

                        }
                        else
                            break;

                    }
                }
                else
                {
                    for (int i = tagTemp; i >= 0; i--)
                    {
                        pos = this.cellPosList[i];
                        if (pos.y - 0.01f > endPos)
                        {
                            endTag = i;
                            break;
                        }
                    }
                }


            }
            //Debug.Log("visibleRange: " + startTag + " " + endTag);
            return new VisibleRange() { from = startTag, to = endTag };
        }

        /// <summary>
        /// 计算可视区域
        /// </summary>
        void CalcViewRectPos()
        {
            if (this.isHorizontal)
            {
                this.viewStartPos = -(this.contentRT.anchoredPosition.x) / this.scaleFactor.x;
                this.viewEndPos = (this.viewStartPos + this.viewRT.rect.width / this.scaleFactor.y);
            }
            else
            {
                this.viewStartPos = -(this.contentRT.anchoredPosition.y) / this.scaleFactor.y;
                this.viewEndPos = (this.viewStartPos - this.viewRT.rect.height / this.scaleFactor.y);
            }
        }


        bool IsNeedDirtyByScrolling()
        {
            //Debug.Log("IsNeedDirtyByScrolling viisblerange: " + this.curVisibleRange.from + " " + this.curVisibleRange.to);
            if (this.isIncreased)
            {
                if (this.curVisibleRange.to + 1 == this.cellTotal) return false;
                if (this.isHorizontal)
                {
                    if (this.cellPosList[this.curVisibleRange.to].x < this.viewEndPos) return true; else return false;
                }
                else
                {
                    if (this.cellPosList[this.curVisibleRange.to].y > this.viewEndPos) return true; else return false;
                }
            }
            else
            {
                //Debug.Log("IsNeedDirtyByScrolling cell pos: " + this.cellPosList[this.curVisibleRange.from].y + " " + this.cellSize.y + " " + this.viewStartPos);
                if (this.curVisibleRange.from == 0) return false;
                if (this.isHorizontal)
                    if (this.cellPosList[this.curVisibleRange.from].x + this.cellSize.x > this.viewStartPos) return true; else return false;
                else
                    if (this.cellPosList[this.curVisibleRange.from].y - this.cellSize.y < this.viewStartPos) return true; else return false;
            }
        }

        void CalcAllCellPos()
        {
            this.cellPosList.Clear();
            for (int i = 0; i < this.cellTotal; i++)
            {
                this.cellPosList.Add(this.CalcCellPosition(i));
            }
        }


        Vector2 CalcCellPosition(int tag)
        {
            float x = 0;
            float y = 0;
            if (this.isHorizontal)
            {
                x = this.padding.left + (Mathf.Ceil((tag + 1) * 1.0f / this.numPerRowOrCol) - 1) * (this.cellSize.x + this.spacing.x);
                y = -(this.cellAlignOffset + this.padding.top + (tag % this.numPerRowOrCol) * (this.cellSize.y + this.spacing.y));
            }
            else
            {
                x = (this.cellAlignOffset + this.padding.left + (tag % this.numPerRowOrCol) * (this.cellSize.x + this.spacing.x));
                y = -(this.padding.top + (Mathf.Ceil((tag + 1) * 1.0f / this.numPerRowOrCol) - 1) * (this.cellSize.y + this.spacing.y));
            }
            return new Vector2(x, y);
        }


        float CalcCellAlignOffset(bool isHorizontal, CellAlign align)
        {
            float offset = 0f, cellHold = 0f, contentHold = 0f;
            if (isHorizontal)
            {
                cellHold = this.padding.top + this.cellSize.y * this.numPerRowOrCol + this.spacing.y * (this.numPerRowOrCol - 1) + this.padding.bottom;
                contentHold = this.contentRT.rect.height;
            }
            else
            {
                cellHold = this.padding.left + this.cellSize.x * this.numPerRowOrCol + this.spacing.x * (this.numPerRowOrCol - 1) + this.padding.right;
                contentHold = this.contentRT.rect.width;
            }
            if (this.cellAlign == CellAlign.LeftOrTop)
                offset = 0;
            else if (this.cellAlign == CellAlign.Center)
            {
                offset = (contentHold - cellHold) / 2f;
            }
            else
            {
                Debug.LogError("UITableView CellAlign wrong: " + this.cellAlign);
            }
            return Mathf.Max(offset, 0f);
        }

        #region callback
        void OnScrolling(Vector2 scrollRate)
        {
            if (this.isDirty) return;
            if (this.isOnShowing) return;
            if (this.isOverflow)
            {
                if (this.isHorizontal)
                {
                    if (Mathf.Abs(this.lastDirtyRate - scrollRate.x) < this.scrollMinRate) return;
                    this.isIncreased = this.lastDirtyRate < scrollRate.x;
                    this.lastDirtyRate = scrollRate.x;

                }
                else
                {
                    if (Mathf.Abs(this.lastDirtyRate - scrollRate.y) < this.scrollMinRate) return;
                    this.isIncreased = this.lastDirtyRate >= scrollRate.y;
                    this.lastDirtyRate = scrollRate.y;
                }
                this.CalcViewRectPos();
                this.isDirty = this.IsNeedDirtyByScrolling();
            }
        }
        #endregion
    }
}

