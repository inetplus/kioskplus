using System;
using System.Collections;
using System.IO;
using AgentObjects;



namespace kioskplus
{
    class inetAgentCtl
    {
        private AgentObjects.IAgentCtlCharacterEx speaker;
        private AgentObjects.AgentClass mainAgent;
        private Boolean ibInstalled = false;
        private ArrayList animationsListe = new ArrayList();
        private bool ibAgentPos = false;

        // Balloon constants
        private const short BalloonOn = 1;
        private const short SizeToText = 2;
        private const short AutoHide = 4;
        private const short AutoPace = 8;

        public inetAgentCtl()
        {
            mainAgent = new AgentObjects.AgentClass();
            mainAgent.Connected = true;
            if (!mainAgent.Connected)
                ibInstalled = false;
            else
                ibInstalled = true;
        }

        /**
         Adult Female #1, US English, L&H TruVoice  {CA141FD0-AC7F-11D1-97A3-006008273008} 
Adult Female #2, US English, L&H TruVoice  {CA141FD0-AC7F-11D1-97A3-006008273009} 
Adult Male #1, US English, L&H TruVoice {CA141FD0-AC7F-11D1-97A3-006008273000} 
Adult Male #2, US English, L&H TruVoice {CA141FD0-AC7F-11D1-97A3-006008273001} 
Adult Male #3, US English, L&H TruVoice {CA141FD0-AC7F-11D1-97A3-006008273002} 
Adult Male #4, US English, L&H TruVoice {CA141FD0-AC7F-11D1-97A3-006008273003} 
Adult Male #5, US English, L&H TruVoice {CA141FD0-AC7F-11D1-97A3-006008273004} 
Adult Male #6, US English, L&H TruVoice {CA141FD0-AC7F-11D1-97A3-006008273005} 
Adult Male #7, US English, L&H TruVoice {CA141FD0-AC7F-11D1-97A3-006008273006} 
Adult Male #8, US English, L&H TruVoice {CA141FD0-AC7F-11D1-97A3-006008273007} 
Carol, British English, L&H TTS3000  {227A0E40-A92A-11d1-B17B-0020AFED142E}  
Peter, British English, L&H TTS3000  {227A0E41-A92A-11d1-B17B-0020AFED142E}  
Linda, Dutch, L&H TTS3000  {A0DDCA40-A92C-11d1-B17B-0020AFED142E}  
Alexander, Dutch, L&H TTS3000  {A0DDCA41-A92C-11d1-B17B-0020AFED142E}  
Véronique, French, L&H TTS3000  {0879A4E0-A92C-11d1-B17B-0020AFED142E}  
Pierre, French, L&H TTS3000  {0879A4E1-A92C-11d1-B17B-0020AFED142E}  
Anna, German, L&H TTS3000  {3A1FB760-A92B-11d1-B17B-0020AFED142E}  
Stefan, German, L&H TTS3000  {3A1FB761-A92B-11d1-B17B-0020AFED142E}  
Barbara, Italian, L&H TTS3000  {7EF71700-A92D-11d1-B17B-0020AFED142E}  
Stefano, Italian, L&H TTS3000  {7EF71701-A92D-11d1-B17B-0020AFED142E}  
Naoko, Japanese, L&H TTS3000  {A778E060-A936-11d1-B17B-0020AFED142E}  
Kenji, Japanese, L&H TTS3000  {A778E061-A936-11d1-B17B-0020AFED142E}  
Shin-Ah, Korean, L&H TTS3000  {12E0B720-A936-11d1-B17B-0020AFED142E}  
Jun-Ho, Korean, L&H TTS3000  {12E0B721-A936-11d1-B17B-0020AFED142E}  
Juliana, Portuguese (Brazil), L&H TTS3000  {8AA08CA0-A1AE-11d3-9BC5-00A0C967A2D1}  
Alexandre, Portuguese (Brazil), L&H TTS3000  {8AA08CA1-A1AE-11d3-9BC5-00A0C967A2D1}  
Svetlana, Russian, L&H TTS3000  {06377F80-D48E-11d1-B17B-0020AFED142E}  
Boris, Russian, L&H TTS3000  {06377F81-D48E-11d1-B17B-0020AFED142E}  
Carmen, Spanish, L&H TTS3000  {2CE326E0-A935-11d1-B17B-0020AFED142E}  
Julio, Spanish, L&H TTS3000  {2CE326E1-A935-11d1-B17B-0020AFED142E}  
         */

        internal void SetNewAgent(String asCharacterID, String asLocalKey,  Boolean abBallonShow)
        {
            if (!ibInstalled)
                return;

            try
            {
                mainAgent.Characters.Unload(asCharacterID);
                mainAgent.Connected = true;
                ibInstalled = true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            setMainSpeaker(asCharacterID, asLocalKey, abBallonShow);
            String[] ls1 = { String.Empty, String.Empty};

            try
            {
                if (Program.gnvSplash != null)
                    ls1[0] = Program.gnvSplash.isKundenID;

                ls1[1] = asLocalKey.Substring(asLocalKey.LastIndexOf("\\") + 1);
                ls1[1] = ls1[1].Substring(0, ls1[1].IndexOf("."));

                SetText(String.Format(Program.getMyLangString("newAgent"), ls1));
            }
            catch { }
        }

        public void setMainSpeaker(String asCharacterID, String asLocalKey, Boolean abBallonShow)
        {
            if (!ibInstalled)
                return;

           
            try
            {
                mainAgent.Characters.Load(asCharacterID, (object)asLocalKey);
                speaker = mainAgent.Characters[asCharacterID];

                try
                {
                    Program.gnvSplash.il_hwnd3 = Win32User.FindWindow(inetConstants.icsAgentName, null);
                }
                catch { }

                if (abBallonShow)
                {
                    speaker.Balloon.Style = 0; // hide Ballon
                    speaker.Balloon.Visible = false;
                }
                else
                {
                    speaker.Balloon.Style = speaker.Balloon.Style | BalloonOn;
                    speaker.Balloon.Style = speaker.Balloon.Style | SizeToText;
                    speaker.Balloon.Style = speaker.Balloon.Style | AutoHide;
                }

                Console.WriteLine("Stly:" + speaker.Balloon.Style);
                // asLanguage = "de";
                String asLanguage = Program.gnvSplash.isLanguage;

                if (asLanguage.Equals(""))
                    speaker.LanguageID = 0x407;
                else if (asLanguage.Equals("en-US"))
                    speaker.LanguageID = 0x409;
                else if (asLanguage.Equals("fr"))
                    speaker.LanguageID = 0x40C;
                else if (asLanguage.Equals("tr-TR"))
                    speaker.LanguageID = 0x41F;
                else if (asLanguage.Equals("ru-RU"))
                    speaker.LanguageID = 0x419;
                else
                    speaker.LanguageID = 0x409;

                try
                {
                    if (asLanguage.Equals(""))
                        speaker.TTSModeID = "{3A1FB760-A92B-11d1-B17B-0020AFED142E}";
                    else if (asLanguage.Equals("en-US"))
                        speaker.TTSModeID = "{CA141FD0-AC7F-11D1-97A3-006008273008}";
                    else if (asLanguage.Equals("fr"))
                        speaker.TTSModeID = "{0879A4E0-A92C-11d1-B17B-0020AFED142E}";
                    else if (asLanguage.Equals("tr-TR"))
                        speaker.TTSModeID = "{3A1FB760-A92B-11d1-B17B-0020AFED142E}";
                    else if (asLanguage.Equals("ru-RU"))
                        speaker.TTSModeID = "{06377F80-D48E-11d1-B17B-0020AFED142E}";
                    else
                        speaker.TTSModeID = "{3A1FB760-A92B-11d1-B17B-0020AFED142E}";
                    
                    // speaker.SRModeID = speaker.LanguageID.ToString();
                   
                }
                catch
                { }

                ibAgentPos = false;

                GetAnimationNames();

                speaker.Show(0);

                ibInstalled = true;
            }
            catch //(FileNotFoundException fex)
            {
                ibInstalled = false;
            }
            /*catch //(Exception ex)
            {
                ibInstalled = false;
            }
            */
        }

        public void SetText(String asText)
        {
            if (!ibInstalled)
            {
                return;
            }
            if (asText == null || asText.Equals("") || asText.Equals("null"))
                return;

            Random rand = new Random();
            int index = rand.Next(0, animationsListe.Count);
            try
            {
             //   speaker.Show(0);

                try
                {
                    if (animationsListe != null && animationsListe.Count>0)
                    {
                        speaker.Play((String)animationsListe[index]);
                    }
                }
                catch // (Exception ex) 
                { }
               // Console.WriteLine(asText);
                speaker.Speak(asText, null);

                if (!ibAgentPos)
                {
                    short l;
                    l = (short)(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2);
                    l -= 100;
                    speaker.MoveTo(l, 100, 5);

                    ibAgentPos = true;
                }
                
            }
            catch // (Exception ex)
            { /*System.Windows.Forms.MessageBox.Show(ex.Message);*/ }
        }

        public void SetStop()
        {
            if (!ibInstalled)
                return;

            try
            {
                speaker.Hide(true);
                speaker.Show(1);
                speaker.StopAll(null);
            }
            catch
            { }
        }

        public void SetHideAgent()
        {
            try
            {
                speaker.Hide(true);
            }
            catch 
            { }
        }

        // get animation names and store in ArrayList
        private void GetAnimationNames()
        {
            // ensure thread safety
          //  lock (this)
          //  {
                // get animation names
                IEnumerator enumerator = mainAgent.Characters["main"].AnimationNames.GetEnumerator();

                string voiceString;

                // clear animationsliste
                animationsListe.Clear();
                speaker.Commands.RemoveAll();

                // copy enumeration to ArrayList
                while (enumerator.MoveNext())
                {
                    // remove underscores in speech string
                    voiceString = (string)enumerator.Current;
                    voiceString = voiceString.Replace("_", "underscore");

                    animationsListe.Add(enumerator.Current);

                    // add all animations as voice enabled commands
                    speaker.Commands.Add((string)enumerator.Current,
                       enumerator.Current, voiceString, true, false);
                } // end while

                // add custom command
                speaker.Commands.Add("MoveToMouse", "MoveToMouse", "MoveToMouse", true, true);
        //    } // end lock
        } // end method GetAnimationNames
    }
}
