using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CharacterEngineTest
{
    public partial class frmMain : Form
    {
        private ICharacterEngine engine;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            CharacterEngine.SetCharacterEngineMessageHandlerLogging(LogLevel.Info);
            CharacterEngine.SetCharacterEngineLogDirectory(@"C:\Users\ZACH-GAMING\AppData\Local\ZachLogs\CharacterEngine\InternalLogging", LogLevel.Max);
            CharacterEngine.SetCharacterEngineThreadCount(3);

            engine = CharacterEngine.CreateCharacterEngine();

            engine.SetInteractionLogDirectory(@"C:\Users\ZACH-GAMING\AppData\Local\ZachLogs\CharacterEngine\InternalLogging");
            engine.SetModelDirectory(@"C:\Users\ZACH-GAMING\Documents\Visual Studio 2017\Projects\CharacterEngineTest\CharacterEngineTest\models");
            engine.SetScriptDirectory(@"C:\Users\ZACH-GAMING\Documents\Visual Studio 2017\Projects\CharacterEngineTest\CharacterEngineTest\scripts");
            engine.SetProjectIdentifier("Test-v0.1");

            var interaction = engine.CreateUserNonverbalInteraction(NonverbalType.WalkingAway);
        }
    }
}
