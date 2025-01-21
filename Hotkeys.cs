using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WK.Libraries.HotkeyListenerNS;

namespace HotKey
{
    internal class Hotkeys
    {
        #region hotkeys

      
        internal static HotkeyListener hotkeyListener = new HotkeyListener();
        internal static Hotkey AOMBOT = new Hotkey(Keys.F4);
        internal static Hotkey v = new Hotkey(Keys.F3);


        #endregion
    }
}


