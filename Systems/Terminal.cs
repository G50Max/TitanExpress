using System;
using System.Collections.Generic;
using System.Linq;
using TitanExpress.Models;

namespace TitanExpress.Systems
{
    public class Terminal
    {
        private readonly List<string> _history = new List<string>();
        private string _currentInput = "";
        private readonly List<string> _inputHistory = new List<string>();
        private int _inputHistoryIndex = -1;
        private readonly int _maxHistoryLines = 100;
        private int _scrollOffset = 0;
        private int _visibleLines = 25;

        public event Action<string> OnCommandEntered;

        public string CurrentInput
        {
            get => _currentInput;
            set => _currentInput = value;
        }

        public IReadOnlyList<string> History => _history.AsReadOnly();
        public int ScrollOffset
        {
            get => _scrollOffset;
            set => _scrollOffset = Math.Max(0, value);
        }

        public void AddLine(string line)
        {
            _history.Add(line);
            if (_history.Count > _maxHistoryLines)
            {
                _history.RemoveAt(0);
            }
            _scrollOffset = 0;
        }

        public void AddLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                AddLine(line);
            }
        }

        public void SubmitInput()
        {
            if (string.IsNullOrWhiteSpace(_currentInput))
                return;

            AddLine($"> {_currentInput}");
            _inputHistory.Add(_currentInput);
            _inputHistoryIndex = _inputHistory.Count;

            OnCommandEntered?.Invoke(_currentInput.Trim());
            _currentInput = "";
        }

        public void NavigateInputHistory(bool up)
        {
            if (up)
            {
                if (_inputHistoryIndex > 0)
                {
                    _inputHistoryIndex--;
                    _currentInput = _inputHistory[_inputHistoryIndex];
                }
            }
            else
            {
                if (_inputHistoryIndex < _inputHistory.Count - 1)
                {
                    _inputHistoryIndex++;
                    _currentInput = _inputHistory[_inputHistoryIndex];
                }
                else
                {
                    _inputHistoryIndex = _inputHistory.Count;
                    _currentInput = "";
                }
            }
        }

        public void Clear()
        {
            _history.Clear();
            _scrollOffset = 0;
        }

        public List<string> GetVisibleLines()
        {
            int totalLines = _history.Count + 1;
            int startLine = Math.Max(0, totalLines - _visibleLines - _scrollOffset);
            int count = Math.Min(_visibleLines, _history.Count - startLine);

            var visible = _history.Skip(startLine).Take(count).ToList();
            return visible;
        }
    }
}
