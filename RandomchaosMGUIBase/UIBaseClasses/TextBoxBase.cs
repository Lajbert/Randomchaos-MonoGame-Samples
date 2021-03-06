﻿using System;
using System.Collections.Generic;
using System.Windows;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class TextBoxBase : ControlBase
    {

        KeyboardStateManager KeyboardManager { get { return Game.Services.GetService<KeyboardStateManager>(); } }

        public Vector2 TextOffset = new Vector2(8, 8);

        public Color TextColor { get; set; }
        public string PromptText { get; set; }
        string text { get; set; }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
            }
        }

        string fontAsset { get; set; }
        Vector2 textSize
        {
            get
            {
                if (font != null && text != null)
                    return font.MeasureString(text);
                else
                    return Vector2.Zero;
            }
        }

        public string SelectedText { get; set; }

        SpriteFont font;

        public Color ForeColor = Color.Black;
        public Color ForePromptColor = Color.DimGray;

        string lastText = string.Empty;
        public int CursorPos { get; set; }

        Texture2D cursorTexture;

        Vector2 textRenderPos = Vector2.Zero;

        public Color UnselectedBorderColor = Color.Black;
        public Color HoverBorderColor = Color.Cyan;
        public Color SelctedBorderColor = Color.Blue;

        protected float HorizontalTextScroll = 0;

        int CursorHeight = 0;

        int SelectedStart = 0;
        int SelectedEnd = 0;

        Color SelectedTextColor;
        Color SelectedBGColor;
        Rectangle SelectedRect;

        public TextBoxBase(Game game, Rectangle sizeRect, string fontAsset, string backgroundAsset = null) : base(game, sizeRect, backgroundAsset)
        {
            this.fontAsset = fontAsset;
            text = string.Empty;
            BorderColor = UnselectedBorderColor;

            OnMouseClickEvent += SetCursorPos;

           
        }

        void SetCursorPos(object sender, bool leftButton, Vector2 pos)
        {
            if (leftButton)
            {
                // Set Cursor Pos to click position..
                float startofText = Transform.Position2D.X +  TextOffset.X;
                float endOfText = Transform.Position2D.X + textSize.X;

                if (pos.X > endOfText)
                    CursorPos = text.Length;
                else if (pos.X < startofText)
                    CursorPos = 0;
                else
                {
                    float p = pos.X - startofText; // X coord in the string.
                    float d = textSize.X / text.Length; // Width of a single character
                    float cp = p / d; // Cursor position.

                    CursorPos = (int)Math.Round(cp,0, MidpointRounding.ToEven);
                }

                resetSelectedStartEnd();
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            font = Game.Content.Load<SpriteFont>(fontAsset);

            cursorTexture = new Texture2D(GraphicsDevice, 1, 1);
            cursorTexture.SetData(new Color[] { Color.White });

            CursorHeight = (int)font.MeasureString(" ").Y;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseOver)
                BorderColor = HoverBorderColor;
            else
                BorderColor = UnselectedBorderColor;

            if (HasFocus)
            {
                BorderColor = SelctedBorderColor;

                List<Keys> keys = KeyboardManager.KeysPressedThisFrame();
                foreach (Keys key in keys)
                {
                    if ((int)key >= 65 && (int)key <= 90)
                    {
                        if (key == Keys.V && KeyboardManager.CtrlIsDown)
                        {
                            // Clipboard
                            // Not supported in .NET Core
                            //AddTextAtCursor(Clipboard.GetText());                            
                        }
                        else if (key == Keys.C && KeyboardManager.CtrlIsDown)
                        {
                            // Not supported in .NET Core
                            //Clipboard.SetText(SelectedText);
                        }
                        else
                        {
                            if (KeyboardManager.CapsLock || KeyboardManager.ShiftIsDown)
                                AddTextAtCursor(key.ToString());
                            else
                                AddTextAtCursor(key.ToString().ToLower());
                        }
                    }
                    else
                    {
                        switch (key)
                        {
                            case Keys.Apps:
                            case Keys.Attn:
                                break;
                            case Keys.Back:
                                // Delete from curso pos back
                                if (Text.Length > 0 && (CursorPos != 0 || SelectedText.Length > 0))
                                {
                                    if (!string.IsNullOrEmpty(SelectedText))
                                        cutSelectedText();
                                    else
                                        text = Text.Substring(0, CursorPos - 1) + Text.Substring(CursorPos);

                                    MoveCursorPosition(-1);                                    
                                }
                                break;
                            case Keys.BrowserBack:
                            case Keys.BrowserFavorites:
                            case Keys.BrowserForward:
                            case Keys.BrowserHome:
                            case Keys.BrowserRefresh:
                            case Keys.BrowserSearch:
                            case Keys.BrowserStop:
                            case Keys.CapsLock:
                            case Keys.ChatPadGreen:
                            case Keys.ChatPadOrange:
                            case Keys.Crsel:
                                break;
                            case Keys.D0:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor(")");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.D1:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("!");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.D2:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("\"");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.D3:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("£");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.D4:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("$");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.D5:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("%");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.D6:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("^");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.D7:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("&");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.D8:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("*");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.D9:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("(");
                                else
                                    AddTextAtCursor(key.ToString().Substring(1));
                                break;
                            case Keys.Delete:
                                if (CursorPos != Text.Length || !string.IsNullOrWhiteSpace(SelectedText))
                                {
                                    if (!string.IsNullOrEmpty(SelectedText))
                                    {
                                        CursorPos = SelectedStart;
                                        cutSelectedText();
                                    }
                                    else
                                        text = Text.Substring(0, CursorPos) + Text.Substring(CursorPos + 1);
                                }
                                break;
                            case Keys.Divide:
                                AddTextAtCursor("/");
                                break;
                            case Keys.Down:
                                break;
                            case Keys.End:
                                // Send cursor to end of line
                                if (KeyboardManager.ShiftIsDown)
                                {
                                    if (SelectedStart == 0)
                                        SelectedStart = CursorPos;

                                    SelectedEnd = text.Length;

                                    MoveCursorPosition(text.Length - CursorPos);
                                }
                                else
                                {
                                    CursorPos = Text.Length;
                                    MoveCursorPosition(0);
                                    resetSelectedStartEnd();
                                }
                                break;
                            case Keys.Enter:
                                break;
                            case Keys.EraseEof:
                            case Keys.Escape:
                            case Keys.Execute:
                            case Keys.Exsel:
                            case Keys.F1:
                            case Keys.F10:
                            case Keys.F11:
                            case Keys.F12:
                            case Keys.F13:
                            case Keys.F14:
                            case Keys.F15:
                            case Keys.F16:
                            case Keys.F17:
                            case Keys.F18:
                            case Keys.F19:
                            case Keys.F2:
                            case Keys.F20:
                            case Keys.F21:
                            case Keys.F22:
                            case Keys.F23:
                            case Keys.F24:
                            case Keys.F3:
                            case Keys.F4:
                            case Keys.F5:
                            case Keys.F6:
                            case Keys.F7:
                            case Keys.F8:
                            case Keys.F9:
                            case Keys.Help:
                                break;
                            case Keys.Home:
                                // Sent cursor to start of line
                                if (KeyboardManager.ShiftIsDown)
                                {
                                    if (SelectedEnd == 0)
                                        SelectedEnd = CursorPos;

                                    SelectedStart = 0;
                                    MoveCursorPosition(-CursorPos);
                                }
                                else
                                {
                                    CursorPos = 0;
                                    MoveCursorPosition(0);
                                    resetSelectedStartEnd();
                                }
                                break;
                            case Keys.ImeConvert:
                            case Keys.ImeNoConvert:
                                break;
                            case Keys.Insert:
                                // Overwite text
                                break;
                            case Keys.Kana:
                            case Keys.Kanji:
                            case Keys.LaunchApplication1:
                            case Keys.LaunchApplication2:
                            case Keys.LaunchMail:
                                break;
                            case Keys.Left:
                                // Move curso left..
                                if (CursorPos >= 1)
                                {
                                    MoveCursorPosition(-1);
                                    if (KeyboardManager.ShiftIsDown)
                                    {
                                        if (SelectedEnd == SelectedStart)
                                            SelectedEnd = CursorPos + 1;

                                        if (SelectedStart == 0 || SelectedStart > CursorPos)
                                            SelectedStart = CursorPos;
                                        else
                                            SelectedEnd = CursorPos;
                                        
                                        //if (SelectedEnd == SelectedStart)
                                        //    SelectedStart = CursorPos;

                                        //if (CursorPos > SelectedEnd)
                                        //    SelectedEnd = CursorPos+1;
                                        //else
                                        //    SelectedStart = CursorPos;
                                    }
                                    else
                                        resetSelectedStartEnd();
                                }
                                break;
                            case Keys.LeftAlt:
                            case Keys.LeftControl:
                            case Keys.LeftShift:
                                // Next Character has caps
                                break;
                            case Keys.LeftWindows:
                            case Keys.MediaNextTrack:
                            case Keys.MediaPlayPause:
                            case Keys.MediaPreviousTrack:
                            case Keys.MediaStop:
                                break;
                            case Keys.Multiply:
                                AddTextAtCursor("*");
                                break;
                            case Keys.None:
                            case Keys.NumLock:
                                break;
                            case Keys.NumPad0:
                            case Keys.NumPad1:
                            case Keys.NumPad2:
                            case Keys.NumPad3:
                            case Keys.NumPad4:
                            case Keys.NumPad5:
                            case Keys.NumPad6:
                            case Keys.NumPad7:
                            case Keys.NumPad8:
                            case Keys.NumPad9:
                                if (KeyboardManager.NumLock)
                                    AddTextAtCursor(key.ToString().Substring(6));
                                break;
                            case Keys.Oem8:
                            case Keys.OemAuto:
                                break;
                            case Keys.OemBackslash:
                            case Keys.OemClear:
                            case Keys.OemCloseBrackets:
                            case Keys.OemComma:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("<");
                                else
                                    AddTextAtCursor(",");
                                break;
                            case Keys.OemCopy:
                            case Keys.OemEnlW:
                                break;
                            case Keys.OemMinus:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("_");
                                else
                                    AddTextAtCursor("-");
                                break;
                            case Keys.OemOpenBrackets:
                                break;
                            case Keys.OemPeriod:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor(">");
                                else
                                    AddTextAtCursor(".");
                                break;
                            case Keys.OemPipe:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("|");
                                else
                                    AddTextAtCursor("\\");
                                break;
                            case Keys.OemPlus:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("+");
                                else
                                    AddTextAtCursor("=");
                                break;
                            case Keys.OemQuestion:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("?");
                                else
                                    AddTextAtCursor("/");
                                break;
                            case Keys.OemQuotes:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("~");
                                else
                                    AddTextAtCursor("#");
                                break;
                            case Keys.OemSemicolon:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor(":");
                                else
                                    AddTextAtCursor(";");
                                break;
                            case Keys.OemTilde:
                                if (KeyboardManager.ShiftIsDown)
                                    AddTextAtCursor("@");
                                else
                                    AddTextAtCursor("'");
                                break;
                            case Keys.Pa1:
                            case Keys.PageDown:
                            case Keys.PageUp:
                            case Keys.Pause:
                            case Keys.Play:
                            case Keys.Print:
                            case Keys.PrintScreen:
                            case Keys.ProcessKey:
                                break;
                            case Keys.Right:
                                // Move cursor right
                                if (CursorPos < Text.Length)
                                {
                                    MoveCursorPosition(1);

                                    if (KeyboardManager.ShiftIsDown)
                                    {
                                        if (SelectedEnd == SelectedStart)
                                            SelectedStart = CursorPos - 1;

                                        if (CursorPos > SelectedEnd)
                                            SelectedEnd = CursorPos;
                                        else
                                            SelectedStart = CursorPos;
                                    }
                                    else
                                        resetSelectedStartEnd();
                                }
                                break;
                            case Keys.RightAlt:
                            case Keys.RightControl:
                                break;
                            case Keys.RightShift:
                                // NExt char is upper case.
                                break;
                            case Keys.RightWindows:
                            case Keys.Scroll:
                            case Keys.Select:
                            case Keys.SelectMedia:
                            case Keys.Separator:
                            case Keys.Sleep:
                                break;
                            case Keys.Space:
                                AddTextAtCursor(" ");
                                break;
                            case Keys.Subtract:
                                AddTextAtCursor("-");
                                break;
                            case Keys.Tab:

                                break;
                            case Keys.Up:
                            case Keys.VolumeDown:
                            case Keys.VolumeMute:
                            case Keys.VolumeUp:
                            case Keys.Zoom:
                                break;

                        }
                    }

                }
            }
        }

        void resetSelectedStartEnd()
        {
            SelectedStart = 0;
            SelectedEnd = 0;

            SelectedText = string.Empty;
        }

        bool cutSelectedText()
        {
            if (!string.IsNullOrEmpty(SelectedText))
            {
                text = Text.Substring(0, SelectedStart) + Text.Substring(SelectedEnd);
                resetSelectedStartEnd();
                return true;
            }
            return false;
        }

        protected void AddTextAtCursor(string text)
        {
            if (!string.IsNullOrEmpty(SelectedText))
                cutSelectedText();

            Text = Text.Substring(0, CursorPos) + text + Text.Substring(CursorPos);

            MoveCursorPosition(text.Length);
        }

        protected virtual void MoveCursorPosition(int moveBy)
        {
            CursorPos += moveBy;

            Vector2 cursorPos = Vector2.Transform(Transform.Position2D + font.MeasureString(Text.Substring(0, CursorPos)), Matrix.Invert(Transform.World));
            while (cursorPos.X + HorizontalTextScroll >= ScissorRectangle.Width - TextOffset.X)
            {
                float ch = font.MeasureString(" ").X;
                // Move text back..
                HorizontalTextScroll -= ch * 2;
            }

            while (cursorPos.X + HorizontalTextScroll <= -TextOffset.X)
            {
                float ch = font.MeasureString(" ").X;
                // Move text back..
                HorizontalTextScroll += ch * 2;
            }
        }

        

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            
            StartDraw(gameTime);

            // Calcualte text statring position..
            textRenderPos = new Vector2(HorizontalTextScroll, textRenderPos.Y);


            if (string.IsNullOrEmpty(Text))
            {
                if (!string.IsNullOrWhiteSpace(PromptText))
                    spriteBatch.DrawString(font, PromptText, textRenderPos + TextOffset, ForePromptColor, Transform.Rotation.Z, Vector2.One * .5f, 1, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.DrawString(font, Text, textRenderPos + TextOffset, ForeColor, Transform.Rotation.Z, Vector2.One * .5f, 1, SpriteEffects.None, 0);
                DrawSelected();
            }

            if (HasFocus && (gameTime.TotalGameTime.TotalSeconds % 1) > .5f)
                DrawCursor();

            EndDraw(gameTime);
        }

        void DrawSelected()
        {
            

            if (SelectedStart != SelectedEnd)
            {
                float selectStart = font.MeasureString(Text.Substring(0, SelectedStart)).X;
                float selectEnd = font.MeasureString(Text.Substring(0, SelectedEnd)).X;
                Point topLeft = new Point((int)(textRenderPos.X + selectStart + TextOffset.X), (int)(textRenderPos.Y + TextOffset.Y));
                int width = (int)(selectEnd - selectStart);

                SelectedRect = new Rectangle(topLeft.X, topLeft.Y, width, CursorHeight);

                SelectedBGColor = new Color(255 - BackgroundColor.R, 255 - BackgroundColor.G, 255 - BackgroundColor.B, BackgroundColor.A);

                spriteBatch.Draw(BackgroundTexture, SelectedRect, SelectedBGColor);

                SelectedTextColor = new Color(255 - ForeColor.R, 255 - ForeColor.G, 255 - ForeColor.B, ForeColor.A);
                SelectedText = Text.Substring(SelectedStart, SelectedEnd - SelectedStart);

                spriteBatch.DrawString(font, SelectedText, textRenderPos + TextOffset + new Vector2(selectStart,0), SelectedTextColor, Transform.Rotation.Z, Vector2.One * .5f, 1, SpriteEffects.None, 0);
            }
        }

        void DrawCursor()
        {
            Vector2 textSize = font.MeasureString(Text.Substring(0, CursorPos));
            spriteBatch.Draw(cursorTexture, new Rectangle((int)textRenderPos.X + (int)(textSize.X + TextOffset.X), (int)(textRenderPos.Y + TextOffset.Y), 2, CursorHeight), ForeColor);
        }
    }
}
