using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Managers;
using Microsoft.MixedReality.WorldLocking.Tools;
using Solvers;
using TMPro;
using UnityEngine;

namespace Controllers.Asset
{
    public class AssetTextController : AssetController
    {
        public bool FaceUser { get; set; }
        
        public new void Awake()
        {
            base.Awake();
            ExperimentAssetMenu = ExperimentController.experimentTextMenu;
        }

        public override void OnSave()
        {
            base.OnSave();
            Asset.GetComponent<FaceUser>().enabled = FaceUser;
        }


        // --- Anchor Properties ---
        public override Dictionary<string, string> GetAnchorProps()
        {
            var baseAnchorProps = base.GetAnchorProps();
            
            var assetColor = Asset.GetComponent<TMP_Text>().color;
            Color.RGBToHSV(assetColor, H: out var h, S: out var s, V: out var v);

            var text = Asset.GetComponent<TMP_Text>().text;

            var width = Asset.GetComponent<RectTransform>().rect.width.ToString(CultureInfo.InvariantCulture);

            var childAnchorProps = new Dictionary<string, string>
            {
                [@"asset-type"] = "AssetText",
                [@"factory-type"] = "AssetTextFactory",
                [@"text"] = text,
                [@"italic"] = Italic.ToString(),
                [@"bold"] = Bold.ToString(),
                [@"serif"] = Serif.ToString(),
                [@"orient-to-user"] = FaceUser.ToString(),
                [@"width"] = width,
                [@"hue"] = h.ToString(CultureInfo.InvariantCulture),
                [@"saturation"] = s.ToString(CultureInfo.InvariantCulture),
                [@"value"] = v.ToString(CultureInfo.InvariantCulture)
            };
            
            var anchorProps = baseAnchorProps.Concat(childAnchorProps).ToDictionary(x=>x.Key,x=>x.Value);
            
            SimpleConsole.AddLine(8, "Saved the following params:");
            foreach (var kvp in anchorProps)
            {
                SimpleConsole.AddLine(8, $"{kvp.Key}: {kvp.Value}");
            }
            return anchorProps;
        }

        public override void SetAnchorProps(IDictionary<string, string> anchorProps)
        {
            SimpleConsole.AddLine(8, "Got the following params:");
            foreach (var kvp in anchorProps)
            {
                SimpleConsole.AddLine(8, $"{kvp.Key}: {kvp.Value}");
            }
            
            base.SetAnchorProps(anchorProps);
            
            Asset.GetComponent<TMP_Text>().text = anchorProps["text"];
            
            if (bool.Parse(anchorProps["italic"]))
            {
                Italic = bool.Parse(anchorProps["italic"]);
            }

            if (bool.Parse(anchorProps["bold"]))
            {
                Bold = bool.Parse(anchorProps["bold"]);
            }

            if (bool.Parse(anchorProps["serif"]))
            {
                Serif = bool.Parse(anchorProps["serif"]);
            }

            if (bool.Parse(anchorProps["orient-to-user"]))
            {
                FaceUser = bool.Parse(anchorProps["orient-to-user"]);
            }

            // Retain the line-width
            ChangeWidth(float.Parse(anchorProps["width"]));

            // Color
            ChangeHue(float.Parse(anchorProps["hue"], CultureInfo.InvariantCulture));
            ChangeSaturation(float.Parse(anchorProps["saturation"], CultureInfo.InvariantCulture));
            ChangeValue(float.Parse(anchorProps["value"], CultureInfo.InvariantCulture));
        }

        protected override void UpdateBoxCollider()
        {
            ShapeManger.UpdateBoxCollider(transform, Asset.GetComponent<RectTransform>());
        }

        // --- Change Properties ---

        public void ChangeWidth(float newWidth)
        {
            var rectTransform = Asset.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.rect.height);
            
            UpdateBoxCollider();
        }
        
        public void ChangeHue(float newHue)
        {
            Color.RGBToHSV(Asset.GetComponent<TMP_Text>().color, H: out var h, S: out var s, V: out var v);
            h = newHue;
            Asset.GetComponent<TMP_Text>().color = Color.HSVToRGB(h, s, v);
        }

        public void ChangeSaturation(float newSaturation)
        {
            Color.RGBToHSV(Asset.GetComponent<TMP_Text>().color, H: out var h, S: out var s, V: out var v);
            s = newSaturation;
            Asset.GetComponent<TMP_Text>().color = Color.HSVToRGB(h, s, v);
        }

        public void ChangeValue(float newValue)
        {
            Color.RGBToHSV(Asset.GetComponent<TMP_Text>().color, H: out var h, S: out var s, V: out var v);
            v = newValue;
            Asset.GetComponent<TMP_Text>().color = Color.HSVToRGB(h, s, v);
        }
        
        private bool _italic;
        public bool Italic
        {
            get => _italic;
            set
            {
                if (value)
                {
                    Asset.GetComponent<TMP_Text>().fontStyle = FontStyles.Italic | Asset.GetComponent<TMP_Text>().fontStyle ;
                    SimpleConsole.AddLine(8, "Set italics");
                }
                else
                {
                    Asset.GetComponent<TMP_Text>().fontStyle ^= FontStyles.Italic;
                    SimpleConsole.AddLine(8, "Unset italics");
                }
                _italic = value;
            }
        }

        private bool _bold;
        public bool Bold
        {
            get => _bold;
            set
            {
                if (value)
                {
                    Asset.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold | Asset.GetComponent<TMP_Text>().fontStyle;
                    SimpleConsole.AddLine(8, "Set bold");
                }
                else
                {
                    Asset.GetComponent<TMP_Text>().fontStyle ^= FontStyles.Bold;
                    SimpleConsole.AddLine(8, "Unset bold");
                }
                _bold = value;
            }
        }

        private bool _serif;
        public bool Serif
        {
            get => _serif;
            set
            {
                Asset.GetComponent<TMP_Text>().font = value ? AssetManager.GetSerifFont() : AssetManager.GetNonSerifFont();
                _serif = value;
            }
        }

        
    }
}
