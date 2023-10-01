using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using XGraph.Editor;

namespace XGraph
{
    public class StickyNoteView : StickyNote
    {
        private StickyNoteData _stickyNoteData;
        public StickyNoteData StickyNoteData => _stickyNoteData;

        public event Action<Vector2> OnPositionChanged;

        public StickyNoteView(StickyNoteData nodeData)
        {
            fontSize = StickyNoteFontSize.Small;
            theme = StickyNoteTheme.Black;
            
            _stickyNoteData = nodeData;

            // 监听位置变化事件
            RegisterCallback<GeometryChangedEvent>(evt =>
            {
                _stickyNoteData.editorPosition.X = evt.newRect.x;
                _stickyNoteData.editorPosition.Y = evt.newRect.y;
                _stickyNoteData.width = evt.newRect.width;
                _stickyNoteData.height = evt.newRect.height;
                OnPositionChanged?.Invoke(evt.newRect.position);
            });
            
            this.Q<TextField>("title-field").RegisterCallback<ChangeEvent<string>>(e => {
                _stickyNoteData.title = e.newValue;
            });
            this.Q<TextField>("contents-field").RegisterCallback<ChangeEvent<string>>(e => {
                _stickyNoteData.content = e.newValue;
            });

            // 设置节点标题
            title = nodeData.title;
            contents = nodeData.content;
        }
    }
}