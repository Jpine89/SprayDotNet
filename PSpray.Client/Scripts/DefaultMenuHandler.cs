using ScaleformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using ScaleformUI.Elements;
using CitizenFX.Core.UI;
using PSpray.Client.Entities;

namespace PSpray.Client.Scripts
{
    internal class DefaultMenuHandler
    {
        private static readonly object _padlock = new();
        private static DefaultMenuHandler _instance;
        private long txd;
        private List<AddonFont> _font;
        private string _fontName;

        private DefaultMenuHandler()
        {
            Init();
            Debug.WriteLine("^2PSpray Default Menu Handler has been initialised.");
        }

        internal static DefaultMenuHandler Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new DefaultMenuHandler();
                }
            }
        }

        private async void Init()
        {
            SetupEventHandler();
            SetupRegisterCommands();
            _font = FontHandler.Instance.returnList();
        }

        private void SetupEventHandler()
        {
            Main.Instance.EventHandlerDictionary.Add("pspray:Default_Menu_Trigger", new Action(DefaultMenuTrigger));
            Main.Instance.EventHandlerDictionary.Add("pspray:Menu_Font", new Action<string>(MenuFont));

        }

        private void SetupRegisterCommands()
        {
            RegisterCommand("MenuReset", new Action(MenuReset), false);
        }

        private void MenuReset()
        {
            Debug.WriteLine("Command Called");
            Debug.WriteLine(MenuHandler.IsAnyMenuOpen.ToString());
        }
        private void DefaultMenuTrigger()
        {
            txd = API.CreateRuntimeTxd("scaleformui");
            DefaultMenu();
        }

        public async Task DefaultMenu()
        {
            long _titledui = API.CreateDui("https://i.imgur.com/3yrFYbF.gif", 288, 130);
            API.CreateRuntimeTextureFromDuiHandle(txd, "bannerbackground", API.GetDuiHandle(_titledui));

            // first true means add menu Glare scaleform to the menu
            // last true means it's using the alternative title style
            UIMenu exampleMenu = new UIMenu("ScaleformUI", "ScaleformUI ~o~SHOWCASE", new PointF(20, 200), "scaleformui", "bannerbackground", true, true);
            exampleMenu.MaxItemsOnScreen = 7; // To decide max items on screen at time, default 7
            exampleMenu.BuildingAnimation = MenuBuildingAnimation.LEFT_RIGHT;
            exampleMenu.AnimationType = MenuAnimationType.BACK_INOUT;
            exampleMenu.ScrollingType = ScrollingType.CLASSIC;


            UIMenuItem notificationsItem = new UIMenuItem("Set Spray Text", "Let's try them!");
            exampleMenu.AddItem(notificationsItem);


            float scaleValue = 1f;
            UIMenuDynamicListItem dynamicItem = new UIMenuDynamicListItem("Font Size", "Try pressing ~INPUT_FRONTEND_LEFT~ or ~INPUT_FRONTEND_RIGHT~", scaleValue.ToString("F3"), async (sender, direction) =>
            {
                if (direction == UIMenuDynamicListItem.ChangeDirection.Left && scaleValue > -.8) scaleValue -= 0.2f;
                if (direction == UIMenuDynamicListItem.ChangeDirection.Right && scaleValue < 5.8) scaleValue += 0.2f;

                Debug.WriteLine("ScaleValue: " + scaleValue);

                BaseScript.TriggerEvent("pspray:Scale_Spray", scaleValue);
                return scaleValue.ToString("F3");
            });
            dynamicItem.BlinkDescription = true;
            exampleMenu.AddItem(dynamicItem);


            int fontIndex = 0;
            UIMenuDynamicListItem fontItem = new UIMenuDynamicListItem($"Font Name: ", "Try pressing ~INPUT_FRONTEND_LEFT~ or ~INPUT_FRONTEND_RIGHT~", fontIndex.ToString("F3"), async (sender, direction) =>
            {
                if (direction == UIMenuDynamicListItem.ChangeDirection.Left && fontIndex > 0) fontIndex -= 1;
                if (direction == UIMenuDynamicListItem.ChangeDirection.Right && fontIndex < _font.Count - 1) fontIndex += 1;

                BaseScript.TriggerEvent("pspray:Font_Spray", fontIndex);
                Debug.WriteLine($"The Current Font is:: {_fontName}");
                //return fontIndex.ToString("F3");
                return _fontName;
            });
            fontItem.BlinkDescription = true;
            exampleMenu.AddItem(fontItem);



            UIMenuItem cookItem = new UIMenuItem("Font Color", "Select a Hex Color");
            cookItem.SetRightLabel("rightLabel");
            cookItem.LabelFont = ScaleformFonts.ENGRAVERS_OLD_ENGLISH_MT_STD;
            cookItem.RightLabelFont = ScaleformFonts.PRICEDOWN_GTAV_INT;
            exampleMenu.AddItem(cookItem);
            UIVehicleColourPickerPanel sidePanelB = new UIVehicleColourPickerPanel(PanelSide.Right, "ColorPicker");
            cookItem.AddSidePanel(sidePanelB);
            cookItem.SetRightBadge(BadgeIcon.STAR);
            sidePanelB.OnVehicleColorPickerSelect += (item, panel, value) =>
            {
                //Due to some weird bug, add 2 to value to get proper index
                string hexValue = $"#{VehicleColors.VehiclePairs[value + 2].R:X2}{VehicleColors.VehiclePairs[value + 2].G:X2}{VehicleColors.VehiclePairs[value + 2].B:X2}";
                BaseScript.TriggerEvent("pspray:Color_Spray", hexValue);
                //Notifications.ShowNotification($"Vehicle Color: ");

                //Debug.WriteLine($"{value+2} -- {(VehicleColor)value} -- " +
                //    $"{VehicleColors.VehiclePairs[value + 2].R} ->{VehicleColors.VehiclePairs[value + 2].R:X2}, " +
                //    $"{VehicleColors.VehiclePairs[value + 2].G}->{VehicleColors.VehiclePairs[value + 2].G:X2}, " +
                //    $"{VehicleColors.VehiclePairs[value + 2].B}->{VehicleColors.VehiclePairs[value + 2].B:X2}");
                //Debug.WriteLine($"{(VehicleColor)value} -- {r}, {g}, {b}");
                sidePanelB.Title = ((VehicleColor)value).ToString();
            };


            UIMenuItem bigMessageItem = new UIMenuItem("~g~Big ~w~Message ~r~Examples", "Select me to finish the spray!");
            UIMenu uiMenuBigMessage = new UIMenu("Big Message", "Big Message");
            exampleMenu.AddItem(bigMessageItem);

            string _text = string.Empty;
            notificationsItem.Activated += async (_menu, _item) =>
            {
                API.AddTextEntry("FMMC_KEY_TIP8", "Insert text (Max 50 chars):");
                string text = await Game.GetUserInput("", 50); // i set max 50 chars here as example but it can be way more!
                _text = text;
                BaseScript.TriggerEvent("pspray:Text_Spray", text);
            };

            bigMessageItem.Activated += async (_menu, _item) =>
            {
                BaseScript.TriggerEvent("pspray:Save_Spray");
                MenuHandler.CloseAndClearHistory();     
            };


            exampleMenu.Visible = true;
        }


        private void MenuFont(string fontName) { _fontName = fontName; Debug.WriteLine("I'm inside the Menu Font"); }
        
    }
}
