using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using SlimDX;
using Graphics.Content;

namespace Client.Game.Interface
{
    public enum InGameMenuResult
    {
        MainMenu,
        Resume,
        Restart,
    }
    class InGameMenu : InGameMenuScreen
    {
        public InGameMenu()
        {
            Size = new Vector2(250, 400);
            LargeWindow = true;

            FlowLayout leftButtons = new FlowLayout
            {
                HorizontalFill = true,
                AutoSize = true,
                Newline = false,
                Anchor = Graphics.Orientation.Left
            };
            bottomBar.AddChild(leftButtons);
            ButtonBase returnToMainMenu = new StoneButton
            {
                Text = Locale.Resource.GenQuit
            };
            returnToMainMenu.Click += new EventHandler(returnToMainMenu_Click);
            leftButtons.AddChild(returnToMainMenu);

            ButtonBase restart = new StoneButton
            {
                Text = Locale.Resource.GenRestart
            };
            restart.Click += new EventHandler(restart_Click);
            leftButtons.AddChild(restart);

            ButtonBase options = new StoneButton
            {
                Text = Locale.Resource.GenOptions
            };
            options.Click += new EventHandler(options_Click);
            leftButtons.AddChild(options);

            ButtonBase returnToGame = new LargeStoneButton
            {
                Text = Locale.Resource.GenResume,
                Anchor = global::Graphics.Orientation.Right,
                Hotkey = new KeyCombination { Key = System.Windows.Forms.Keys.Escape },
                Size = new Vector2(210, 62)
            };
            returnToGame.Click += new EventHandler(returnToGame_Click);
            bottomBar.AddChild(returnToGame);
            Result = InGameMenuResult.Resume;
        }

        void options_Click(object sender, EventArgs e)
        {
            Visible = false;
            Program.Instance.OpenOptionsWindow(true);
            Program.Instance.OnOptionWindowedClosed += new Action(Instance_OnOptionWindowedClosed);
        }

        void Instance_OnOptionWindowedClosed()
        {
            Visible = true;
            Program.Instance.OnOptionWindowedClosed -= new Action(Instance_OnOptionWindowedClosed);
        }

        public InGameMenuResult Result { get; private set; }

        void restart_Click(object sender, EventArgs e)
        {
            Result = InGameMenuResult.Restart;
            Close();
        }

        void returnToMainMenu_Click(object sender, EventArgs e)
        {
            Result = InGameMenuResult.MainMenu;
            Close();
        }

        void returnToGame_Click(object sender, EventArgs e)
        {
            Result = InGameMenuResult.Resume;
            Close();
        }


    }

}
