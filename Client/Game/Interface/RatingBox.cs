using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using Graphics.Content;
using Graphics.Interface;
using SlimDX;
using System.Drawing;

namespace Client.Game.Interface
{
    public class RatingBox : Window
    {
        public RatingBox()
        {
            ControlBox = false;
            Size = new Vector2(630, 300);
            Anchor = Orientation.Center;

            AddChild(headerTextBox);
            AddChild(difficultyRow);
            AddChild(lengthRow);
            AddChild(entertainmentRow);

            AddChild(new Label 
            { 
                Text = Locale.Resource.MapRatingCommentsLabel,
                Dock = System.Windows.Forms.DockStyle.Top,
                AutoSize = AutoSizeMode.Full,
                Margin = new System.Windows.Forms.Padding(0, 5, 0, 5)
            });
            AddChild(commentBox);

            FlowLayout bottom = new FlowLayout
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Newline = false,
                HorizontalFill = true,
                AutoSize = true,
            };
            AddChild(bottom);
            bottom.AddChild(sendButton);
            sendButton.Click += new EventHandler(sendButton_Click);
            bottom.AddChild(resetButton);
            resetButton.Click += new EventHandler(resetButton_Click);
            if (Program.Settings.DisplayMapRatingDialog != MapRatingDialogSetup.Required)
            {
                bottom.AddChild(cancelButton);
                cancelButton.Click += new EventHandler(cancelButton_Click);
            }
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        void resetButton_Click(object sender, EventArgs e)
        {
            difficultyRow.Score = 0;
            lengthRow.Score = 0;
            entertainmentRow.Score = 0;
            commentBox.Text = "";
        }

        void sendButton_Click(object sender, EventArgs e)
        {
            if (Game.Instance == null || Game.Instance.FeedbackInfo == null)
                throw new Exception("Something is null that hopefully shouldn't be.");

            if (difficultyRow.Score == 0 && lengthRow.Score == 0 && entertainmentRow.Score == 0 && commentBox.Text.Length == 0)
                return;

            FeedbackCommon.MapFeedback mf = new FeedbackCommon.MapFeedback
            {
                GameInstance = Game.Instance.FeedbackInfo,
                DifficultyRating = difficultyRow.Score,
                LengthRating = lengthRow.Score,
                EntertainmentRating = entertainmentRow.Score,
                Comments = commentBox.Text
            };
            mf.HttpPost(Settings.StatisticsURI);

            resetButton.Click -= new EventHandler(resetButton_Click);
            sendButton.Click -= new EventHandler(sendButton_Click);
            Close();
        }

        RatingRow difficultyRow = new Client.Game.Interface.RatingRow
        {
            Anchor = Orientation.TopLeft,
            Dock = System.Windows.Forms.DockStyle.Top,
            Name = Locale.Resource.MapRatingDifficulty,
            LowValueText = Locale.Resource.MapRatingTooEasy,
            HighValueText = Locale.Resource.MapRatingTooHard
        };
        RatingRow lengthRow = new Client.Game.Interface.RatingRow
        {
            Anchor = Orientation.TopLeft,
            Dock = System.Windows.Forms.DockStyle.Top,
            Name = Locale.Resource.MapRatingLength,
            LowValueText = Locale.Resource.MapRatingTooShort,
            HighValueText = Locale.Resource.MapRatingTooLong
        };
        RatingRow entertainmentRow = new Client.Game.Interface.RatingRow
        {
            Anchor = Orientation.TopLeft,
            Dock = System.Windows.Forms.DockStyle.Top,
            Name = Locale.Resource.MapRatingEntertainment,
            LowValueText = Locale.Resource.MapRatingNotFun,
            HighValueText = Locale.Resource.MapRatingVeryFun,
            LightLowerValues = true
        };
        ButtonBase sendButton = new StoneButton
        {
            Text = Locale.Resource.GenSend,
        };
        ButtonBase resetButton = new StoneButton
        {
            Text = Locale.Resource.GenReset
        };
        ButtonBase cancelButton = new StoneButton
        {
            Text = Locale.Resource.GenCancel
        };
        Label headerTextBox = new Label
        {
            AutoSize = AutoSizeMode.Full,
            Text = Locale.Resource.MapRatingHeader,
            Dock = System.Windows.Forms.DockStyle.Top,
            Margin = new System.Windows.Forms.Padding(0, 0, 0, 5)
        };
        TextBox commentBox = new TextBox
        {
            Size = new Vector2(400, 100),
            Dock = System.Windows.Forms.DockStyle.Top,
            Background = new BorderGraphic
            {
                Layout = new Graphics.Content.BorderLayout(new Rectangle(0, 0, 4, 4), new Rectangle(3, 0, 1, 4), new Rectangle(0, 3, 4, 1), new Rectangle(3, 3, 1, 1))
                {
                    BackgroundStyle = BorderBackgroundStyle.Inner,
                    Border = new Vector2(4, 4)
                },
                Texture = new TextureFromFile("Graphics.Resources.ButtonBorder.png"),
                TextureSize = new Vector2(4, 4)
            }
        };
    }


    public class RatingRow : Control
    {
        public RatingRow()
        {
            Size = new Vector2(430, 20);
            AddChild(nameBox = new Label { Background = null, Anchor = Orientation.Left, Size = new Vector2(120, 20) });
            AddChild(lowValueBox = new Label { Background = null, Anchor = Orientation.Left, Size = new Vector2(80, 20), Position = new Vector2(120, 0), TextAnchor = Orientation.Right });
            AddChild(ratingBox = new RatingControl { Size = new Vector2(140, 20), Position = new Vector2(205, 0) });
            AddChild(highValueBox = new Label { Background = null, Anchor = Orientation.Left, Size = new Vector2(80, 20), Position = new Vector2(355, 0) });
        }
        public string Name { get { return nameBox.Text; } set { nameBox.Text = value; } }
        public string LowValueText { get { return lowValueBox.Text; } set { lowValueBox.Text = value; } }
        public string HighValueText { get { return highValueBox.Text; } set { highValueBox.Text = value; } }
        public bool LightLowerValues { get { return ratingBox.LightLowerValues; } set { ratingBox.LightLowerValues = value; } }
        public int Score { get { return ratingBox.RatedScore; } set { ratingBox.RatedScore = value; } }
        private RatingControl ratingBox;
        private Label nameBox, lowValueBox, highValueBox;
    }

    public class RatingControl : Control
    {
        public RatingControl()
        {
            Anchor = Orientation.Left;
            normalTexture = new TextureFromFile("Interface/Common/StarGray.png") { DontScale = true };
            selectedTexture = new TextureFromFile("Interface/Common/Star.png") { DontScale = true };
            chosenTexture = new TextureFromFile("Interface/Common/StarSelected.png") { DontScale = true };

            for (int j = 0; j < 5; j++)
            {
                Button b = new Button
                {
                    Position = new Vector2(25 * j + (j + 1) * 2, 0),
                    Size = new Vector2(25, 25),
                    Anchor = Orientation.Left,
                    Background = new ImageGraphic
                    {
                        SizeMode = SizeMode.AutoAdjust,
                        Texture = normalTexture
                    },
                    Tag = j,
                    NormalTexture = null,
                    HoverTexture = null,
                    ClickTexture = null
                };
                buttons[j] = b;
                b.MouseEnter += new EventHandler((oo, ee) =>
                {
                    hoverScore = (int)b.Tag + 1;
                    InvalidateScore();
                });
                b.MouseLeave += new EventHandler((oo, ee) =>
                {
                    hoverScore = 0;
                    InvalidateScore();
                });
                b.Click += new EventHandler((oo, ee) =>
                {
                    RatedScore = (int)b.Tag + 1;
                    InvalidateScore();
                });
                AddChild(b);
            }
        }

        private void InvalidateScore()
        {
            int score = hoverScore > 0 ? hoverScore : RatedScore;
            for (int n = 0; n < 5; n++)
            {
                if (n == RatedScore - 1)
                    buttons[n].Background = new ImageGraphic
                    {
                        SizeMode = SizeMode.AutoAdjust,
                        Texture = chosenTexture
                    };
                else if (n < score && (LightLowerValues || n == score - 1))
                    buttons[n].Background = new ImageGraphic
                    {
                        SizeMode = SizeMode.AutoAdjust,
                        Texture = selectedTexture
                    };
                else
                    buttons[n].Background = new ImageGraphic
                    {
                        SizeMode = SizeMode.AutoAdjust,
                        Texture = normalTexture
                    };
            }
        }

        public bool LightLowerValues { get; set; }
        public int RatedScore { get { return ratedScore; } set { ratedScore = value; InvalidateScore(); } }
        private int ratedScore = 0;

        TextureFromFile normalTexture, selectedTexture, chosenTexture;
        Button[] buttons = new Button[5];
        int hoverScore = 0;
    }
}
