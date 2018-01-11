using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RGX.HTML;
using RGX.Utils;
using RGX.VT;
using RGX.PIDJ;
using RGX.SOGO;
using RGX.Examine;
using RGX.Psychonauts;

namespace PPLib
{
    public static class PPRGX
    {
        public static readonly HasFilenameInfo HAS_FILE_INFO = new HasFilenameInfo();
        public static readonly Names NAMES = new Names();
        public static readonly LocInfo LOC_INFO = new LocInfo();

        // ~~ HTML
        public static readonly Token TOKEN = new Token();
        public static readonly ListElements LIST = new ListElements();
        public static readonly URLCode URL_CODE = new URLCode();

        public static readonly InvoiceID INVOICE_ID = new InvoiceID();
        public static readonly InvoiceInfo INVOICE_INFO = new InvoiceInfo();
        public static readonly TechNote TECH_NOTE = new TechNote();
        public static readonly MessageBoard MSG_BOARD = new MessageBoard();
        public static readonly Notes VT_NOTES = new Notes();

        public static readonly Contacts PIDJ_CONTACTS = new Contacts();
        public static readonly Tokens PIDJ_TOKENS = new Tokens();

        public static readonly Questions SOGO_QUESTIONS = new Questions();
        public static readonly VariableSpaces SOGO_VARIABLE_SPACES = new VariableSpaces();

        public static readonly Title EXAMINE_TITLE = new Title();
        public static readonly Tooltips EXAMINE_TOOLIPS = new Tooltips();
        public static readonly Tags EXAMINE_TAGS = new Tags();
        public static readonly EffectsMatrix EXAMINE_EFFECTS = new EffectsMatrix();
        public static readonly EditorUsername EXAMINE_EDITOR = new EditorUsername();
        public static readonly QnA EXAMINE_QnA = new QnA();
        public static readonly Citations EXAMINE_CITATIONS = new Citations();

        public static readonly SubjectiveEffectsIndex PSYCHONAUTS_EFFECTS_INDEX = new SubjectiveEffectsIndex();
    }
}
