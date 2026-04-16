using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject.Actor
{
    [DisallowMultipleComponent]
    public class ScrollBackgroundActor : ActorBase
    {
        [SerializeField] Image image;
        [SerializeField] float angleDegrees = 180f;
        [SerializeField] float speed = 80f;

        readonly List<Image> tiles = new();
        readonly List<Vector2> tileOffsets = new();
        readonly List<Image> generatedTiles = new();

        RectTransform imageRectTransform;
        Vector2 baseAnchoredPosition;
        Vector2 tileSize;
        Vector2 basisX;
        Vector2 basisY;
        Vector2 basisXUnit;
        Vector2 basisYUnit;
        Vector2 scrollOffset;
        int cachedScreenWidth;
        int cachedScreenHeight;
        Vector2 cachedTileSize;
        Quaternion cachedImageLocalRotation;
        RectTransform rootCanvasRectTransform;

        public override void Initialize()
        {
            imageRectTransform = image.rectTransform;
            baseAnchoredPosition = imageRectTransform.anchoredPosition;
            scrollOffset = Vector2.zero;
            rootCanvasRectTransform = GetRootCanvasRectTransform();

            RebuildTiles();
            CacheDimensions();
            ApplyTilePositions();

            gameObject.SetActive(false);
        }

        public override UniTask ShowAsync(CancellationToken ct)
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public override UniTask HideAsync(CancellationToken ct)
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        void Update()
        {
            if (ShouldRebuildTiles())
            {
                RebuildTiles();
                CacheDimensions();
            }

            var direction = GetMoveDirection();
            scrollOffset += direction * (speed * Time.deltaTime);

            var gridOffset = new Vector2(
                Vector2.Dot(scrollOffset, basisXUnit),
                Vector2.Dot(scrollOffset, basisYUnit));
            gridOffset = new Vector2(
                WrapOffset(gridOffset.x, tileSize.x),
                WrapOffset(gridOffset.y, tileSize.y));
            scrollOffset = (basisXUnit * gridOffset.x) + (basisYUnit * gridOffset.y);

            ApplyTilePositions();
        }

        void RebuildTiles()
        {
            for (var i = 0; i < generatedTiles.Count; i++)
            {
                Destroy(generatedTiles[i].gameObject);
            }

            generatedTiles.Clear();
            tiles.Clear();
            tileOffsets.Clear();

            Canvas.ForceUpdateCanvases();
            tileSize = imageRectTransform.rect.size;

            if (tileSize.x <= 0f || tileSize.y <= 0f)
            {
                throw new InvalidOperationException("ScrollBackgroundActor: Image の Rect サイズが 0 です。");
            }

            UpdateTileBasis();

            var viewSize = GetRootCanvasSize(rootCanvasRectTransform);
            var projectedViewSize = ProjectViewSizeToTileAxes(viewSize);
            var columns = Mathf.CeilToInt(projectedViewSize.x / tileSize.x) + 2;
            var rows = Mathf.CeilToInt(projectedViewSize.y / tileSize.y) + 2;

            var minX = -Mathf.FloorToInt(columns * 0.5f);
            var maxX = minX + columns - 1;
            var minY = -Mathf.FloorToInt(rows * 0.5f);
            var maxY = minY + rows - 1;

            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    var tile = x == 0 && y == 0
                        ? image
                        : Instantiate(image, imageRectTransform.parent);

                    if (x != 0 || y != 0)
                    {
                        generatedTiles.Add(tile);
                    }

                    var offset = (basisX * x) + (basisY * y);
                    tiles.Add(tile);
                    tileOffsets.Add(offset);
                }
            }
        }

        bool ShouldRebuildTiles()
        {
            if (cachedScreenWidth != Screen.width || cachedScreenHeight != Screen.height)
            {
                return true;
            }

            var currentTileSize = imageRectTransform.rect.size;
            if (cachedTileSize != currentTileSize)
            {
                return true;
            }

            var currentRotation = imageRectTransform.localRotation;
            return Quaternion.Angle(cachedImageLocalRotation, currentRotation) > 0.01f;
        }

        void CacheDimensions()
        {
            cachedScreenWidth = Screen.width;
            cachedScreenHeight = Screen.height;
            cachedTileSize = tileSize;
            cachedImageLocalRotation = imageRectTransform.localRotation;
        }

        void UpdateTileBasis()
        {
            basisXUnit = (Vector2)(imageRectTransform.localRotation * Vector3.right);
            basisYUnit = (Vector2)(imageRectTransform.localRotation * Vector3.up);
            basisX = basisXUnit * tileSize.x;
            basisY = basisYUnit * tileSize.y;
        }

        RectTransform GetRootCanvasRectTransform()
        {
            if (image.canvas == null)
            {
                throw new InvalidOperationException("ScrollBackgroundActor: image は Canvas 配下に置いてください。");
            }

            if (image.canvas.rootCanvas.transform is not RectTransform rootCanvasRectTransform)
            {
                throw new InvalidOperationException("ScrollBackgroundActor: rootCanvas の RectTransform を取得できません。");
            }

            return rootCanvasRectTransform;
        }

        static Vector2 GetRootCanvasSize(RectTransform rootCanvasRectTransform)
        {
            var size = rootCanvasRectTransform.rect.size;
            if (size.x <= 0f || size.y <= 0f)
            {
                throw new InvalidOperationException("ScrollBackgroundActor: rootCanvas の Rect サイズが 0 です。");
            }

            return size;
        }

        Vector2 ProjectViewSizeToTileAxes(Vector2 viewSize)
        {
            if (basisXUnit.sqrMagnitude <= 0f || basisYUnit.sqrMagnitude <= 0f)
            {
                throw new InvalidOperationException("ScrollBackgroundActor: 回転軸ベクトルを計算できません。");
            }

            var axisX = basisXUnit.normalized;
            var axisY = basisYUnit.normalized;

            var neededWidthOnTileX = (Mathf.Abs(axisX.x) * viewSize.x) + (Mathf.Abs(axisX.y) * viewSize.y);
            var neededWidthOnTileY = (Mathf.Abs(axisY.x) * viewSize.x) + (Mathf.Abs(axisY.y) * viewSize.y);
            return new Vector2(neededWidthOnTileX, neededWidthOnTileY);
        }

        void ApplyTilePositions()
        {
            for (var i = 0; i < tiles.Count; i++)
            {
                tiles[i].rectTransform.anchoredPosition = baseAnchoredPosition + tileOffsets[i] + scrollOffset;
            }
        }

        Vector2 GetMoveDirection()
        {
            var radians = angleDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        static float WrapOffset(float value, float length)
        {
            return Mathf.Repeat(value + length, length * 2f) - length;
        }
    }
}
