using System;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.UserIO;


namespace kOS.Safe.Encapsulation
{
    /// <summary>
    /// Provides the kerboscript program with a wrapper around the
    /// kOS.Safe.UserIO.UnicodeCommand.cs enum type.
    /// </summary>
    [kOS.Safe.Utilities.KOSNomenclature("Keycodes")]
    public class Keycodes : Structure
    {
        private static Keycodes instance;
        
        public static Keycodes Instance
        {
            get
            {
                if (instance == null)
                    instance = new Keycodes();
                return instance;
            }
        }
        
        public Keycodes()
        {
            InitializeSuffixes();
        }
        
        private void InitializeSuffixes()
        {
            // make a suffix for each name in the UnicodeCommand enum:
            foreach (UnicodeCommand val in UnicodeCommand.GetValues(typeof(UnicodeCommand)))
            {
                AddSuffix(UnicodeCommand.GetName(typeof(UnicodeCommand), val), new NoArgsSuffix<StringValue>(() => new StringValue((char)val)));
            }
        }
                
    }
}
