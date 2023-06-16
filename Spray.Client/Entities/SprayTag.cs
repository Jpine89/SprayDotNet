namespace Spray.Client.Entities
{
    internal class SprayTag
    {
        private Scaleform _scaleform;
        private string _text;
        private string _font = "Beat Street";
        private string _color = "#FA1C09";

        public Scaleform Scaleform
        {
            get => _scaleform;
            set
            {
                // Only update if the scaleform has changed
                if (value != _scaleform)
                {
                    _scaleform = value;
                    SetScaleformText();
                }
            }
        }

        public Vector3 Location { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(2.0f, 2.0f, 1.0f);

        /// <summary>
        /// Set the text of the scaleform
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                SetScaleformText();
            }
        }

        /// <summary>
        /// Set the font of the text
        /// </summary>
        public string Font
        {
            get => _font;
            set
            {
                _font = value;
                SetScaleformText();
            }
        }

        /// <summary>
        /// Set the color of the text
        /// </summary>
        public string Color
        {
            get => _color;
            set
            {
                _color = value;
                SetScaleformText();
            }
        }

        /// <summary>
        /// Set the text of the scaleform to the text, font, and color
        /// </summary>
        private void SetScaleformText()
        {
            if (_scaleform == null) return;
            if (!_scaleform.IsValid!) return;
            if (!_scaleform.IsLoaded!) return;

            Scaleform.CallFunction("SET_PLAYER_NAME", $"<FONT color=\"{Color}\" FACE=\"{Font}\">{Text}");
        }

        /// <summary>
        /// Draw the scaleform
        /// </summary>
        public void Draw()
        {
            if (_scaleform == null) return;
            if (!_scaleform.IsValid!) return;
            if (!_scaleform.IsLoaded!) return;

            _scaleform.Render3D(Location, Rotation, Scale);
        }
    }
}
