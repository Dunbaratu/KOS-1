using kOS.Safe.Utilities;
using kOS.Safe.Encapsulation;

namespace kOS.Safe.Screen
{
    public interface IInterpreter : IScreenBuffer
    {
        void Type(char ch);
        bool SpecialKey(char key);
        string GetCommandHistoryAbsolute(int absoluteIndex);
        void SetInputLock(bool isLocked);
        bool IsAtStartOfCommand();
        void Reset();
        UniqueSetValue<UserDelegate> GetKeypressWatchers();
    }
}