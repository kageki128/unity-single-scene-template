using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyProject.Actor
{
    /// <summary>
    /// マウスポインターのイベントを観測し、各種イベントを発行するクラス。
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PointerEventObserver : MonoBehaviour,
        IPointerClickHandler,
        IScrollHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IInitializePotentialDragHandler,
        IPointerMoveHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler
    {
        /// <summary>
        /// ポインターがクリックされたときに発行されます。
        /// </summary>
        public Observable<PointerEventData> Clicked => clicked;
        readonly Subject<PointerEventData> clicked = new();

        /// <summary>
        /// マウスホイールでスクロールされたときに発行されます。
        /// </summary>
        public Observable<PointerEventData> Scrolled => scrolled;
        readonly Subject<PointerEventData> scrolled = new();

        /// <summary>
        /// ポインターがこのオブジェクトの領域に入ったときに発行されます。
        /// </summary>
        public Observable<PointerEventData> PointerEntered => pointerEntered;
        readonly Subject<PointerEventData> pointerEntered = new();

        /// <summary>
        /// ポインターがこのオブジェクトの領域から出たときに発行されます。
        /// </summary>
        public Observable<PointerEventData> PointerExited => pointerExited;
        readonly Subject<PointerEventData> pointerExited = new();

        /// <summary>
        /// ポインター押下が開始されたときに発行されます。
        /// </summary>
        public Observable<PointerEventData> PointerDown => pointerDown;
        readonly Subject<PointerEventData> pointerDown = new();

        /// <summary>
        /// ポインター押下が解除されたときに発行されます。
        /// </summary>
        public Observable<PointerEventData> PointerUp => pointerUp;
        readonly Subject<PointerEventData> pointerUp = new();

        /// <summary>
        /// ドラッグ初期化時に発行されます。
        /// </summary>
        public Observable<PointerEventData> InitializePotentialDrag => initializePotentialDrag;
        readonly Subject<PointerEventData> initializePotentialDrag = new();

        /// <summary>
        /// ポインター移動時に発行されます。
        /// </summary>
        public Observable<PointerEventData> PointerMoved => pointerMoved;
        readonly Subject<PointerEventData> pointerMoved = new();

        /// <summary>
        /// ドラッグ開始時に発行されます。
        /// </summary>
        public Observable<PointerEventData> BeginDrag => beginDrag;
        readonly Subject<PointerEventData> beginDrag = new();

        /// <summary>
        /// ドラッグ中に継続して発行されます。
        /// </summary>
        public Observable<PointerEventData> Dragged => dragged;
        readonly Subject<PointerEventData> dragged = new();

        /// <summary>
        /// ドラッグ終了時に発行されます。
        /// </summary>
        public Observable<PointerEventData> EndDrag => endDrag;
        readonly Subject<PointerEventData> endDrag = new();

        /// <summary>
        /// ドロップされたときに発行されます。
        /// </summary>
        public Observable<PointerEventData> Dropped => dropped;
        readonly Subject<PointerEventData> dropped = new();

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            clicked.OnNext(eventData);
        }

        void IScrollHandler.OnScroll(PointerEventData eventData)
        {
            scrolled.OnNext(eventData);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            pointerEntered.OnNext(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            pointerExited.OnNext(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            pointerDown.OnNext(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            pointerUp.OnNext(eventData);
        }

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            initializePotentialDrag.OnNext(eventData);
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            pointerMoved.OnNext(eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            beginDrag.OnNext(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            dragged.OnNext(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            endDrag.OnNext(eventData);
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            dropped.OnNext(eventData);
        }

        void OnDestroy()
        {
            clicked.OnCompleted();
            clicked.Dispose();

            scrolled.OnCompleted();
            scrolled.Dispose();

            pointerEntered.OnCompleted();
            pointerEntered.Dispose();

            pointerExited.OnCompleted();
            pointerExited.Dispose();

            pointerDown.OnCompleted();
            pointerDown.Dispose();

            pointerUp.OnCompleted();
            pointerUp.Dispose();

            initializePotentialDrag.OnCompleted();
            initializePotentialDrag.Dispose();

            pointerMoved.OnCompleted();
            pointerMoved.Dispose();

            beginDrag.OnCompleted();
            beginDrag.Dispose();

            dragged.OnCompleted();
            dragged.Dispose();

            endDrag.OnCompleted();
            endDrag.Dispose();

            dropped.OnCompleted();
            dropped.Dispose();
        }
    }
}
